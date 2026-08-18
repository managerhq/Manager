using ManagerServer.Api.Businesses.Business.Reports;
using System;
using System.Collections.Generic;

namespace ManagerServer.Api.Businesses.Business
{
    [ProtoContract]
    internal class GetViewV1 : ViewEndpoint<View>
    {
        public override View AuthorizedHandle()
        {
            if (Handler != null)
            {
                var handler = typeof(Program).Assembly.GetType(Handler);
                if (handler != null)
                {
                    var endpoint = Activator.CreateInstance(handler);
                    if (endpoint is IView viewEndpoint)
                    {
                        viewEndpoint.Business = Business;
                        viewEndpoint.Key = Key;
                        viewEndpoint.Referrer = Referrer;
                        viewEndpoint.Language = Language;
                        viewEndpoint.Context = Context;
                        return viewEndpoint.GetView();
                    }
                }
            }

            if (!Key.HasValue) return null;

            var business = GetApplicationData().Businesses.Get(Business);
            var obj = business.SingleOrDefault(Key.Value);
            if (obj == null) return null;

            var objType = obj.GetType();

            if (TransactionEndpointTypes.Value.TryGetValue(objType, out var transactionEndpointType))
            {
                var endpoint = (ViewEndpoint<TransactionView>)Activator.CreateInstance(transactionEndpointType);
                CopyTo(endpoint);
                var tv = endpoint.AuthorizedHandle();
                return ViewMapper.From(tv);
            }

            return null;
        }

        private void CopyTo<T>(ViewEndpoint<T> endpoint)
        {
            endpoint.Business = Business;
            endpoint.Key = Key;
            endpoint.Referrer = Referrer;
            endpoint.Language = Language;
            endpoint.Context = Context;
        }

        private static readonly Lazy<Dictionary<Type, Type>> TransactionEndpointTypes = new(() => DiscoverEndpoints(typeof(GetTransactionView<>)));

        private static Dictionary<Type, Type> DiscoverEndpoints(Type genericBaseDefinition)
        {
            var map = new Dictionary<Type, Type>();
            foreach (var type in typeof(Program).Assembly.GetTypes())
            {
                var baseType = type.BaseType;
                if (baseType is { IsGenericType: true } && baseType.GetGenericTypeDefinition() == genericBaseDefinition)
                {
                    map[baseType.GetGenericArguments()[0]] = type;
                }
            }
            return map;
        }
    }
}
