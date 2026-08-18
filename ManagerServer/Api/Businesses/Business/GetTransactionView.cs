using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.HttpHandlers.Businesses.Business;
using ManagerServer.Model;
using ManagerServer.Model.Obsolete.Obsolete86;
using Markdig;
using System.Collections.Generic;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business
{
    internal abstract class GetTransactionView<T> : ViewEndpoint<TransactionView>, IView where T : Model.Object, new()
    {
        protected Database Database { get; private set; }

        public sealed override TransactionView AuthorizedHandle()
        {
            Languages.SetLanguage(Language);

            Database = GetApplicationData().Businesses.Get(Business);

            var o = Database.SingleOrDefault<T>(Key);
            if (o == null)
            {
                o = Database.Single<T>();
                if (o == null || o.Key != Key) return null;
            }

            var viewData = GetViewData(o);
            if (viewData == null) return null;

            viewData.timestamp = o.Timestamp;
            viewData.key = o.Key;
            viewData.type = o.GetType().Name.ToLowerInvariant();
            viewData.direction = Languages.IsRightToLeft() ? "rtl" : "ltr";

            var businessDetails = Database.Single<Model.BusinessDetails>();
            viewData.business.name = businessDetails.Name;
            if (string.IsNullOrWhiteSpace(viewData.business.name)) viewData.business.name = Business;
            viewData.business.address = businessDetails.Address;
            viewData.business.custom_fields.AddRange(GetCustomFields(typeof(Model.BusinessDetails), businessDetails.CustomFields));
            viewData.business.custom_fields.AddRange(GetCustomFields2(typeof(Model.BusinessDetails), businessDetails.CustomFields2));

            var image = Database.GetImage(Database.Single<BusinessDetails>().Key);
            if (image.HasValue)
            {
                viewData.business.logo = new GetBusinessLogo() { Business = Business, Timestamp = image.Value }.ToUrl();
            }

            var customFieldsField = o.GetType().GetFieldOrProperty("CustomFields");
            if (customFieldsField != null)
            {
                viewData.custom_fields.AddRange(GetCustomFields(o.GetType(), customFieldsField.GetMemberValue(o) as Dictionary<Guid, string>));
            }

            var customFieldsField2 = o.GetType().GetFieldOrProperty("CustomFields2");
            if (customFieldsField2 != null)
            {
                viewData.custom_fields.AddRange(GetCustomFields2(o.GetType(), customFieldsField2.GetMemberValue(o) as Model.CustomFields));
            }

            if (o is Model.IHasCustomTheme customTheme)
            {
                viewData.custom_theme = customTheme.GetCustomTheme();
            }

            var hasFooters = o.GetType().GetFieldOrProperty("Has" + o.GetType().Name + "Footers")?.GetMemberValue(o) as bool?;
            if (hasFooters == true)
            {
                var footerKeys = o.GetType().GetFieldOrProperty(o.GetType().Name + "Footers")?.GetMemberValue(o) as Guid[];
                if (footerKeys != null && footerKeys.Length > 0)
                {
                    var pipeline = new Markdig.MarkdownPipelineBuilder().UseSoftlineBreakAsHardlineBreak().Build();

                    var footers = new List<string>();
                    foreach (var e in footerKeys)
                    {
                        var footer = Database.SingleOrDefault<ManagerServer.Model.NamedObject>(e);
                        if (footer != null)
                        {
                            var content = footer.GetType().GetFieldOrProperty("Content").GetMemberValue(footer) as string;
                            if (!string.IsNullOrWhiteSpace(content))
                            {
                                foreach (var e2 in this.GetType().Assembly.GetTypes().Where(x => x.BaseType == typeof(NakedObjectsWithAutomaticRows<T>)).Select(x => Activator.CreateInstance(x)).OfType<NakedObjectsWithAutomaticRows<T>>().ToArray())
                                {
                                    e2.HttpContext = Context;
                                    e2.Business = Business;

                                    var columns = new List<NakedObjects.Column>();
                                    columns.AddRange(e2.GetColumns());
                                    columns.AddRange(e2.GetCustomFieldColumns());

                                    foreach (var e3 in columns)
                                    {
                                        if (!e3.CanConvertToPlainText) continue;
                                        if (string.IsNullOrWhiteSpace(e3.MergeTag)) continue;
                                        if (content.Contains(e3.MergeTag))
                                        {
                                            e3.EnsureCells(new[] { o });
                                            var text = e3.GetValueAsPlainText(o);

                                            if (e3.GetValue(o) is NakedObjects.QrCode)
                                            {
                                                text = e3.GetValueAsHtml(o);
                                            }

                                            content = content.Replace(e3.MergeTag, text);
                                        }
                                    }
                                }

                                content = Markdig.Markdown.ToHtml(content, pipeline);

                                if (content.Contains("document.onreadystatechange = function () {"))
                                {
                                    content = content.Replace("document.onreadystatechange = function () {", "function initFooterFunction() {");
                                    content += "<script>initFooterFunction();</script>";
                                }

                                footers.Add(content);
                            }
                        }
                    }

                    var list = new List<string>();
                    if (viewData.footers != null) list.AddRange(viewData.footers);
                    list.AddRange(footers.ToArray());

                    viewData.footers = list.ToArray();
                }
            }

            return viewData;
        }

        protected abstract TransactionView GetViewData(T o);

        protected List<TransactionView.CustomField> GetCustomFields(Type type, Dictionary<Guid, string> customFields)
        {
            var output = new List<TransactionView.CustomField>();
            if (customFields == null || customFields.Count == 0) return output;

            var defs = Database.OfType<Model.CustomField>().Where(x => x.Contains(type) && x.DisplayOnView).ToList();
            defs.AddRange(Localizations.Localizations.Get(Database.Single<Model.BusinessDetails>().Obsolete_Country).OfType<Model.CustomField>().Where(x => x.Contains(type) && x.DisplayOnView));

            foreach (var e in defs.OrderBy(x => x.Position))
            {
                if (!customFields.TryGetValue(e.Key, out var text)) continue;
                if (string.IsNullOrWhiteSpace(text)) continue;

                if (e.Type == Model.Enums.CustomFieldStyle.Date)
                {
                    var parts = text.Split('-');
                    if (parts.Length == 3
                        && int.TryParse(parts[0], out var year)
                        && int.TryParse(parts[1], out var month)
                        && int.TryParse(parts[2], out var day)
                        && year > 0 && month > 0 && day > 0
                        && year < 10000 && month <= 12 && day <= 31)
                    {
                        var date = new DateTime(year, month, day);
                        output.Add(new TransactionView.CustomField { key = e.Key.ToString(), label = e.Name, text = date.ToLocalShortDisplayString(), value = date, displayAtTheTop = e.ShowAtTheTop });
                    }
                }
                else if (e.Type == Model.Enums.CustomFieldStyle.Number)
                {
                    if (decimal.TryParse(text, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out var number))
                    {
                        output.Add(new TransactionView.CustomField { key = e.Key.ToString(), label = e.Name, text = number.ToNumberString(), value = number, displayAtTheTop = e.ShowAtTheTop });
                    }
                }
                else
                {
                    output.Add(new TransactionView.CustomField { key = e.Key.ToString(), label = e.Name, text = text, value = text, displayAtTheTop = e.ShowAtTheTop });
                }
            }
            return output;
        }

        protected List<TransactionView.CustomField> GetCustomFields2(Type type, Model.CustomFields customFields)
        {
            var output = new List<TransactionView.CustomField>();
            if (customFields == null) return output;

            var defs = Database.GetCustomFields(type).Where(x => x.DisplayOnView).ToList();

            foreach (var e in defs.OrderBy(x => x.Position))
            {
                var value = customFields.GetValue(e);
                if (value is DateTime date)
                {
                    output.Add(new TransactionView.CustomField { key = e.Key.ToString(), label = e.Name, text = date.ToLocalShortDisplayString(), value = date, displayAtTheTop = e.ShowAtTheTop });
                }
                else if (value is decimal number)
                {
                    output.Add(new TransactionView.CustomField { key = e.Key.ToString(), label = e.Name, text = number.ToNumberString(), value = number, displayAtTheTop = e.ShowAtTheTop });
                }
                else if (value is string text)
                {
                    output.Add(new TransactionView.CustomField { key = e.Key.ToString(), label = e.Name, text = text, value = text, displayAtTheTop = e.ShowAtTheTop });
                }
                else if (value is bool b && b)
                {
                    output.Add(new TransactionView.CustomField { key = e.Key.ToString(), label = e.Name, text = Strings.Yes, value = b, displayAtTheTop = e.ShowAtTheTop });
                }
                else if (value is string[] strings && strings.Length > 0)
                {
                    output.Add(new TransactionView.CustomField { key = e.Key.ToString(), label = e.Name, text = string.Join(", ", strings.Where(x => !string.IsNullOrWhiteSpace(x))), value = strings, displayAtTheTop = e.ShowAtTheTop });
                }
                if (e is ImageCustomField imageCustomField && value is Guid guid)
                {
                    output.Add(new TransactionView.CustomField
                    {
                        key = e.Key.ToString(),
                        label = e.Name,
                        value = guid,
                        image = new TransactionView.Image
                        {
                            url = new Image() { Business = Business }.ToUrl() + "&key=" + guid.ToString(),
                            width = imageCustomField.GetWidth(),
                            height = imageCustomField.GetHeight(),
                        },
                    });
                }
            }
            return output;
        }

        protected static string GetBilingualString(bool bilingual, string name, string englishText)
        {
            var output = Strings.GetPropertyValue(name);
            if (bilingual && output != englishText) output += '\n' + englishText;
            return output;
        }

        protected TransactionView.Table BuildTable(
            Model.Transaction transaction,
            bool showTaxAmountOnLineItems = true,
            bool showTaxCodeOnLineItems = true,
            bool bilingual = false,
            bool showLineNumbers = false,
            bool forceTotals = false,
            bool showItemImages = false)
        {
            var amountsIncludeTax = false;
            if (transaction is Model.Receipt receipt) amountsIncludeTax = !receipt.AmountsAreTaxExclusive;
            if (transaction is Model.Payment payment) amountsIncludeTax = !payment.AmountsAreTaxExclusive;
            if (transaction is Model.PurchaseQuote purchaseQuote) amountsIncludeTax = purchaseQuote.AmountsIncludeTax;
            if (transaction is Model.PurchaseOrder purchaseOrder) amountsIncludeTax = purchaseOrder.AmountsIncludeTax;
            if (transaction is Model.PurchaseInvoice purchaseInvoice) amountsIncludeTax = purchaseInvoice.AmountsIncludeTax;
            if (transaction is Model.DebitNote debitNote) amountsIncludeTax = debitNote.AmountsIncludeTax;
            if (transaction is Model.SalesQuote salesQuote) amountsIncludeTax = salesQuote.AmountsIncludeTax;
            if (transaction is Model.SalesOrder salesOrder) amountsIncludeTax = salesOrder.AmountsIncludeTax;
            if (transaction is Model.SalesInvoice salesInvoice) amountsIncludeTax = salesInvoice.AmountsIncludeTax;
            if (transaction is Model.CreditNote creditNote) amountsIncludeTax = creditNote.AmountsIncludeTax;
            if (transaction is Model.ExpenseClaim expenseClaim) amountsIncludeTax = expenseClaim.AmountsIncludeTax;
            if (transaction is Model.JournalEntry) amountsIncludeTax = true;

            var reverseSign = false;
            if (transaction is Model.Receipt) reverseSign = true;
            if (transaction is Model.SalesOrder) reverseSign = true;
            if (transaction is Model.SalesInvoice) reverseSign = true;
            if (transaction is Model.SalesQuote) reverseSign = true;
            if (transaction is Model.DebitNote) reverseSign = true;
            if (transaction is Model.DepreciationEntry) reverseSign = true;
            if (transaction is Model.AmortizationEntry) reverseSign = true;

            var o = new TransactionView.Table();

            var generalLedgerTransactions = transaction.GetGeneralLedgerTransactions(Database);
            var transactionLines = generalLedgerTransactions.Where(x => x.TransactionLine != null && !x.IsCostOfGoodsSold).ToArray();

            if (!transactionLines.Any()) return o;

            var currency = transactionLines.Where(x => !x.IsCostOfGoodsSold).First().TransactionCurrency;

            var lineType = transactionLines.Where(x => x.TransactionLine.GetType2() != null).Select(x => x.TransactionLine.GetType2()).Distinct().SingleOrDefault();

            var customFields = Database.OfType<Model.CustomField>().Where(x => x.Contains(typeof(Model.InventoryItem)) || x.Contains(typeof(Model.InventoryKit)) || x.Contains(typeof(Model.NonInventoryItem)) || x.Contains(lineType)).Where(x => x.DisplayOnView).OrderBy(x => x.Position).ToArray();
            var customFieldNames = customFields.Select(x => x.Name).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct();
            var taxComponents = transactionLines.Select(x => x.TaxComponent).Distinct().Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

            var qtyTotal = false;
            var qtyLabel = GetBilingualString(bilingual, nameof(Strings.Qty), "Qty");
            var showUnitNames = true;
            var unitNames = transactionLines.Where(x => x.Qty.HasValue).Select(x => x.Item?.GetUnitName()).Distinct().ToArray();
            if (unitNames.Length == 1 && !string.IsNullOrWhiteSpace(unitNames[0]))
            {
                qtyTotal = true;
                showUnitNames = false;
                qtyLabel = unitNames[0];
            }

            if (!transactionLines.Any(x => x.IsTaxTransaction)) amountsIncludeTax = true;

            if (!showTaxAmountOnLineItems)
            {
                var taxCodes = transactionLines.Where(x => x.TransactionAmount != 0m).Select(x => x.TaxCode).Distinct().ToArray();
                if (taxCodes.Length == 1) showTaxCodeOnLineItems = false;
            }

            var customFields2 = Database.GetCustomFields(lineType).Where(x => x.DisplayOnView).OrderBy(x => x.Position).ToArray();

            /* Columns */
            o.columns.Add(new TransactionView.Column { label = "#", nowrap = true, align = "center", minWidth = true });
            o.columns.Add(new TransactionView.Column { label = GetBilingualString(bilingual, nameof(Strings.Item), "Item") });
            o.columns.Add(new TransactionView.Column { label = GetBilingualString(bilingual, nameof(Strings.Account), "Account") });
            o.columns.Add(new TransactionView.Column { label = GetBilingualString(bilingual, nameof(Strings.Description), "Description") });
            foreach (var e in customFieldNames) o.columns.Add(new TransactionView.Column { label = e });
            foreach (var e in customFields2)
            {
                string align = null;
                var nowrap = false;
                var total = false;
                if (e is Model.NumberCustomField) { align = "right"; nowrap = true; }
                if (e is Model.DateCustomField) { align = "center"; nowrap = true; }
                if (e is Model.CheckboxCustomField) { align = "center"; nowrap = true; }
                if (e is Model.NumberCustomField numberCustomField && !numberCustomField.HideTotalAmount) total = true;
                o.columns.Add(new TransactionView.Column { label = e.Name, align = align, nowrap = nowrap, total = total });
            }
            o.columns.Add(new TransactionView.Column { label = qtyLabel, align = "center", nowrap = true, total = qtyTotal });
            o.columns.Add(new TransactionView.Column { label = GetBilingualString(bilingual, nameof(Strings.UnitPrice), "Unit price"), align = "right", nowrap = true });
            o.columns.Add(new TransactionView.Column { label = GetBilingualString(bilingual, nameof(Strings.Price), "Price"), align = "right", nowrap = true, total = true });
            o.columns.Add(new TransactionView.Column { label = GetBilingualString(bilingual, nameof(Strings.Discount), "Discount"), align = "right", nowrap = true });
            o.columns.Add(new TransactionView.Column { label = GetBilingualString(bilingual, nameof(Strings.FreightIn), "Freight-in"), align = "right", nowrap = true, total = true });
            o.columns.Add(new TransactionView.Column { label = GetBilingualString(bilingual, nameof(Strings.Amount), "Amount"), align = "right", nowrap = true, total = true });
            o.columns.Add(new TransactionView.Column { label = GetBilingualString(bilingual, nameof(Strings.Tax), "Tax"), align = "center", nowrap = true });
            o.columns.Add(new TransactionView.Column { label = GetBilingualString(bilingual, nameof(Strings.TaxAmount), "Tax Amount"), align = "right", nowrap = true, total = true });
            o.columns.Add(new TransactionView.Column { label = GetBilingualString(bilingual, nameof(Strings.Total), "Total"), align = "right", nowrap = true, alwaysShow = forceTotals });

            if (transaction is Model.JournalEntry)
            {
                o.columns.Add(new TransactionView.Column { label = GetBilingualString(bilingual, nameof(Strings.Debit), "Debit"), align = "right", nowrap = true, alwaysShow = true, total = true });
                o.columns.Add(new TransactionView.Column { label = GetBilingualString(bilingual, nameof(Strings.Credit), "Credit"), align = "right", nowrap = true, alwaysShow = true, total = true });
            }

            /* Rows */
            var lineNumber = 0;
            foreach (var e in transactionLines.GroupBy(x => x.TransactionLine))
            {
                var row = new TransactionView.Row();
                o.rows.Add(row);

                var inventoryItem = Database.SingleOrDefault<Model.InventoryItem>(e.Key.GetItem());
                var nonInventoryItem = Database.SingleOrDefault<Model.NonInventoryItem>(e.Key.GetItem());
                var inventoryKit = Database.SingleOrDefault<Model.InventoryKit>(e.Key.GetItem());
                var freightInItem = (e.Key.GetItem() == new Guid("3458c24f-2a5f-4dcf-9de7-7340b1463d9c") ? Database.Single<FreightInItem>() : null);

                var account = e.SingleOrDefault(x => !x.IsTaxTransaction && !x.IsCostOfGoodsSold && !x.IsBillableExpense && !x.IsLandingCost);
                var canAccountBeHidden = false;
                if (!string.IsNullOrWhiteSpace(inventoryItem?.GetDisplayName())) canAccountBeHidden = true;
                if (!string.IsNullOrWhiteSpace(nonInventoryItem?.GetDisplayName())) canAccountBeHidden = true;
                if (!string.IsNullOrWhiteSpace(inventoryKit?.GetDisplayName())) canAccountBeHidden = true;
                if (!string.IsNullOrWhiteSpace(e.Key.GetLineDescription(transaction))) canAccountBeHidden = true;
                if (account?.GeneralLedgerAccount is Model.BalanceSheetSuspenseAccount) canAccountBeHidden = true;
                if (transaction is Model.JournalEntry) canAccountBeHidden = false;

                if (showLineNumbers)
                {
                    lineNumber++;
                    row.cells.Add(new TransactionView.Cell { text = lineNumber.ToString() });
                }
                else
                {
                    row.cells.Add(new TransactionView.Cell());
                }

                var itemName = inventoryItem?.GetDisplayName() ?? inventoryKit?.GetDisplayName() ?? nonInventoryItem?.GetDisplayName();

                string itemImage = null;
                if (showItemImages && e.Key.GetItem().HasValue)
                {
                    itemImage = ApplicationData.Instance.Businesses.GetImageDataUrl(Business, e.Key.GetItem().Value);
                }

                if (!string.IsNullOrWhiteSpace(itemImage))
                {
                    row.cells.Add(new TransactionView.Cell { text = itemName, image = new TransactionView.Image { url = itemImage } });
                }
                else
                {
                    row.cells.Add(new TransactionView.Cell { text = itemName });
                }
                row.cells.Add(new TransactionView.Cell { text = account?.Account, canBeHidden = canAccountBeHidden });
                row.cells.Add(new TransactionView.Cell { text = e.Key.GetLineDescription(transaction) });
                foreach (var e2 in customFieldNames)
                {
                    var customFieldText = string.Empty;
                    if (inventoryItem?.CustomFields != null)
                    {
                        var customField = customFields.FirstOrDefault(x => x.Name == e2 && x.Contains(typeof(Model.InventoryItem)));
                        if (customField != null && inventoryItem.CustomFields.ContainsKey(customField.Key))
                        {
                            customFieldText = inventoryItem.CustomFields[customField.Key];
                        }
                    }
                    if (inventoryKit?.CustomFields != null)
                    {
                        var customField = customFields.FirstOrDefault(x => x.Name == e2 && x.Contains(typeof(Model.InventoryKit)));
                        if (customField != null && inventoryKit.CustomFields.ContainsKey(customField.Key))
                        {
                            customFieldText = inventoryKit.CustomFields[customField.Key];
                        }
                    }
                    if (nonInventoryItem?.CustomFields != null)
                    {
                        var customField = customFields.FirstOrDefault(x => x.Name == e2 && x.Contains(typeof(Model.NonInventoryItem)));
                        if (customField != null && nonInventoryItem.CustomFields.ContainsKey(customField.Key))
                        {
                            customFieldText = nonInventoryItem.CustomFields[customField.Key];
                        }
                    }
                    if (e.Key.GetCustomFields() != null)
                    {
                        var customField = customFields.FirstOrDefault(x => x.Name == e2 && x.Contains(lineType));
                        if (customField != null && e.Key.GetCustomFields().ContainsKey(customField.Key))
                        {
                            customFieldText = e.Key.GetCustomFields()[customField.Key];
                        }
                    }
                    row.cells.Add(new TransactionView.Cell { text = customFieldText });
                }

                foreach (var e2 in customFields2)
                {
                    var value = e.Key.GetCustomFields2()?.GetValue(e2);
                    string text = null;
                    if (value is decimal d) text = d.ToNumberString();
                    if (value is DateTime dateTime) text = dateTime.ToShortDateString();
                    if (value is string s) text = s;
                    if (value is bool b && b) text = Strings.Yes;
                    if (value is string[] stringArray && stringArray.Length > 0) text = string.Join(", ", stringArray.Where(x => !string.IsNullOrWhiteSpace(x)));
                    row.cells.Add(new TransactionView.Cell { text = text, value = value });
                }

                if (e.Key.GetQty(transaction).HasValue)
                {
                    var qtyText = e.Key.GetQty(transaction).ToNumberString();
                    var item = Database.SingleOrDefault<Model.NamedObject>(e.Key.GetItem()) as Model.IItem;
                    if (showUnitNames && !string.IsNullOrWhiteSpace(item?.GetUnitName())) qtyText += " " + item.GetUnitName();
                    row.cells.Add(new TransactionView.Cell { value = e.Key.GetQty(transaction), text = qtyText });
                }
                else
                {
                    row.cells.Add(new TransactionView.Cell());
                }

                decimal? unitPrice = null;
                if (e.Key.GetQty(transaction).HasValue) unitPrice = e.Key.GetUnitPrice(transaction);
                row.cells.Add(new TransactionView.Cell { value = unitPrice, text = unitPrice.ToCurrencyString(currency, CurrencySymbol.None) });

                var anyDiscount = transactionLines.Any(x => x.TransactionLine.HasDiscount(transaction));

                if (anyDiscount)
                {
                    var amount = currency.Round(e.Key.GetLineTotal(transaction));
                    if (amount != 0m || unitPrice.HasValue)
                    {
                        row.cells.Add(new TransactionView.Cell { value = amount, text = amount.ToCurrencyString(currency, CurrencySymbol.None) });
                    }
                    else
                    {
                        row.cells.Add(new TransactionView.Cell());
                    }
                }
                else
                {
                    row.cells.Add(new TransactionView.Cell());
                }

                if (e.Key.GetDiscountPercentage(transaction).HasValue)
                {
                    if (e.Key.GetDiscountPercentage(transaction).Value == 0m) row.cells.Add(new TransactionView.Cell());
                    else row.cells.Add(new TransactionView.Cell { value = e.Key.GetDiscountPercentage(transaction).Value, text = e.Key.GetDiscountPercentage(transaction).Value + " %" });
                }
                else if (e.Key.GetDiscountAmount(transaction).HasValue)
                {
                    if (e.Key.GetDiscountAmount(transaction).Value == 0m) row.cells.Add(new TransactionView.Cell());
                    else row.cells.Add(new TransactionView.Cell { value = e.Key.GetDiscountAmount(transaction).Value, text = e.Key.GetDiscountAmount(transaction).Value.ToCurrencyString(e.First().TransactionCurrency, CurrencySymbol.None) });
                }
                else
                {
                    row.cells.Add(new TransactionView.Cell());
                }

                var landingCost = e.Where(x => x.IsLandingCost).Sum(x => x.TransactionAmount);
                if (landingCost != 0m)
                {
                    row.cells.Add(new TransactionView.Cell { value = landingCost, text = landingCost.ToCurrencyString(currency, CurrencySymbol.None) });
                }
                else
                {
                    row.cells.Add(new TransactionView.Cell());
                }

                var subtotal = e.Where(x => !x.IsTaxTransaction || (x.IsTaxTransaction && string.IsNullOrWhiteSpace(x.TaxComponent))).Sum(x => x.TransactionAmount);
                if (reverseSign) subtotal *= -1m;
                if (subtotal == 0m || amountsIncludeTax || !showTaxAmountOnLineItems) row.cells.Add(new TransactionView.Cell());
                else row.cells.Add(new TransactionView.Cell { value = subtotal, text = subtotal.ToNumberString() });

                var taxCode = Database.SingleOrDefault<Model.TaxCode>(e.Key.GetTaxCode())?.GetDisplayName();
                if (!showTaxCodeOnLineItems) taxCode = null;
                if (e.All(x => x.TaxCode == null)) taxCode = null;
                row.cells.Add(new TransactionView.Cell { text = taxCode });

                var taxAmount = e.Where(x => x.IsTaxTransaction && !string.IsNullOrWhiteSpace(x.TaxComponent)).Sum(x => x.TransactionAmount);
                if (!showTaxAmountOnLineItems) taxAmount = 0m;
                if (reverseSign) taxAmount *= -1m;
                if (taxAmount == 0m) row.cells.Add(new TransactionView.Cell());
                else row.cells.Add(new TransactionView.Cell { value = taxAmount, text = taxAmount.ToNumberString() });

                var lineTotal = e.Sum(x => x.TransactionAmount);
                if (reverseSign) lineTotal *= -1m;
                if (!showTaxAmountOnLineItems && !amountsIncludeTax) lineTotal = subtotal;
                if (lineTotal == 0m && !e.Key.HasDebitCreditAmountOrUnitPrice(transaction)) row.cells.Add(new TransactionView.Cell());
                else if (transaction is Model.JournalEntry) row.cells.Add(new TransactionView.Cell());
                else row.cells.Add(new TransactionView.Cell { value = lineTotal, text = lineTotal.ToNumberString() });

                if (transaction is Model.JournalEntry)
                {
                    var debit = e.Where(x => x.TransactionAmount > 0m).Sum(x => x.TransactionAmount) as decimal?;
                    var credit = e.Where(x => x.TransactionAmount < 0m).Sum(x => x.TransactionAmount) * -1m as decimal?;
                    if (debit == 0m) debit = null;
                    if (credit == 0m) credit = null;

                    row.cells.Add(new TransactionView.Cell { value = debit, text = debit.ToNumberString() });
                    row.cells.Add(new TransactionView.Cell { value = credit, text = credit.ToNumberString() });
                }
            }

            // Remove unused columns
            foreach (var i in Enumerable.Range(0, o.columns.Count).Reverse())
            {
                if (o.columns[i].alwaysShow) continue;
                if (o.rows.All(x => (string.IsNullOrWhiteSpace((x.cells[i].text ?? string.Empty).Replace("&nbsp;", " ")) && x.cells[i].image == null) || x.cells[i].canBeHidden))
                {
                    o.columns.RemoveAt(i);
                    foreach (var e in o.rows) e.cells.RemoveAt(i);
                }
            }

            for (int i = 0; i < o.columns.Count; i++)
            {
                var column = o.columns[i];
                if (column.total)
                {
                    column.sum = o.rows.Select(x => x.cells[i].value).Where(x => x is decimal).Sum(x => (decimal)x);
                    column.sumText = column.sum.ToNumberString();
                }
            }

            // If all columns are minWidth
            if (o.columns.All(x => x.nowrap))
            {
                var insertIndex = 0;
                if (o.columns.Count > 0 && o.columns[0].label == "#") insertIndex = 1;
                o.columns.Insert(insertIndex, new TransactionView.Column());
                foreach (var e in o.rows) e.cells.Insert(insertIndex, new TransactionView.Cell());
            }

            var roundingTransaction = generalLedgerTransactions.SingleOrDefault(x => x.GeneralLedgerAccount.Key == Database.Single<Model.ProfitAndLossStatementAccountRoundingExpense>().Key);
            var withholdingTransaction = generalLedgerTransactions.SingleOrDefault(x => x.TransactionLine == null && x.GeneralLedgerAccount.Key == Database.Single<Model.BalanceSheetWithholdingTaxReceivableAccount>().Key);
            var withholdingTransaction2 = generalLedgerTransactions.SingleOrDefault(x => x.TransactionLine == null && x.GeneralLedgerAccount.Key == Database.Single<Model.BalanceSheetWithholdingTaxPayableAccount>().Key);

            if (!amountsIncludeTax)
            {
                var taxExclusiveAmount = transactionLines.Where(x => !x.IsTaxTransaction || (x.IsTaxTransaction && string.IsNullOrWhiteSpace(x.TaxComponent))).Sum(x => x.TransactionAmount);
                if (reverseSign) taxExclusiveAmount *= -1m;
                o.totals.Add(new TransactionView.Total { label = GetBilingualString(bilingual, nameof(Strings.Subtotal), "Subtotal"), number = taxExclusiveAmount, text = taxExclusiveAmount.ToCurrencyString(currency, CurrencySymbol.Short) });

                foreach (var e in taxComponents)
                {
                    var taxAmount = transactionLines.Where(x => x.TaxComponent == e).Sum(x => x.TransactionAmount);
                    if (reverseSign) taxAmount *= -1m;
                    o.totals.Add(new TransactionView.Total { label = e, @class = "taxAmount", number = taxAmount, text = taxAmount.ToCurrencyString(currency, CurrencySymbol.Short) });
                }
            }

            if (forceTotals || transactionLines.Any(x => x.TransactionAmount != 0m))
            {
                var total = transactionLines.Select(x => x.TransactionAmount).SafeSum();

                if (roundingTransaction != null)
                {
                    var roundingAmount = roundingTransaction.TransactionAmount;
                    if (roundingAmount != 0m)
                    {
                        if (reverseSign) roundingAmount *= -1m;
                        o.totals.Add(new TransactionView.Total { label = GetBilingualString(bilingual, nameof(Strings.Rounding), "Rounding"), number = roundingAmount, text = roundingAmount.ToCurrencyString(roundingTransaction.TransactionCurrency, CurrencySymbol.Short) });
                    }
                }

                total += (roundingTransaction?.TransactionAmount ?? 0m);

                if (withholdingTransaction != null)
                {
                    var withholdingAmount = withholdingTransaction.TransactionAmount;
                    if (withholdingAmount != 0m)
                    {
                        var subtotal = total;
                        if (reverseSign) subtotal *= -1m;
                        o.totals.Add(new TransactionView.Total { label = GetBilingualString(bilingual, nameof(Strings.Subtotal), "Sub-total"), number = subtotal, text = subtotal.ToCurrencyString(currency, CurrencySymbol.Short), emphasis = true });

                        if (reverseSign) withholdingAmount *= -1m;
                        o.totals.Add(new TransactionView.Total { label = GetBilingualString(bilingual, nameof(Strings.WithholdingTax), "Withholding tax"), number = withholdingAmount, text = withholdingAmount.ToCurrencyString(withholdingTransaction.TransactionCurrency, CurrencySymbol.Short) });
                    }
                }

                if (withholdingTransaction2 != null)
                {
                    var withholdingAmount = withholdingTransaction2.TransactionAmount;
                    if (withholdingAmount != 0m)
                    {
                        var subtotal = total;
                        if (reverseSign) subtotal *= -1m;
                        o.totals.Add(new TransactionView.Total { label = GetBilingualString(bilingual, nameof(Strings.Subtotal), "Sub-total"), number = subtotal, text = subtotal.ToCurrencyString(currency, CurrencySymbol.Short), emphasis = true });

                        if (reverseSign) withholdingAmount *= -1m;
                        o.totals.Add(new TransactionView.Total { label = GetBilingualString(bilingual, nameof(Strings.WithholdingTax), "Withholding tax"), number = withholdingAmount, text = withholdingAmount.ToCurrencyString(withholdingTransaction2.TransactionCurrency, CurrencySymbol.Short) });
                    }
                }

                total += (withholdingTransaction?.TransactionAmount ?? 0m);
                total += (withholdingTransaction2?.TransactionAmount ?? 0m);

                if (reverseSign) total *= -1m;
                o.totals.Add(new TransactionView.Total { key = "Total", label = GetBilingualString(bilingual, nameof(Strings.Total), "Total"), number = total, text = total.ToCurrencyString(currency, CurrencySymbol.Short), emphasis = true });

                if (amountsIncludeTax)
                {
                    foreach (var e in taxComponents)
                    {
                        var taxAmount = transactionLines.Where(x => x.TaxComponent == e).Sum(x => x.TransactionAmount);
                        if (reverseSign) taxAmount *= -1m;
                        o.totals.Add(new TransactionView.Total { label = string.Format(Strings.Includes_XXX, e), number = taxAmount, @class = "taxAmount", text = taxAmount.ToCurrencyString(currency, CurrencySymbol.Short) });
                    }
                }
            }

            return o;
        }

        public View GetView()
        {
            var o = AuthenticatedHandle();
            return ViewMapper.From(o);
        }
    }    
}
