using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerComponents;
using ManagerServer.HttpHandlers.Businesses.Business.Reports.CustomReports;
using ManagerServer.HttpHandlers.Businesses.Business.Reports.ReportTransformationReports;
using ManagerServer.HttpHandlers.Businesses.Business.Settings.UserPermissions;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    internal abstract class PersistentObjectTable<T> : ObjectTable<T> where T : ManagerServer.Model.Object, new()
    {
        [InheritedProtoMember(307)] public bool BatchDelete { get; set; }        

        protected override HeaderButton GetPrimaryButton()
        {
            var newButtonLabel = this.GetType().GetCustomAttribute<NewButtonAttribute>()?.Value ?? $"New{typeof(T).Name}";
            return new ManagerComponents.HeaderButton()
            {
                Text = Strings.GetPropertyValue(newButtonLabel),
                Url = GetEdit(null, this.ToUrl()).ToUrl()
            };
        }

        protected override void OnTable(ManagerComponents.Table table, T[] rows)
        {
            if (BatchDelete)
            {
                table.Columns.Insert(0, new TableColumn()
                {
                    Text = Strings.BatchDelete,
                    Checkbox = true,
                    MinWidth = true,
                    WhitespaceNoWrap = true,
                    Cells = rows.Select(x => new TableCell()
                    {
                        Checkbox = CanDelete(x) ? new Tuple<string, byte[]>(nameof(BatchDelete), x.Key.ToByteArray()) : null
                    }).ToArray()
                });
            }
        }

        protected override bool IsInactive(T row)
        {
            return row.IsInactive();
        }

        protected override void OnBatchOperationsButton(FooterButton batchOperationsButton)
        {
            if (!BatchDelete)
            {
                var clone = (PersistentObjectTable<T>)this.MemberwiseClone();
                clone.BatchDelete = true;
                batchOperationsButton.Menu.Add(new Tuple<string, string>(Strings.BatchDelete, clone.ToUrl()));
            }
        }

        protected override void OnFooter(Footer footer)
        {
            if (BatchDelete)
            {
                var batchDeleteFooter = new ManagerComponents.Panel();
                batchDeleteFooter.IsActionBar = true;
                batchDeleteFooter.StartElements.Add(new HeaderButton() { Style = HeaderButton.ButtonStyle.Danger, Text = Strings.BatchDelete, Url = this.ToUrl(), Form = nameof(BatchDelete) });

                var clone = (PersistentObjectTable<T>)this.MemberwiseClone();
                clone.BatchDelete = false;
                batchDeleteFooter.StartElements.Add(new HeaderButton() { Style = HeaderButton.ButtonStyle.Secondary, Text = Strings.Cancel, Url = clone.ToUrl() });

                Write(batchDeleteFooter);
            }

            if (formType != null)
            {
                var genericArgument = formType.BaseType.GetGenericArguments().Single();
                var key = ManagerServer.Model.Object.GetGuidByType(genericArgument);
                footer.EndElements.Insert(0, new FooterButton() { Text = Strings.FormDefaults, Url = GetEdit(new T() { Key = key }, this.ToUrl()).ToUrl() });
            }
        }

        protected override void InnerGet2()
        {
            if (Request.HasFormContentType)
            {
                var form = Request.ReadFormAsync().GetAwaiter().GetResult();

                if (form.TryGetValue(nameof(BatchDelete), out var batchDelete))
                {
                    var batchDeleteKeys = batchDelete.Select(x => new Guid(Convert.FromBase64String(x))).ToArray();
                    ApplicationData.Businesses.Process(Business, batchDeleteKeys, GetUserName());
                    Response.Redirect(this.ToUrl());
                    return;
                }                
            }

            base.InnerGet2();          
        }

        protected override T[] GetObjects()
        {
            return ApplicationData.Businesses.Get(Business).OfType<T>();
        }

        private static Type formType = typeof(Program).Assembly.GetTypes().SingleOrDefault(x => x.IsSubclassOf(typeof(NakedVueForm<T>)));

        protected override BusinessTemplate GetEdit(T o, string referrer)
        {
            if (formType != null)
            {
                var editHandler = (NakedVueForm<T>)Activator.CreateInstance(formType);
                editHandler.HttpContext = HttpContext;
                editHandler.Key = o?.Key;
                editHandler.Business = Business;
                editHandler.Referrer = referrer;
                return editHandler;
            }
            else if (typeof(T) == typeof(ManagerServer.Model.UserPermissions))
            {
                var viewHandler = new UserPermissionsForm();
                viewHandler.HttpContext = HttpContext;
                viewHandler.Key = o?.Key;
                viewHandler.Business = Business;
                viewHandler.Referrer = referrer;
                return viewHandler;
            }
            return null;
        }

        private bool CanDelete(T row)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var lockDate = database.Single<LockDate>().GetLockDate();

            if (lockDate.HasValue && row is Transaction transaction && transaction.GetGeneralLedgerTransactions(database).Any() && transaction.GetGeneralLedgerTransactions(database).Min(x => x.Date) < lockDate.Value)
            {
                return false;
            }
            else if (row is InventoryUnitCost)
            {
                return true;
            }
            else if (database.GetGeneralLedgerTransactions().GetTransactionsByForeignKey(row.Key).Any())
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private static Type defaultView = Assembly.GetHttpHandlerTypeByCamelCaseKey($"{typeof(T).Name}View");

        protected override BusinessTemplate GetView(T o, string referrer)
        {
            if (defaultView != null && defaultView.IsSubclassOf(typeof(BaseView3)))
            {
                var viewHandler = (BaseView3)Activator.CreateInstance(defaultView);
                viewHandler.HttpContext = HttpContext;
                viewHandler.Key = o.Key;
                viewHandler.Business = Business;
                viewHandler.Referrer = referrer;
                return viewHandler;
            }
            else if (typeof(T) == typeof(ManagerServer.Model.CustomReport))
            {
                var viewHandler = new CustomReportView();
                viewHandler.HttpContext = HttpContext;
                viewHandler.Key = o.Key;
                viewHandler.Business = Business;
                viewHandler.Referrer = referrer;
                return viewHandler;                
            }  
            else
            {
                return null;
            }
        }
    }
}