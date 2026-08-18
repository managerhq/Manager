using System;
using System.Linq;
using System.Text;
using System.Reflection;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;

namespace ManagerServer.Attributes
{
    public sealed class NamespaceAttribute : AbstractGuideAttribute
    {
        public Type Type { get; init; }
        public string Filter { get; init; }
        public NamespaceAttribute(Type type, string suffix = null)
        {
            Type = type;
            Filter = type.Namespace + ".";
            if (!string.IsNullOrWhiteSpace(suffix)) Filter += suffix + ".";
        }
    }
}
