using System;

namespace ManagerServer.Attributes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public sealed class GuideAttribute : AbstractGuideAttribute
    {
        public string Text { get; init; }
        
        public GuideAttribute(string text) => Text = text;
    }
}