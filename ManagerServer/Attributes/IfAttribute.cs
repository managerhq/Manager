using System;

namespace ManagerServer.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    internal sealed class IfTabAttribute : Attribute
    {
        internal string[] Value { get; private set; }

        internal IfTabAttribute(params string[] value)
        {
            Value = value;
        }
    }
}
