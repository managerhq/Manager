using System;

namespace ManagerServer.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TitleAttribute : Attribute
    {
        public string[] Text { get; init; }

        public TitleAttribute(params string[] text)
        {
            Text = text;
        }
    }
}
