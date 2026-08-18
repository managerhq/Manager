using System;
using System.Linq;
using System.Text;
using System.Reflection;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;

namespace ManagerServer.Attributes
{
    public sealed class FieldsAttribute : AbstractGuideAttribute
    {
        public Type Type { get; init; }
        public FieldsAttribute(Type type) => Type = type;        
    }
}
