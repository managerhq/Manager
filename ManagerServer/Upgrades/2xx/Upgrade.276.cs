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
        private static async Task<IEnumerable<Model.Object>> Upgrade276(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.RecurringSalesInvoice>())
            {
                if (e.Lines == null && e.Obsolete_Lines != null)
                {
                    var lines = new List<SalesInvoice.Line>();
                    foreach (var e2 in e.Obsolete_Lines)
                    {
                        Guid? capitalAccount = null;
                        Guid? specialAccount = null;
                        Guid? fixedAsset = null;
                        Guid? intangibleAsset = null;

                        Guid? account = e2.Account;
                        if (account.HasValue)
                        {
                            var account2 = objects.SingleOrDefault(account.Value);
                            if (account2 != null)
                            {
                                if (account2 is CapitalAccount)
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
                            }
                        }

                        lines.Add(new SalesInvoice.Line()
                        {
                            Item = e2.Item,
                            Account = account,
                            CapitalAccount = capitalAccount,
                            SpecialAccount = specialAccount,
                            FixedAsset = fixedAsset,
                            IntangibleAsset = intangibleAsset,
                            LineDescription = e2.Description,
                            TaxCode = e2.TaxCode,
                            Division = e2.TrackingCode,
                            CurrencyAmount = e2.ProposedAccountAmount ?? 0m,
                            Qty = e2.Qty,
                            SubAccount = e2.MemberAccount,
                            SalesUnitPrice = e2.Amount ?? 0m,
                            CustomFields = e2.CustomFields,
                            DiscountAmount = e2.DiscountAmount ?? 0m,
                            DiscountPercentage = e2.Discount ?? 0m
                        });
                    }
                    e.Lines = lines.ToArray();

                    e.HasLineDescription = e.Lines.Any(x => !string.IsNullOrWhiteSpace(x.LineDescription));
                    list.Add(e);
                }
            }

            return list;
        }
    }
}
