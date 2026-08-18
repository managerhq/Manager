using System;
using System.Linq;
using System.Collections.Generic;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.CustomThemes
{
    [ProtoContract]
    [NamespaceEntry]
    [Title(nameof(Strings.Themes))]
    [NewButton(nameof(Strings.NewTheme))]
    [Guide("Themes control the visual appearance and layout of your business documents such as invoices, quotes, orders, and other forms.")]
    [SettingsItemScreenshot("fa-paint-roller", nameof(Strings.Themes))]
    [Guide("You can create custom themes to match your company branding, including colors, fonts, logos, layout preferences, and importantly, to display information like bank details.")]
    [Header("Making Themes Apply Automatically")]
    [Guide("If you want your custom theme to appear automatically on new documents without having to select it each time, you need to set up form defaults:")]
    [Guide("1. Go to the relevant tab (e.g., **Sales Invoices**, **Sales Quotes**, **Purchase Orders**)")]
    [Guide("2. Click the **Form Defaults** button at the bottom of the screen")]
    [Guide("3. Check the **Custom Theme** checkbox")]
    [Guide("4. Select your preferred theme from the dropdown")]
    [Guide("5. Click **Update** to save your form defaults")]
    [LinkGuide("To learn more about *Form Defaults* see:", typeof(NakedObjectsWithCreateNewAndFormDefaultsButtons<>))]
    [Guide("Now every new document of that type will automatically use your selected theme.")]
    [Header("Why There's No View Button")]
    [Guide("Unlike other items in the software, themes don't have a **View** button because a theme cannot be viewed on its own. A theme is a template that only becomes visible when merged with actual data from a specific invoice, quote, order, or other document.")]
    [Header("How to Preview Your Theme")]
    [Guide("To see how your theme looks while editing it, we recommend using two browser tabs:")]
    [Guide("1. In one tab, open the theme for editing by clicking the **Edit** button")]
    [Guide("2. In another tab, open a specific document (invoice, quote, or order) that has your theme selected")]
    [Guide("This way, you can make changes to your theme in the first tab, then switch to the second tab and refresh the document to immediately see how your theme looks when applied to real data.")]
    [Header("Creating and Customizing Themes")]
    [Guide("To create a new theme, click the **New Theme** button. You can create multiple themes for different purposes - for example, one theme for invoices and another for quotes.")]
    [Guide("Themes can be customized with HTML and CSS to achieve precise control over document appearance. This includes adding company logos, adjusting margins, changing fonts, modifying color schemes.")]
    [Guide("Once created, themes need to be selected when creating or editing documents. To avoid manual selection each time, use the **Form Defaults** feature as described above.")]
    [Columns]
    internal sealed class CustomThemes : PersistentObjectTable<Model.CustomTheme>
    {
        [Guid("ff70ec13-3b1f-4ea4-b1ff-7022c63624ab")]
        public string GetName(Model.CustomTheme o) => o.Name;

        /*
        protected override TitleButton[] GetPrimaryButtons()
        {
            return new TitleButton[] {
                new TitleButton() { Name = Strings.NewTheme, HttpHandler = new ThemeForm() { FileID = FileID } },
            };
        }

        internal override bool IsEmpty(ManagerServer.Extensions.TabsExtensions.Item[] tabs)
        {
            return !Manager.ApplicationData.Businesses.Get(FileID).OfType<Manager.Model.Theme>().Any();
        }

        protected override Column[] GetColumns()
        {
            return new Column[] {
                new Column("name") { Name = Strings.Name }
            };
        }

        protected override IEnumerable<Row> GetRows()
        {
            var themes = Manager.ApplicationData.Businesses.Get(FileID).OfType<Manager.Model.Theme>();

            var rows = new Row[themes.Length];
            for (int i = 0; i < rows.Length; i++)
            {
                var theme = themes[i];

                rows[i] = new Row()
                {
                    Inactive = theme.Inactive,
                    Edit = new ThemeForm() { FileID = FileID, Key = theme.Key },
                    Cells = new Cell[]
                    {
                        new Cell() { Value = theme.Name },
                    }
                };
            }
            return rows;
        }
        */
    }
}