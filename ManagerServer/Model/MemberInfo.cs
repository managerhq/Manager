using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace ManagerServer.Model
{
    [ProtoContract]
    public sealed class MemberInfo
    {
        [ProtoMember(1)] public string Key { get; set; }

        private System.Reflection.MemberInfo memberInfo;

        public MemberInfo()
        {
        }

        public MemberInfo(System.Reflection.MemberInfo memberInfo)
        {
            this.Key = memberInfo.DeclaringType.FullName + "." + memberInfo.Name;
            this.memberInfo = memberInfo;
        }

        private void EnsureMemberInfo()
        {
            if (memberInfo != null) return;
            var declaringType = string.Join('.', Key.Split('.').Reverse().Skip(1).Reverse());
            var name = Key.Split('.').Last();
            memberInfo = typeof(MemberInfo).Assembly.GetType(declaringType)?.GetMember(name).FirstOrDefault();
        }

        public string UniqueName
        {
            get
            {
                EnsureMemberInfo();
                return ManagerServer.Globalization.Strings.GetPropertyValue(Name);
            }
        }

        public string Name
        {
            get
            {
                EnsureMemberInfo();
                return memberInfo?.Name;
            }
        }

        public object GetValue(object obj)
        {
            EnsureMemberInfo();
            if (memberInfo is System.Reflection.FieldInfo fieldInfo)
            {
                return fieldInfo.GetValue(obj);
            }
            return null;
        }

        private Type GetFieldOrPropertyType()
        {
            EnsureMemberInfo();
            if (memberInfo is System.Reflection.FieldInfo fieldInfo) return fieldInfo.FieldType;
            if (memberInfo is System.Reflection.PropertyInfo propertyInfo) return propertyInfo.PropertyType;
            return typeof(object);
        }

        public string DeclaringType
        {
            get
            {
                return GetFieldOrPropertyType().FullName;
            }
        }

        public Type DeclaringType2
        {
            get
            {
                EnsureMemberInfo();
                return memberInfo?.DeclaringType;
            }
        }

        public FieldType ValueType
        {
            get
            {
                var t = GetFieldOrPropertyType();
                if (t.Equals(typeof(decimal))) return FieldType.Decimal;
                if (t.Equals(typeof(DateTime))) return FieldType.Date;
                if (t.Equals(typeof(string))) return FieldType.String;
                if (t.Equals(typeof(Dictionary<Guid, string>))) return FieldType.String;
                if (t.Equals(typeof(bool))) return FieldType.Boolean;
                if (t.IsSubclassOf(typeof(ManagerServer.Model.Object))) return FieldType.Object;
                return 0;
            }
        }

        public bool IsCustomFields
        {
            get
            {
                return GetFieldOrPropertyType().Equals(typeof(Dictionary<Guid, string>));
            }
        }

        public bool IsObject
        {
            get
            {
                return GetFieldOrPropertyType().IsSubclassOf(typeof(ManagerServer.Model.Object)) || GetFieldOrPropertyType().Equals(typeof(ManagerServer.Model.IGeneralLedgerAccount));
            }
        }

        public object ObjectKey
        {
            get
            {
                var guid = GetFieldOrPropertyType().GetCustomAttribute<ManagerServer.Model.Attributes.GuidAttribute>();
                if (guid != null)
                {
                    return new { Key = guid.Value.ToString() };
                }
                else
                {
                    return null;
                }
            }
        }

        public enum FieldType : int
        {
            String = 1,
            Decimal = 2,
            Boolean = 3,
            Date = 4,
            Object = 5
        }
    }
}
