using System;
using System.Collections.Generic;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    internal static class DeletePermission
    {
        // The delete button of a form is gated by CanDelete on the form's namespace.
        // Delete requests arrive without the form, so look up the form from the object type.
        private static readonly Lazy<Dictionary<Type, string>> FormNamespaces = new(() =>
        {
            var map = new Dictionary<Type, string>();
            foreach (var type in typeof(DeletePermission).Assembly.GetTypes())
            {
                if (type.IsAbstract) continue;
                for (var baseType = type.BaseType; baseType != null; baseType = baseType.BaseType)
                {
                    if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(VueForm<>))
                    {
                        map[baseType.GetGenericArguments()[0]] = type.Namespace;
                        break;
                    }
                }
            }
            return map;
        });

        internal static bool CanDelete(HttpHandler handler, string business, Guid key)
        {
            var obj = ApplicationData.Instance.Businesses.Get(business)?.SingleOrDefault(key);
            if (obj == null) return true;

            var permissions = handler.GetCurrentUserPermissions(business);
            if (permissions.FullAccess) return true;

            return FormNamespaces.Value.TryGetValue(obj.GetType(), out var @namespace) && permissions.CanDelete(@namespace);
        }
    }
}
