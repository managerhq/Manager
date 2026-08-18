using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business
{
    [ProtoContract]
    internal class GetTransactionViewV1 : ViewEndpoint<TransactionView>
    {
        public override TransactionView AuthorizedHandle()
        {
            var business = GetApplicationData().Businesses.Get(Business);
            var obj = business.Single(Key.Value) ?? business.SingleOrDefault(Key.Value);
            if (obj == null) return null;

            if (!EndpointTypesByObjectType.Value.TryGetValue(obj.GetType(), out var endpointType)) return null;

            var endpoint = (ViewEndpoint<TransactionView>)Activator.CreateInstance(endpointType);
            endpoint.Business = Business;
            endpoint.Key = Key;
            endpoint.Referrer = Referrer;
            endpoint.Language = Language;
            endpoint.Context = Context;
            return endpoint.AuthenticatedHandle();
        }

        private static readonly Lazy<Dictionary<Type, Type>> EndpointTypesByObjectType = new(() =>
        {
            var map = new Dictionary<Type, Type>();
            foreach (var type in typeof(Program).Assembly.GetTypes())
            {
                var baseType = type.BaseType;
                if (baseType is { IsGenericType: true } && baseType.GetGenericTypeDefinition() == typeof(GetTransactionView<>))
                {
                    map[baseType.GetGenericArguments()[0]] = type;
                }
            }
            return map;
        });
    }
}
