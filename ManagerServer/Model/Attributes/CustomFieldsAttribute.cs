using System;
using System.Linq;
using System.Reflection;

namespace ManagerServer.Model.Attributes
{
    [Guid("e08ef836-18d0-41e4-8254-75bab0f59e63")]
    public sealed class CustomFieldsAttribute : Attribute
    {
        static CustomFieldsAttribute()
        {
            All = typeof(ManagerServer.Model.Object).Assembly.GetTypes().Where(x => x.GetCustomAttribute<ManagerServer.Model.Attributes.CustomFieldsAttribute>() != null).ToArray();
        }

        public readonly static Type[] All;
    }
}
