using System.Collections.Generic;
using System.Linq;
using ManagerServer.Model;
using ManagerServer.Query.GeneralLedger;
using ManagerServer.Helpers;
using System.Diagnostics;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    internal abstract class TransactionViewer : ObjectTable<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction>
    {
        [InheritedProtoMember(300)] public bool? CashBasisAdjustment;

        private static Dictionary<Type, Type> forms = new Dictionary<Type, Type>();
        private static Dictionary<Type, Type> views = new Dictionary<Type, Type>();

        static TransactionViewer()
        {
            foreach (var e in typeof(TransactionViewer).Assembly.GetTypes().Where(x => !x.IsAbstract && x.IsSubclassOf(typeof(Form))))
            {
                if (e.BaseType.GenericTypeArguments.Length == 1)
                {
                    forms.Add(e.BaseType.GenericTypeArguments[0], e);
                }
            }

            foreach (var e in typeof(TransactionViewer).Assembly.GetTypes().Where(x => !x.IsAbstract).Where(x => x.IsSubclassOf(typeof(BaseView3))))
            {
                if (e.BaseType.GenericTypeArguments.Length == 1)
                {
                    var T = e.BaseType.GenericTypeArguments[0];
                    if (T.IsSubclassOf(typeof(ManagerServer.Model.Transaction)))
                    {
                        var transactionView = typeof(TransactionView<>).MakeGenericType(T);
                        if (!e.IsSubclassOf(transactionView)) continue;

                        views.Add(T, e);
                    }
                }
            }
        }

        public static Form GetEditHandler(string fileId, ManagerServer.Model.Transaction transaction, string referrer = null)
        {
            if (transaction == null) return null;
            if (!forms.ContainsKey(transaction.GetType())) return null;
            var form = (Form)Activator.CreateInstance(forms[transaction.GetType()]);
            form.Business = fileId;
            form.Key = transaction.Key;
            form.Referrer = referrer;
            return form;
        }

        public static BusinessTemplate GetViewHandler(string fileId, ManagerServer.Model.Transaction transaction, string referrer = null)
        {
            if (transaction == null) return null;
            if (!views.ContainsKey(transaction.GetType())) return null;
            var view3 = Activator.CreateInstance(views[transaction.GetType()]) as BaseView3;
            if (view3 != null)
            {
                view3.Business = fileId;
                view3.Key = transaction.Key;
                view3.Referrer = referrer;
                return view3;
            }
            return null;
        }

        protected virtual bool MultipleByOne()
        {
            return false;
        }

        protected virtual bool ShowTransactionAmount()
        {
            return false;
        }

        protected virtual bool ShowBaseAmount()
        {
            return false;
        }

        protected virtual bool HideAmounts()
        {
            return false;
        }

        protected virtual IEnumerable<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction> GetTransactions()
        {
            return new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[0];
        }

        protected override GeneralLedgerTransaction[] GetObjects()
        {
            var multiplyByOne = MultipleByOne();
            var transactions = GetTransactions() ?? new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[0];
            if (multiplyByOne) transactions = transactions.OrderByDescending(x => x.Date).ThenBy(x => x.AccountAmount < 0m);
            else transactions = transactions.OrderByDescending(x => x.Date).ThenBy(x => x.AccountAmount > 0m);
            return transactions.ToArray();
        }

        protected override BusinessTemplate GetEdit(GeneralLedgerTransaction o, string referrer)
        {
            return TransactionViewer.GetEditHandler(Business, o.Transaction, referrer);
        }

        protected override BusinessTemplate GetView(GeneralLedgerTransaction o, string referrer)
        {
            return TransactionViewer.GetViewHandler(Business, o.Transaction, referrer);
        }

        [Center, MinWidth, WhitespaceNoWrap]
        [Guid("835565df-f670-4e18-ac17-fc97d497e092")]
        public DateTime? GetDate(GeneralLedgerTransaction o) => o.Date > DateTime.MinValue ? o.Date : null;

        [Guid("b3f3ad5e-0fd7-44d5-acab-b3f9240f0a24")]
        public string GetTransaction(GeneralLedgerTransaction o) => o.Transaction?.GetTransactionName();

        [HideColumnIfAllEmpty]
        [Guid("c1ddffba-ea45-40c4-bb84-12ae6af1e2ec")]
        public ManagerServer.Model.Customer GetCustomer(GeneralLedgerTransaction o) => o.Customer;

        [HideColumnIfAllEmpty]
        [Guid("640bc633-7d59-4422-bc63-be2f1d8124c7")]
        public ManagerServer.Model.Supplier GetSupplier(GeneralLedgerTransaction o) => o.Supplier;

        [HideColumnIfAllEmpty]
        [Guid("c12119e1-2ee0-4944-aa02-6590d7d2c3f9")]
        public ManagerServer.Model.Employee GetEmployee(GeneralLedgerTransaction o) => o.Employee;

        [HideColumnIfAllEmpty]
        [Guid("1399702f-c001-4e50-982d-2358dc3f49d6")]
        public ManagerServer.Model.InventoryItem GetInventoryItem(GeneralLedgerTransaction o) => o.InventoryItem;

        [HideColumnIfAllEmpty]
        [Guid("284bf599-21c2-477b-8a2d-1d6524dc1af6")]
        public ManagerServer.Model.BankOrCashAccount GetBankOrCashAccount(GeneralLedgerTransaction o) => o.BankAccount;

        [HideColumnIfAllEmpty]
        [Guid("e325b352-b23a-49b7-b5e3-bb2ac1130c79")]
        public ManagerServer.Model.SpecialAccount GetSpecialAccount(GeneralLedgerTransaction o) => o.SpecialAccount;

        [HideColumnIfAllEmpty]
        [Guid("0c465d67-e1e9-4c03-8b14-ef97b3528973")]
        public ManagerServer.Model.CapitalAccount GetCapitalAccount(GeneralLedgerTransaction o) => o.CapitalAccount;

        [HideColumnIfAllEmpty]
        [Guid("b6b61a33-790c-46ec-9a26-4e9fc340fef8")]
        public string GetDescription(GeneralLedgerTransaction o) => o.Description;

        [HideColumnIfAllEmpty, Center]
        [Guid("d4b51bab-5187-4765-a713-c89593435abb")]
        public ManagerServer.Model.TaxCode GetTaxCode(GeneralLedgerTransaction o) => o.TaxCode;

        [HideColumnIfAllEmpty, Center, WhitespaceNoWrap]
        [Guid("f66aadec-73fd-4549-8fd1-bd34efab24cb")]
        public decimal? GetQty(GeneralLedgerTransaction o) => o.Qty.HasValue ? Math.Abs(o.Qty.Value) : null;

        [HideColumnIfAllEmpty, Right, WhitespaceNoWrap, Sum, Bold, RedIfNegative]
        [Guid("723e0753-1997-4b77-8b4c-0840631daeff")]
        public Tuple<decimal, string> GetAmount(GeneralLedgerTransaction o)
        {
            if (HideAmounts()) return null;
            else if (ShowTransactionAmount()) return new(MultiplyByMinusOneIfApplicable(o.TransactionAmount), MultiplyByMinusOneIfApplicable(o.TransactionAmount).ToCurrencyString(o.TransactionCurrency, CurrencySymbol.Short));
            else if (ShowBaseAmount()) return new(MultiplyByMinusOneIfApplicable(o.BaseAmount), MultiplyByMinusOneIfApplicable(o.BaseAmount).ToCurrencyString(ApplicationData.Businesses.Get(Business).Single<BaseCurrency>(), CurrencySymbol.Short));
            else return new(MultiplyByMinusOneIfApplicable(o.AccountAmount), MultiplyByMinusOneIfApplicable(o.AccountAmount).ToCurrencyString(o.AccountCurrency, CurrencySymbol.Short));
        }

        private decimal MultiplyByMinusOneIfApplicable(decimal input)
        {
            if (MultipleByOne()) return -input;
            return input;
        }

        protected override decimal? GetBalanceMovement(GeneralLedgerTransaction row)
        {
            if (HideAmounts()) return null;
            else if (ShowTransactionAmount()) return MultiplyByMinusOneIfApplicable(row.TransactionAmount);
            else if (ShowBaseAmount()) return MultiplyByMinusOneIfApplicable(row.BaseAmount);
            else return MultiplyByMinusOneIfApplicable(row.AccountAmount);
        }
    }
}