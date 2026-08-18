using System;

namespace ManagerServer.Attributes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public sealed class HeaderAttribute : AbstractGuideAttribute
    {
        public string Text { get; init; }
        
        public HeaderAttribute(string text) => Text = text;
    }
}