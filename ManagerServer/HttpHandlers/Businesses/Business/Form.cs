using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    internal abstract class Form : BusinessTemplate
    {
        [InheritedProtoMember(200)] public Guid? Key { get; set; }
        [InheritedProtoMember(201)] public Guid? Clone;
        [InheritedProtoMember(202)] public Guid? Source;
        [InheritedProtoMember(203)] public byte[] Data2;        

        private static Dictionary<Type, Type> FormsByType;

        static Form()
        {
            FormsByType = new Dictionary<Type, Type>();
            foreach (var e in typeof(Form).Assembly.GetTypes())
            {
                if (e.BaseType == null) continue;
                if (!e.BaseType.IsGenericType) continue;
                if (e.BaseType.GetGenericTypeDefinition() != typeof(NakedVueForm<>)) continue;
                FormsByType.Add(e.BaseType.GenericTypeArguments[0], e);
            }
        }

        public static Form GetEdit(ManagerServer.Model.Object o, string fileId, string referrer)
        {
            if (o == null) return null;
            if (FormsByType.TryGetValue(o.GetType(), out var form))
            {
                var output = (Form)Activator.CreateInstance(form);
                output.Key = o.Key;
                output.Business = fileId;
                output.Referrer = referrer;
                return output;
            }
            return null;
        }
    }
}