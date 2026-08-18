using ManagerServer.Api.Businesses.Business.Settings.CustomFields.CheckboxCustomFields;
using ManagerServer.Api.Businesses.Business.Settings.CustomFields.DateCustomFields;
using ManagerServer.Api.Businesses.Business.Settings.CustomFields.ImageCustomFields;
using ManagerServer.Api.Businesses.Business.Settings.CustomFields.MultipleValueCustomFields;
using ManagerServer.Api.Businesses.Business.Settings.CustomFields.NumberCustomFields;
using ManagerServer.Api.Businesses.Business.Settings.CustomFields.TextCustomFields;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ManagerServer.Api.Businesses.Business.Settings.CustomFields
{
    internal sealed record CustomFieldsResource(
        [property: JsonPropertyName("_links")] Dictionary<string, Link> Links);

    [ProtoContract]
    internal sealed class GetCustomFields : AuthorizedEndpoint<CustomFieldsResource>
    {
        public override CustomFieldsResource AuthorizedHandle()
        {
            var links = Hyperlinks.ForCurrentDocument(this);

            links["checkboxCustomFields"] = new Link(new GetCheckboxCustomFieldBatch { Business = Business }.ToUrl());
            links["dateCustomFields"] = new Link(new GetDateCustomFieldBatch { Business = Business }.ToUrl());
            links["imageCustomFields"] = new Link(new GetImageCustomFieldBatch { Business = Business }.ToUrl());
            links["multipleValueCustomFields"] = new Link(new GetMultipleValueCustomFieldBatch { Business = Business }.ToUrl());
            links["numberCustomFields"] = new Link(new GetNumberCustomFieldBatch { Business = Business }.ToUrl());
            links["textCustomFields"] = new Link(new GetTextCustomFieldBatch { Business = Business }.ToUrl());

            return new CustomFieldsResource(links);
        }
    }
}
