using ManagerServer.Api;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business
{
    internal abstract class GetObjectBatchEndpoint<T, TGet, TPost, TPut, TDelete> : AuthorizedEndpoint<BusinessObjectsResource<T>>
        where T : Model.Object, new()
        where TGet : GetObjectEndpoint<T>, new()
        where TPost : PostObjectEndpoint<T>, new()
        where TPut : PutObjectEndpoint<T>, new()
        where TDelete : DeleteObjectEndpoint<T>, new()
    {
        [Description("Specify how many items should be skipped")]
        [InheritedProtoMember(200)] public int? Skip { get; set; }
        [Description("Specify page size. Default is 50")]
        [InheritedProtoMember(201)] public int? PageSize { get; set; }
        [Description("Specify keys which objects to retrieve")]
        [InheritedProtoMember(202)] public Guid[] Keys { get; set; }

        public override BusinessObjectsResource<T> AuthorizedHandle()
        {
            var business = ApplicationData.Instance.Businesses.Get(Business);
            var output = business.OfType<T>();
            if (Keys != null && Keys.Length > 0) output.Where(x => Keys.Contains(x.Key)).ToArray();
            if (Skip.HasValue) output = output.Skip(Skip.Value).ToArray();
            output = output.Take(PageSize ?? 50).ToArray();
            output = Filter(output);

            var items = output
                .Select(obj => new Item<T>(
                    Key: obj.Key,
                    Value: obj,
                    Links: new Dictionary<string, Link>
                    {
                        ["self"] = new Link(new TGet { Business = Business, Key = obj.Key }.ToUrl()),
                    },
                    Actions: new Dictionary<string, Link>
                    {
                        ["edit"]   = Hyperlinks.ForAction<TPut>(),
                        ["delete"] = Hyperlinks.ForAction<TDelete>(),
                    }))
                .ToArray();

            var links = Hyperlinks.ForCurrentDocument(this);
            var actions = new Dictionary<string, Link>
            {
                ["create"] = Hyperlinks.ForAction<TPost>(),
            };

            return new BusinessObjectsResource<T>(Links: links, Actions: actions, Items: items);
        }

        public virtual T[] Filter(T[] objects)
        {
            return objects;
        }
    }
}
