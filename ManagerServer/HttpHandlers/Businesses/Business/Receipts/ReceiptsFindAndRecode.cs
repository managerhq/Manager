using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using HttpFramework;
using ManagerServer.Query;
using System.Threading.Tasks;
using ProtoBuf;
using ManagerServer.Model;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Receipts
{
    [ProtoContract]
    [Title(nameof(Strings.FindAndRecode))]
    [Guide("The **Find & Recode** screen helps you locate and update receipt transactions in bulk. This tool is designed to save time when you need to change the account allocation or tax code for multiple receipts at once.")]
    [Guide("Use the search feature to filter receipts by date, bank account, description, account, or tax code. Once you find the receipts you need to update, you can select multiple transactions and apply changes to all of them simultaneously.")]
    [Header("How to Use Find & Recode")]
    [Guide("Enter search terms in the search box to filter receipts. The search matches against dates, bank accounts, descriptions, accounts, and tax codes. You can use multiple keywords separated by spaces to narrow down your results.")]
    [Guide("Select receipts by checking the boxes next to each transaction, or use the checkbox in the header to select all visible receipts. Then choose the new *account* and/or *tax code* from the dropdown menus at the bottom of the screen.")]
    [Guide("Click the **Bulk Update** button to apply your changes. Only the fields you select will be updated - leave a dropdown empty if you don't want to change that field.")]
    [Header("Important Notes")]
    [Guide("This tool only displays receipt lines that are allocated to accounts (not inventory items or other non-account allocations). It also excludes system accounts like *Accounts Payable*, *Accounts Receivable*, and *Retained Earnings*.")]
    [Guide("Changes made through Find & Recode are immediate and affect your accounting records. The original receipt transactions are updated, maintaining their dates and other details while only changing the specified fields.")]
    internal sealed class ReceiptsFindAndRecode : BusinessTemplate
    {
        [ProtoMember(1)] public Guid? BankAccount;
        [ProtoMember(2)] public int Skip;
        [ProtoMember(3)] public int? Take;
        [ProtoMember(4)] public string Term;

        protected override void InnerGet2()
        {
            var generalLedgerAccounts = ManagerServer.Query.GeneralLedgerAccounts.GetAccounts(Business).ToDictionary(x => x.Key);
            var taxCodes = ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.TaxCode>().ToDictionary(x => x.Key, x => x.Name);
            var bankAccounts = ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.BankOrCashAccount>().ToDictionary(x => x.Key, x => x.Name);
            var userPermissions = this.GetCurrentUserPermissions(Business).GetBankCashAccounts();
            if (userPermissions.Length > 0)
            {
                bankAccounts = ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.BankOrCashAccount>().Where(x => userPermissions.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Name);
            }
            var currencies = ManagerServer.Query.Currencies.GetCurrencyProvider(Business);

            var accounts = new Dictionary<Guid, string>();

            foreach (var e in generalLedgerAccounts.Values)
            {
                accounts.Add(e.Key, e.Name);
            }

            var rows = new List<Row>();
            foreach (var e in ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.Receipt>())
            {
                if (!e.ReceivedIn.HasValue) continue;
                if (!bankAccounts.ContainsKey(e.ReceivedIn.Value)) continue;
                if (e.Lines == null) continue;
                for (int i = 0; i < e.Lines.Length; i++)
                {
                    if (e.Lines[i] == null) continue;
                    if (e.Lines[i].Item.HasValue) continue;
                    var account = Strings.Suspense;
                    if (e.Lines[i].Account.HasValue)
                    {
                        if (accounts.ContainsKey(e.Lines[i].Account.Value))
                        {
                            account = accounts[e.Lines[i].Account.Value];
                        }
                        else
                        {
                            account = e.Lines[i].Account.Value.ToString();
                        }
                    }
                    var tax = "-";
                    if (e.Lines[i].TaxCode.HasValue && taxCodes.ContainsKey(e.Lines[i].TaxCode.Value)) tax = taxCodes[e.Lines[i].TaxCode.Value];
                    rows.Add(new Row() { Key = e.Key, Index = i, BankAccount = bankAccounts[e.ReceivedIn.Value], Account = account, Tax = tax, Amount = e.Lines[i].Amount, Date = e.Date, Description = e.Lines[i].LineDescription.IfEmptyReplaceWith(e.Description) });
                }
            }
            rows = rows.OrderByDescending(x => x.Date).ToList();
            var total = rows.Count;
            if (!string.IsNullOrWhiteSpace(Term))
            {
                var keywords = Term.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                rows = rows.Where(x => ContainsKeywords((x.Date.ToLocalShortDisplayString() + " " + x.BankAccount + " " + x.Description + " " + x.Account + " " + x.Tax), keywords)).ToList();
            }

            if (!Take.HasValue) Take = 50;

            var selection = rows.Skip(Skip).Take(Take.Value).ToArray();

            Script("resources/jquery/jquery-1-8-2-min.js?version=" + typeof(Template).Assembly.GetName().Version.ToString());
            Script("resources/select2/select2.js?version=" + typeof(Template).Assembly.GetName().Version.ToString());
            using (Script())
            {
                Write(@"$(function() {
$('select[name=Account]').select2({ allowClear: true, placeholder: " + Strings.DoNotRecode.EncodeJsString() + @", width: '250px' });
$('select[name=TaxCode]').select2({ allowClear: true, placeholder: " + Strings.DoNotRecode.EncodeJsString() + @", width: '150px' });
$('#checkAll').click(function(){ $('input.bulk').not(this).prop('checked', this.checked); });
                });");
            }

            using (Div(@class: "card"))
            {
                using (Div(@class: "card-header flex justify-between items-center"))
                {
                    using (Span(@class: "card-title")) Write(Strings.FindAndRecode);
                    using (Form(action: this.ToUrl(), method: "POST", @class: "form-inline"))
                    {
                        using (Div(@class: "flex items-center gap-2"))
                        {
                            InputText(name: "Term", @class: "form-control", placeholder: Strings.Search, value: Term);
                            using (Button(@class: "btn")) Write(Strings.Search);
                        }
                    }
                }
                var hidden = total - rows.Count;
                HiddenRows(Term, hidden, new ReceiptsFindAndRecode() { BankAccount = BankAccount, Business = Business }.ToUrl());
                if (selection.Length == 0)
                {
                    using (Div(@class: "card-inset text-center p-24"))
                    {
                        using (Span(@class: "card-title")) Write(Strings.Empty);
                    }
                }
                else
                {
                    using (Form(action: this.ToUrl(), method: "POST"))
                    {
                        var index = 0;
                        using (Table(@class: "card-table"))
                        {
                            using (THead())
                            {
                                using (Tr())
                                {
                                    using (Th(style: "text-align: center")) InputCheckbox(id: "checkAll", @class: "form-check-input");
                                    using (Th(style: "text-align: center")) I(@class: "fas fa-edit", style: "font-size: 16px; opacity: 0.25");
                                    using (Th(style: "text-align: center")) Write(Strings.Date);
                                    if (!BankAccount.HasValue) using (Th()) Write(Strings.BankAccount);
                                    using (Th()) Write(Strings.Account);
                                    if (taxCodes.Any()) using (Th(style: "text-align: center")) Write(Strings.Tax);
                                    using (Th()) Write(Strings.Description);
                                    using (Th(style: "text-align: right")) Write(Strings.Amount);
                                }
                            }
                            var referrer = this.ToUrl();

                            foreach (var e in selection)
                            {
                                using (Tr())
                                {
                                    using (Td(style: "width: 1px"))
                                    {
                                        InputCheckbox(name: "Items[" + index + "].Key", @class: "form-check-input bulk", value: e.Key.ToString());
                                        InputHidden(name: "Items[" + index + "].Index", value: e.Index.ToString());
                                        index++;
                                    }
                                    using (Td(style: "width: 1px"))
                                    {
                                        using (A(href: new ReceiptForm() { Business = Business, Key = e.Key, Referrer = referrer }.ToUrl(), @class: "btn btn-sm")) Write(Strings.Edit);
                                    }
                                    using (Td(@class: "content", style: "text-align: center; width: 1px; white-space: nowrap")) Write(e.Date.ToLocalShortDisplayString());
                                    if (!BankAccount.HasValue) using (Td(@class: "content")) Write(e.BankAccount);
                                    using (Td(@class: "content")) Write((e.Account ?? "").Replace("\n", "<br />"));
                                    if (taxCodes.Any()) using (Td(@class: "content", style: "text-align: center")) Write((e.Tax ?? "").Replace("\n", "<br />"));
                                    using (Td(@class: "content")) Write((e.Description ?? "").Replace("\n", "<br />"));

                                    using (Td(@class: "content", style: "font-weight: bold; width: 100px; white-space: nowrap; text-align: right"))
                                    {
                                        using (Span(style: (e.Amount < 0m ? "color: red" : null))) Write(e.Amount.ToCurrencyString(currencies.Get(null), CurrencySymbol.Long));
                                    }
                                }
                            }
                        }
                        using (Div(@class: "card-header"))
                        {
                            using (Table()) using (Tr())
                            {
                                using (Td(style: "vertical-align: bottom"))
                                {
                                    I(@class: "fas fa-fw fa-turn-up fa-rotate-90", style: "font-size: 32px; color: #ccc");
                                }
                                using (Td(style: "padding-left: 5px"))
                                {
                                    using (Label(style: "margin-bottom: 2px")) Write(Strings.Account);
                                    using (Div()) using (Select(name: "Account"))
                                    {
                                        Option();
                                        using (OptGroup(label: Strings.ProfitAndLossStatement))
                                        {
                                            foreach (var e in ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.ProfitAndLossStatementAccount>().OrderBy(x => x.Name))
                                            {
                                                Option(value: e.Key.ToString(), text: e.Name);
                                            }
                                        }
                                        using (OptGroup(label: Strings.BalanceSheet))
                                        {
                                            foreach (var e in ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.BalanceSheetAccount>().OrderBy(x => x.Name))
                                            {
                                                if (e.Key == ManagerServer.Model.Master.AccountKeys.AccountsPayable) continue;
                                                if (e.Key == ManagerServer.Model.Master.AccountKeys.AccountsReceivable) continue;
                                                if (e.Key == ManagerServer.Model.Master.AccountKeys.CashAtBank) continue;
                                                if (e.Key == ManagerServer.Model.Master.AccountKeys.RetainedEarnings) continue;
                                                Option(value: e.Key.ToString(), text: e.Name);
                                            }
                                            Option(value: ManagerServer.Model.Master.AccountKeys.Suspense.ToString(), text: Strings.Suspense);
                                        }
                                    }
                                }
                                if (taxCodes.Any())
                                {
                                    using (Td(style: "padding-left: 5px"))
                                    {
                                        using (Label(style: "margin-bottom: 2px")) Write(Strings.Tax);
                                        using (Div()) using (Select(name: "TaxCode"))
                                        {
                                            Option();
                                            Option(value: Guid.Empty.ToString(), text: Strings.NoTax);
                                            foreach (var e2 in taxCodes.OrderBy(x => x.Value))
                                            {
                                                Option(value: e2.Key.ToString(), text: e2.Value);
                                            }
                                        }
                                    }
                                }
                                using (Td(style: "padding-left: 10px; vertical-align: bottom"))
                                {
                                    InputHidden(name: "Items", value: index.ToString());
                                    InputSubmit(value: Strings.BulkUpdate, @class: "btn btn-success", style: "font-weight: bold");
                                }
                            }
                        }
                        using (Div(@class: "card-header"))
                        {
                            this.Pagination(rows.Count);
                        }
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(Term))
            {
                Script(src: "resources/mark/mark-min.js");
                using (Script())
                {
                    Write(@"var instance = new Mark(""td.content"");");
                    Write("instance.mark(" + Term.EncodeJsString() + @");");
                }
            }
        }

        private void HiddenRows(string term, int hidden, string undoUrl)
        {
            if (!string.IsNullOrWhiteSpace(term) && hidden > 0)
            {
                using (Div(@class: "card-header"))
                {
                    Write(string.Format(Strings.HiddenRowsCount, "<b>" + hidden + @"</b>", "<b>" + term + @"</b>"));
                    Write(".&nbsp;");
                    using (A(href: undoUrl, style: "font-weight: bold")) Write(Strings.Undo);
                }
            }
        }

        private void Pagination(int total)
        {
            var take = Take ?? 50;

            var handler = (ReceiptsFindAndRecode)this.MemberwiseClone();

            if (total > take || Skip > 0)
            {
                var totalPages = Math.DivRem(total, take, out int lastPageCount);
                if (lastPageCount > 0) totalPages++;
                if (lastPageCount == 0) lastPageCount = take;
                int currentPage = 1;
                if (Skip > 0) currentPage = (Skip / take) + 1;

                using (Div(@class: "text-center"))
                {
                    using (Div(@class: "btn-group"))
                    {
                        handler.Skip = 0;
                        using (A(href: handler.ToUrl(), @class: "btn btn-sm" + (currentPage <= 1 ? " disabled" : ""), style: "min-width: 60px"))
                        {
                            I(@class: "fas fa-step-backward");
                        }
                        handler.Skip = (currentPage - 2) * take;
                        using (A(href: handler.ToUrl(), @class: "btn btn-sm" + (currentPage <= 1 ? " disabled" : ""), style: "min-width: 60px"))
                        {
                            I(@class: "fas fa-backward");
                        }
                    }
                    using (Span(style: "font-size: 12px; color: #ccc; font-weight: bold; margin-left: 10px; margin-right: 10px")) Write(string.Format(Strings.Page_XXX_of_XXX, currentPage.ToString(), totalPages.ToString()));
                    using (Div(@class: "btn-group"))
                    {
                        handler.Skip = currentPage * take;
                        using (A(href: handler.ToUrl(), @class: "btn btn-sm" + (currentPage >= totalPages ? " disabled" : ""), style: "min-width: 60px"))
                        {
                            I(@class: "fas fa-forward");
                        }
                        handler.Skip = total - lastPageCount;
                        using (A(href: handler.ToUrl(), @class: "btn btn-sm" + (currentPage >= totalPages ? " disabled" : ""), style: "min-width: 60px"))
                        {
                            I(@class: "fas fa-step-forward");
                        }
                    }
                }
            }
        }

        private bool ContainsKeywords(string value, string[] keywords)
        {
            if (value == null) return false;
            foreach (var e in keywords)
            {
                if (value.IndexOf(e, StringComparison.OrdinalIgnoreCase) == -1) return false;
            }
            return true;
        }

        public sealed class FormData
        {
            public Guid? Account;
            public Guid? TaxCode;
            public Item[] Items;

            public sealed class Item
            {
                public Guid? Key;
                public int Index;
            }
        }

        public sealed class Row
        {
            public Guid Key;
            public int Index;
            public DateTime? Date;
            public string BankAccount;
            public string Account;
            public string Tax;
            public decimal Amount;
            public string Description;
        }

        protected override async Task InnerPost()
        {
            var suspense = ApplicationData.Businesses.Get(Business).Single<BalanceSheetSuspenseAccount>();

            var form = await Request.ReadFormAsync();

            if (form.ContainsKey("Term"))
            {
                Term = form["Term"].ToString();
                Skip = 0;
                Response.Redirect(this.ToUrl());
                return;
            }

            var formData = form.Parse<FormData>();

            var bankTransactions = ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.Receipt>().ToDictionary(x => x.Key);

            var list = new List<ManagerServer.Model.Receipt>();

            foreach (var e in formData.Items)
            {
                if (!e.Key.HasValue) continue;

                if (bankTransactions.ContainsKey(e.Key.Value))
                {
                    var payment = bankTransactions[e.Key.Value];
                    if (formData.Account.HasValue)
                    {
                        payment.Lines[e.Index].Item = null;
                        if (formData.Account == suspense.Key)
                        {
                            payment.Lines[e.Index].Account = null;
                        }
                        else
                        {
                            payment.Lines[e.Index].Account = formData.Account;
                        }
                    }
                    if (formData.TaxCode.HasValue)
                    {
                        if (formData.TaxCode.Value == Guid.Empty) payment.Lines[e.Index].TaxCode = null;
                        else payment.Lines[e.Index].TaxCode = formData.TaxCode.Value;
                    }

                    list.Add(payment);
                }
            }

            if (list.Any())
            {
                ApplicationData.Businesses.Process(Business, list.Distinct().ToArray(), GetUserName());
            }

            Response.Redirect(this.ToUrl());
        }
    }
}