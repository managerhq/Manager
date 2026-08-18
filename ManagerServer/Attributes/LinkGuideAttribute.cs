using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ManagerServer.Attributes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public sealed class LinkGuideAttribute : AbstractGuideAttribute
    {
        public string Text { get; init; }
        public Type Type { get; init; }

        public LinkGuideAttribute(string text, Type type)
        {
            this.Text = text;
            this.Type = type;
        }

        public string GetKey()
        {
            return Type.GetCustomAttribute<KeyAttribute>(false)?.Key ?? ConvertPascalToKebabCase(Type);
        }

        public string GetHtml()
        {
            if (!Type.GetCustomAttributes<GuideAttribute>().Any()) throw new Exception($"Not a guide: {Type.Name}");

            var titleAttribute = Type.GetCustomAttribute<TitleAttribute>();

            if (titleAttribute == null) throw new Exception(Type.Name);

            var text = string.Join("-", titleAttribute.Text);

            return @$"{Text} <a href=""guides/{GetKey()}"">{text}</a>";
        }

        private static string ConvertPascalToKebabCase(Type type)
        {
            var sb = new StringBuilder();
            foreach (var e in type.Name)
            {
                if (e == '_')
                {
                    sb.Append('.');
                }
                else
                {
                    if (char.IsUpper(e) && sb.Length > 0) sb.Append('-');
                    sb.Append(char.ToLowerInvariant(e));
                }
            }

            return sb.ToString();
        }
    }
}
