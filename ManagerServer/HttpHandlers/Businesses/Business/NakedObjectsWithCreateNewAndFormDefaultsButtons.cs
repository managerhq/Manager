using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    [Key("form-defaults")]
    [Title(nameof(Strings.FormDefaults))]
    [Guide("When you create a new item, the form will start off empty. To set up a default starting point for new items, click on the `FormDefaults` button in bottom-right corner.")]
    [Guide("For example, if you'd like to set up starting form values for new `SalesInvoices`.")]
    [Guide("Go to `SalesInvoices` tab.")]
    [TabScreenshot(icon: "fa-file-invoice", name: nameof(Strings.SalesInvoices))]
    [Guide("Click `FormDefaults` in bottom-right corner.")]
    [SmallBottomButtonScreenshot(name: nameof(Strings.FormDefaults))]
    [Guide("Set initial form values (e.g. default due date)")]
    [Guide("Click `Update` button to confirm changes.")]
    [SuccessButtonScreenshot(name: nameof(Strings.Update))]
    [Guide("Now, every time you click `NewSalesInvoice` button, it will be pre-populated with your initial values set in `FormDefaults`.")]
    [Guide("Here are some ideas how to utilize `FormDefaults`:")]
    [Guide("Use `FormDefaults` to to establish initial values for custom fields.")]
    [Guide("If you are setting up form defaults for transaction form, you can activate automatic generation of reference numbers")]
    [Guide("If you're utilizing `Footers`, it's possible to establish default footers for new transactions. For instance, you might want a footer on sales invoices that provides payment instructions to customers.")]
    [Guide("You can also `Reset` form defaults by returning the initial values to its original state. When editing `FormDefaults`, click `Reset` button.")]
    [DangerButtonScreenshot(name: nameof(Strings.Reset))]
    internal abstract class NakedObjectsWithCreateNewAndFormDefaultsButtons<T> : NakedObjectsWithBatchCreateAndBatchUpdate<T> where T : ManagerServer.Model.Object, new()
    {
        private static Type formType = typeof(NakedObjectsWithCreateNewAndFormDefaultsButtons<>).Assembly.GetTypes().SingleOrDefault(x => x.IsSubclassOf(typeof(NakedVueForm<T>)));

        protected virtual void OnGetNewButton()
        {
            var key = "New" + typeof(T).Name;
            Write(Strings.GetPropertyValue(key));
        }

        protected override void OnHeaderStartSection(Context context)
        {
            if (formType != null)
            {
                var newHandler = (NakedVueForm<T>)Activator.CreateInstance(formType);
                Copy(this, newHandler);
                newHandler.Referrer = this.ToUrl();
                using (A(@class: "btn", href: newHandler.ToUrl()))
                {
                    OnGetNewButton();
                }
            }
            base.OnHeaderStartSection(context);
        }

        protected override void OnFooterEndSection(Context context)
        {
            if (formType != null)
            {
                var key = ManagerServer.Model.Object.GetGuidByType(typeof(T));

                var formDefaultHandler = (NakedVueForm<T>)Activator.CreateInstance(formType);
                formDefaultHandler.HttpContext = HttpContext;
                formDefaultHandler.Business = Business;
                formDefaultHandler.Referrer = this.ToUrl();
                formDefaultHandler.Key = key;
                using (A(href: formDefaultHandler.ToUrl(), @class: "btn btn-xs")) Write(Strings.FormDefaults);
            }
            base.OnFooterEndSection(context);
        }
    }
}
