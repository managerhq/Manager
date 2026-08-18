using System;
using System.Linq;
using System.Reflection;

namespace ManagerServer.Model
{
    public interface ICustomField : IObject
    {
        string Name { get; }
        int? Position { get; }
        Guid[] Placement { get; }
        string Description { get; }
        bool DisplayOnView { get; }
        bool ShowAtTheTop { get; }
        bool Inactive { get; }
        bool ExcludeFromCopyingOrCloning { get; }
        bool LockedForManualEditing { get; }

        public bool Contains(Type type)
        {
            if (Placement == null) return false;
            if (Placement.Length == 0) return false;
            if (type == null) return false;
            if (type.GetCustomAttribute<ManagerServer.Model.Attributes.CustomFieldsAttribute>() == null) return false;
            var key = type.GetCustomAttribute<ManagerServer.Model.Attributes.GuidAttribute>().Value;
            return Placement.Contains(key);
        }
    }
}
