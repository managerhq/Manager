using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Model.Enums;
using ManagerServer.Globalization;
using System.Text;
using System.IO;
using ManagerServer.Model;
using System.Reflection;
using ManagerServer.Model.Attributes;
using ManagerServer.Model.Obsolete;
using System.Threading.Tasks;

namespace ManagerServer
{
    public static partial class Upgrade
    {
        private static async Task<IEnumerable<Model.Object>> Upgrade290(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete67.BankRule>())
            {
                if (e.Lines == null || e.Lines.Length == 0) continue;

                list.Add(new ReceiptRule()
                {
                    Key = Guid.CreateVersion7(),
                    Obsolete_AndDescriptionContains = e.AndDescriptionContains,
                    IfBankAccountIs = e.IfBankAccountIs,
                    Customer = e.Customer,
                    OtherContact = e.OtherContact,
                    PaidBy = e.PayerOrPayee,
                    Supplier = e.Supplier,
                    Lines = new ReceiptRule.Line[]
                    {
                        new ReceiptRule.Line()
                        {
                            Account = e.Lines[0].Account,
                            AccountsPayableSupplier = e.Lines[0].AccountsPayableSupplier,
                            AccountsReceivableCustomer = e.Lines[0].AccountsReceivableCustomer,
                            BillableExpenseCustomer = e.Lines[0].BillableExpenseCustomer,
                            CapitalAccount = e.Lines[0].CapitalAccount,
                            Employee = e.Lines[0].Employee,
                            ExpenseClaimsPayer = e.Lines[0].ExpenseClaimPayer,
                            FixedAsset = e.Lines[0].FixedAsset,
                            IntangibleAsset = e.Lines[0].IntangibleAsset,
                            SpecialAccount = e.Lines[0].SpecialAccount,
                            SubAccount = e.Lines[0].SubAccount,
                            TaxCode = e.Lines[0].TaxCode,
                            Division = e.Lines[0].TrackingCode
                        }
                    }
                });

                list.Add(new PaymentRule()
                {
                    Key = Guid.CreateVersion7(),
                    Obsolete_AndDescriptionContains = e.AndDescriptionContains,
                    IfBankAccountIs = e.IfBankAccountIs,
                    Customer = e.Customer,
                    OtherContact = e.OtherContact,
                    Payee = e.PayerOrPayee,
                    Supplier = e.Supplier,
                    Lines = new PaymentRule.Line[]
                    {
                        new PaymentRule.Line()
                        {
                            Account = e.Lines[0].Account,
                            AccountsPayableSupplier = e.Lines[0].AccountsPayableSupplier,
                            AccountsReceivableCustomer = e.Lines[0].AccountsReceivableCustomer,
                            BillableExpenseCustomer = e.Lines[0].BillableExpenseCustomer,
                            CapitalAccount = e.Lines[0].CapitalAccount,
                            Employee = e.Lines[0].Employee,
                            ExpenseClaimsPayer = e.Lines[0].ExpenseClaimPayer,
                            FixedAsset = e.Lines[0].FixedAsset,
                            IntangibleAsset = e.Lines[0].IntangibleAsset,
                            SpecialAccount = e.Lines[0].SpecialAccount,
                            SubAccount = e.Lines[0].SubAccount,
                            TaxCode = e.Lines[0].TaxCode,
                            Division = e.Lines[0].TrackingCode
                        }
                    }
                });
            }
            return list;
        }
    }
}
