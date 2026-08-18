using System;
using System.IO;
using ManagerServer.Model;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.CustomThemes
{
    [ProtoContract]
    [Title(nameof(Strings.Theme))]
    [Guide("Themes allow you to customize the visual appearance of your business documents such as invoices, quotes, statements, and other forms.")]
    [Guide("Each theme contains HTML and CSS code that controls the layout, colors, fonts, and overall design of documents when they are printed or emailed to customers and suppliers.")]
    [Header("Using Themes")]
    [Guide("After creating a theme, you can select it when customizing individual forms under the `Settings` tab. Different forms can use different themes to match your branding needs.")]
    [Guide("Themes use the Liquid templating language, which allows you to include dynamic content from your transactions while maintaining consistent formatting.")]
    [Fields(typeof(CustomTheme))]
    internal sealed class CustomThemeForm : NakedVueForm<CustomTheme>
    {
        protected override void OnSource(CustomTheme form, Model.Object source)
        {
            if (!Key.HasValue && source == null)
            {
                form.Template = ReadEmbeddedResource("ManagerServer.wwwroot.resources.themes.skeleton.html")
                    .Replace("{{OPENAPI_SPEC}}", ReadEmbeddedResource("ManagerServer.wwwroot.resources.themes.view-v1.json") ?? string.Empty);
            }

            if (source is CustomTheme theme)
            {
                form.Name = theme.Name;
                form.Template = theme.Template;
            }
        }

        private static string ReadEmbeddedResource(string name)
        {
            using var s = typeof(Program).Assembly.GetManifestResourceStream(name);
            if (s == null) return null;
            using var sr = new StreamReader(s);
            return sr.ReadToEnd();
        }
    }
}