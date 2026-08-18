using System;
using System.Reflection;
using ProtoBuf.Meta;

namespace ManagerServer.Attributes
{
    // Property attribute for including base class members in derived class messages without using "oneof" for polymorphism. Can be removed once https://github.com/protobuf-net/protobuf-net/issues/916 is implemented.
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class InheritedProtoMemberAttribute : ProtoMemberAttribute
    {
        public InheritedProtoMemberAttribute(int tag) : base(tag)
        {
        }

        public static void AddInheritedMembersIn(RuntimeTypeModel protoModel)
        {
            protoModel.AfterApplyDefaultBehaviour += (_, e) => AddInheritedMembers(e.Type, e.MetaType);
        }

        private static void AddInheritedMembers(Type baseType, MetaType metaType)
        {
            if (baseType == null) return;
            if (baseType == typeof(object)) return;

            var fields = baseType.GetFields(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public);
            foreach (var field in fields)
            {
                var protoMemberAttribute = field.GetCustomAttribute<InheritedProtoMemberAttribute>();
                if (protoMemberAttribute != null)
                {
                    var valueMember = metaType.AddField(protoMemberAttribute.Tag, field.Name);
                    valueMember.DataFormat = protoMemberAttribute.DataFormat;
                    // TODO: Set other ValueMember values from ProtoMemberAttribute if needed...
                }
            }

            var properties = baseType.GetProperties(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public);
            foreach (var property in properties)
            {
                var protoMemberAttribute = property.GetCustomAttribute<InheritedProtoMemberAttribute>();
                if (protoMemberAttribute != null)
                {
                    var valueMember = metaType.AddField(protoMemberAttribute.Tag, property.Name);
                    valueMember.DataFormat = protoMemberAttribute.DataFormat;
                    // TODO: Set other ValueMember values from ProtoMemberAttribute if needed...
                }
            }

            AddInheritedMembers(baseType.BaseType, metaType);
        }
    }
}