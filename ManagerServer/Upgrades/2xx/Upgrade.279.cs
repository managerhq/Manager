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
        private static async Task<IEnumerable<Model.Object>> Upgrade279(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete67.BankRule>())
            {
                if (e.Lines == null && e.Obsolete_Lines != null)
                {
                    var lines = new List<ManagerServer.Model.Obsolete.Obsolete67.BankRule.Line>();
                    foreach (var e2 in e.Obsolete_Lines)
                    {
                        Guid? customer = null;
                        Guid? supplier = null;
                        Guid? employee = null;
                        Guid? capitalAccount = null;
                        Guid? specialAccount = null;
                        Guid? fixedAsset = null;
                        Guid? intangibleAsset = null;
                        Guid? expenseClaimPayer = null;

                        Guid? account = e2.Account;
                        if (account.HasValue)
                        {
                            var account2 = objects.SingleOrDefault(account.Value);
                            if (account2 != null)
                            {
                                if (account2 is Customer)
                                {
                                    customer = account;
                                    account = ManagerServer.Model.Object.GetGuidByType(typeof(BalanceSheetAccountsReceivableAccount));
                                }
                                else if (account2 is Supplier)
                                {
                                    supplier = account;
                                    account = ManagerServer.Model.Object.GetGuidByType(typeof(BalanceSheetAccountsPayableAccount));
                                }
                                else if (account2 is Employee)
                                {
                                    employee = account;
                                    account = ManagerServer.Model.Object.GetGuidByType(typeof(BalanceSheetEmployeeClearingAccount));
                                }
                                else if (account2 is CapitalAccount)
                                {
                                    capitalAccount = account;
                                    account = ManagerServer.Model.Object.GetGuidByType(typeof(BalanceSheetCapitalAccountsAccount));
                                }
                                else if (account2 is SpecialAccount)
                                {
                                    specialAccount = account;
                                    account = ManagerServer.Model.Object.GetGuidByType(typeof(BalanceSheetSpecialAccountsAccount));
                                }
                                else if (account2 is FixedAsset)
                                {
                                    fixedAsset = account;
                                    account = ManagerServer.Model.Object.GetGuidByType(typeof(BalanceSheetFixedAssetsAtCostAccount));
                                }
                                else if (account2 is IntangibleAsset)
                                {
                                    intangibleAsset = account;
                                    account = ManagerServer.Model.Object.GetGuidByType(typeof(BalanceSheetIntangibleAssetsAtCostAccount));
                                }
                                else if (account2 is ExpenseClaimsPayer)
                                {
                                    expenseClaimPayer = account;
                                    account = ManagerServer.Model.Object.GetGuidByType(typeof(BalanceSheetExpenseClaimsAccount));
                                }
                            }
                        }

                        lines.Add(new ManagerServer.Model.Obsolete.Obsolete67.BankRule.Line()
                        {
                            Account = account,
                            BillableExpenseCustomer = e2.BillableExpenseCustomer,
                            AccountsReceivableCustomer = customer,
                            AccountsPayableSupplier = supplier,
                            Employee = employee,
                            CapitalAccount = capitalAccount,
                            SpecialAccount = specialAccount,
                            FixedAsset = fixedAsset,
                            IntangibleAsset = intangibleAsset,
                            ExpenseClaimPayer = expenseClaimPayer,
                            TaxCode = e2.TaxCode,
                            TrackingCode = e2.TrackingCode,
                            SubAccount = e2.MemberAccount,
                        });
                    }
                    e.Lines = lines.ToArray();

                    list.Add(e);
                }
            }

            return list;
        }
    }
}
