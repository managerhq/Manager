using System;
using System.Linq;
using System.Reflection;

namespace ManagerServer
{
    public static class ReflectionExtensions
    {
        public static MemberInfo[] GetFieldsAndProperties(this Type type, BindingFlags flags = BindingFlags.Instance | BindingFlags.Public)
        {
            return type.GetFields(flags).Cast<MemberInfo>().Concat(type.GetProperties(flags)).ToArray();
        }

        public static MemberInfo GetFieldOrProperty(this Type type, string name, BindingFlags flags = BindingFlags.Instance | BindingFlags.Public)
        {
            return (MemberInfo)type.GetField(name, flags) ?? type.GetProperty(name, flags);
        }

        public static Type GetMemberType(this MemberInfo member) => member switch
        {
            FieldInfo f => f.FieldType,
            PropertyInfo p => p.PropertyType,
            _ => throw new ArgumentException("Member must be a field or property", nameof(member)),
        };

        public static object GetMemberValue(this MemberInfo member, object obj) => member switch
        {
            FieldInfo f => f.GetValue(obj),
            PropertyInfo p => p.GetValue(obj),
            _ => throw new ArgumentException("Member must be a field or property", nameof(member)),
        };

        public static void SetMemberValue(this MemberInfo member, object obj, object value)
        {
            switch (member)
            {
                case FieldInfo f: f.SetValue(obj, value); break;
                case PropertyInfo p: p.SetValue(obj, value); break;
                default: throw new ArgumentException("Member must be a field or property", nameof(member));
            }
        }

        public static bool CanWrite(this MemberInfo member) => member switch
        {
            FieldInfo f => !f.IsInitOnly && !f.IsLiteral,
            PropertyInfo p => p.CanWrite && p.SetMethod != null,
            _ => false,
        };
    }
}
