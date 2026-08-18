using System;

namespace ManagerServer.Model.Attributes
{
    public sealed class IconAttribute : Attribute
    {
        public string Value { get; init; }

        public IconAttribute(string value)
        {
            Value = value;
        }
    }
}
