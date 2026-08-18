using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using ManagerServer.Globalization;
using System.Threading.Tasks;
using System.Globalization;
using CsvHelper.Configuration.Attributes;
using ManagerServer.Helpers;
using ManagerServer.Model;
using CsvHelper;
using ManagerServer.HttpHandlers.Businesses.Business.Settings.BankRules;
using ManagerServer.Attributes;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace ManagerServer.HttpHandlers.Businesses.Business.BankAndCashAccounts
{
    [ProtoContract]
    [Title(nameof(Strings.ImportBankStatement))]
    [Guide("Most banks allow you to download transaction data for import into accounting systems.")]
    [Guide("Import bank statements to save time and reduce manual data entry errors.")]
    [Header("How to Import Bank Statements")]
    [Guide("Navigate to the **Bank and Cash Accounts** tab.")]
    [TabScreenshot("fa-coins", nameof(Strings.BankAndCashAccounts))]
    [Guide("Click the **Import Bank Statement** button in the bottom-right corner.")]
    [SmallBottomButtonScreenshot(nameof(Strings.ImportBankStatement))]
    [Guide("Select the bank account and choose your bank statement file, then click **Next**.")]
    [PrimaryButtonScreenshot(nameof(Strings.Next))]
    [Guide("Review the import summary showing balances and transaction counts, then click **Import** to proceed.")]
    [PrimaryButtonScreenshot(nameof(Strings.Import))]
    [Header("After Import")]
    [Guide("Imported transactions are automatically created as *Receipts* or *Payments*.")]
    [Guide("Use *Bank Rules* to automatically categorize imported transactions and save time.")]
    [LinkGuide("Learn more about automatic categorization:", typeof(BankRules))]
    [Guide("To undo an import, use the **History** screen to reverse the changes.")]
    [LinkGuide("Learn more about reversing transactions:", typeof(History))]
    [Header("Supported File Formats")]
    [Guide("Manager supports these bank statement formats:")]
    [Guide("• **Most reliable**: QIF, OFX, QFX, QBO, STA, SWI, 940, IIF, CAMT053, CAMT052")]
    [Guide("• **Less reliable**: XML, CSV (due to non-standard formats)")]
    [Guide("• **Not supported**: PDF (designed for human reading, not data processing)")]
    [Guide("Manager automatically interprets various CSV column layouts despite the lack of standardization.")]
    [Header("Common Issues")]
    [Guide("**Duplicate transactions** - Usually occur when banks change transaction dates between exports. Regular *bank reconciliations* help identify duplicates.")]
    [LinkGuide("Learn more about bank reconciliations:", typeof(BankReconciliations.BankReconciliations))]
    [Guide("**Date format confusion** - Dates like 01-02-2024 could mean January 2nd or February 1st depending on format.")]
    [Guide("Manager analyzes your file to determine the most likely date format. Import files with many transactions for better accuracy.")]
    internal sealed class ImportBankStatement : BusinessTemplate
    {
        [ProtoMember(1)] public Guid? BankAccount;

        protected override void InnerGet2()
        {
            var bankAccount = ApplicationData.Businesses.Get(Business).SingleOrDefault<BankOrCashAccount>(BankAccount);
            string error = null;
            ParsedBankStatementLine[] parsedBankStatementLines = null;

            if (Request.HasFormContentType)
            {
                var form = Request.ReadFormAsync().GetAwaiter().GetResult();

                if (form.ContainsKey("BankAccount"))
                {
                    if (Guid.TryParse(form["BankAccount"], out Guid key))
                    {
                        bankAccount = ApplicationData.Businesses.Get(Business).SingleOrDefault<ManagerServer.Model.BankOrCashAccount>(key);
                    }
                }

                string filename = null;
                byte[] content = null;

                if (form.Files.Count > 0)
                {
                    filename = form.Files[0].FileName;

                    using (var ms2 = new MemoryStream())
                    {
                        form.Files[0].CopyTo(ms2);
                        content = ms2.ToArray();
                    }
                }

                if (string.IsNullOrWhiteSpace(filename) || content == null || content.Length == 0)
                {
                    error = Strings.ImportedFileInvalid;
                }
                else
                {
                    try
                    {
                        parsedBankStatementLines = ParseBankStatementLines(filename, content);
                        var lockDate = ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.LockDate>();
                        if (lockDate.GetLockDate().HasValue) parsedBankStatementLines = parsedBankStatementLines.Where(x => x.Date > lockDate.GetLockDate()).ToArray();
                    }
                    catch (CsvHelper.HeaderValidationException ex)
                    {
                        error = ex.Message;
                        if (error.Length > 1000) error = error.Substring(0, 1000);
                    }
                    catch (CsvHelper.MissingFieldException ex)
                    {
                        error = ex.Message;
                        if (error.Length > 1000) error = error.Substring(0, 1000);
                    }
                    catch (ArgumentException ex)
                    {
                        error = ex.Message;
                    }
                    catch (System.Xml.XmlException ex)
                    {
                        error = ex.Message;
                    }
                }
            }

            if (bankAccount == null || parsedBankStatementLines == null || !string.IsNullOrWhiteSpace(error))
            {
                using (PostForm(formData: true))
                {
                    using (Div(@class: "card"))
                    {
                        using (Div(@class: "card-header"))
                        {
                            using (Div(@class: "card-title"))
                            {
                                Write(Strings.ImportBankStatement);
                            }
                        }
                        using (Div(@class: "card-form"))
                        {
                            using (Div(@class: "form-group"))
                            {
                                using (Label()) Write(Strings.BankAccount);
                                using (Div(@class: "controls"))
                                {
                                    using (Select(name: "BankAccount", @class: "form-select", style: "width: 300px"))
                                    {
                                        Option();
                                        var userPermissions = this.GetCurrentUserPermissions(Business).GetBankCashAccounts();
                                        foreach (var e in ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.BankOrCashAccount>().Where(x => !x.Inactive).OrderBy(x => x.NameWithCode))
                                        {
                                            if (userPermissions.Length > 0 && !userPermissions.Contains(e.Key)) continue;
                                            Option(value: e.Key.ToString(), text: e.NameWithCode, selected: e.Key == bankAccount?.Key);
                                        }
                                    }
                                }
                            }

                            using (Div(@class: "form-group"))
                            {
                                var extensions = new string[] { "qif", "ofx", "qfx", "qbo", "sta", "swi", "940", "iif", "csv", "camt053", "camt052", "xml" };

                                using (Label()) Write(Strings.SelectFileFromYourComputer);
                                using (Div()) InputFile(accept: string.Join(",", extensions.Select(x => '.' + x).ToArray()), name: "File", @class: "form-file");
                            }

                            if (!string.IsNullOrWhiteSpace(error)) using (P(@class: "text-red-500 font-semibold")) Write(error);
                        }

                        using (Div(@class: "card-header"))
                        {
                            using (PrimaryButton()) Write(Strings.Next);
                        }
                    }
                }
            }
            else
            {
                var transactions = new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).Where(x => x.GeneralLedgerAccount.IsCashAtBank && x.BankAccount?.Key == bankAccount.Key && x.ClearDate.HasValue).ToArray();
                var closingBalanceBeforeImport = transactions.Sum(x => x.AccountAmount);
                var bankAccountCurrency = bankAccount.Currency;

                var temp = transactions.Select(x => new { Date = x.ClearDate, x.AccountAmount, x.Description }).ToList();

                var bankStatementLinesPendingToCheck = parsedBankStatementLines.ToList();
                var bankStatementLinesToImport = new List<ImportBankStatement.ParsedBankStatementLine>();
                foreach (var e in bankStatementLinesPendingToCheck.ToArray())
                {
                    var o = temp.FirstOrDefault(x => x.Date == e.Date && x.AccountAmount == e.Amount && x.Description == e.Description);
                    if (o != null)
                    {
                        temp.Remove(o);
                        bankStatementLinesPendingToCheck.Remove(e);
                    }
                }
                foreach (var e in bankStatementLinesPendingToCheck)
                {
                    var o = temp.FirstOrDefault(x => x.Date == e.Date && x.AccountAmount == e.Amount);
                    if (o == null) bankStatementLinesToImport.Add(e);
                    else temp.Remove(o);
                }
                var closingBalanceAfterImport = closingBalanceBeforeImport + bankStatementLinesToImport.Sum(x => x.Amount);
                var numberOfBankStatementLinesAlreadyImported = parsedBankStatementLines.Length - bankStatementLinesToImport.Count;

                var currencies = ManagerServer.Query.Currencies.GetCurrencyProvider(Business);

                using (PostForm())
                {
                    using (Div(@class: "flex"))
                    {
                        using (Div(@class: "card"))
                        {
                            using (Div(@class: "card-header"))
                            {
                                using (Div(@class: "card-title"))
                                {
                                    Write(Strings.ImportBankStatement);
                                }
                            }
                            using (Table(@class: "card-table"))
                            {
                                using (Tr())
                                {
                                    using (Td()) Write(Strings.Bank_account);
                                    using (Td(@class: "text-right font-semibold")) Write(bankAccount.Name);
                                }
                                using (Tr())
                                {
                                    using (Td()) Write(Strings.ClosingBalanceBeforeImport);
                                    using (Td(@class: "text-right font-semibold")) Write(closingBalanceBeforeImport.ToCurrencyString(currencies.Get(bankAccountCurrency), CurrencySymbol.Long));
                                }
                                using (Tr())
                                {
                                    using (Td()) Write(Strings.NumberOfTransactionsInTheFile);
                                    using (Td(@class: "text-right font-semibold")) Write(string.Format(Strings.XXX_Transactions, parsedBankStatementLines.Length.ToString("N0", System.Threading.Thread.CurrentThread.CurrentCulture)));
                                }
                                using (Tr())
                                {
                                    using (Td()) Write(Strings.NumberOfTransactionsAlreadyImported);
                                    using (Td(@class: "text-right font-semibold")) Write(string.Format(Strings.XXX_Transactions, numberOfBankStatementLinesAlreadyImported.ToString("N0", System.Threading.Thread.CurrentThread.CurrentCulture)));
                                }
                                using (Tr())
                                {
                                    using (Td()) Write(Strings.NumberOfTransactionsToImport);
                                    using (Td(@class: "text-right font-semibold")) Write(string.Format(Strings.XXX_Transactions, bankStatementLinesToImport.Count.ToString("N0", System.Threading.Thread.CurrentThread.CurrentCulture)));
                                }
                                using (Tr())
                                {
                                    using (Td()) Write(Strings.ClosingBalanceAfterImport);
                                    using (Td(@class: "text-right font-semibold")) Write(closingBalanceAfterImport.ToCurrencyString(currencies.Get(bankAccountCurrency), CurrencySymbol.Long));
                                }
                            }

                            using (Div(@class: "card-header"))
                            {
                                using (var ms = new System.IO.MemoryStream())
                                {
                                    ProtoBuf.Serializer.Serialize<ImportBankStatement.ParsedBankStatementLine[]>(ms, bankStatementLinesToImport.ToArray());
                                    InputHidden(name: "Data", value: Convert.ToBase64String(ms.ToArray()));
                                }

                                InputHidden(name: "BankAccount", value: bankAccount.Key.ToString("N"));
                                using (PrimaryButton()) Write(Strings.Import);
                            }
                        }
                    }
                }
            }
        }

        protected override async Task InnerPost()
        {
            var userPermissions = this.GetCurrentUserPermissions(Business);
            if (!this.IsAdministrator())
            {
                if (!userPermissions.CanCreate(this.GetType().Namespace))
                {
                    Response.Redirect(new BankAndCashAccounts() { Business = Business, Referrer = Referrer }.ToUrl());
                    return;
                }
            }

            var form = await Request.ReadFormAsync();

            if (!form.ContainsKey("Data"))
            {
                await Get();
                return;
            }

            var data = form["Data"];
            var ms = new System.IO.MemoryStream(Convert.FromBase64String(data));
            var bankStatementLines = ProtoBuf.Serializer.Deserialize<ImportBankStatement.ParsedBankStatementLine[]>(ms);
            bankStatementLines = bankStatementLines.Where(x => x.Amount != 0m).ToArray();

            var bankAccount = new Guid(form["BankAccount"]);            

            var list = new List<ManagerServer.Model.Object>();
            var receiptTemplate = ApplicationData.Businesses.Get(Business).Single<Receipt>();
            var paymentTemplate = ApplicationData.Businesses.Get(Business).Single<Payment>();
            long nextReceiptReference = 1;
            long nextPaymentReference = 1;
            if (receiptTemplate.AutomaticReference)
            {
                var references = ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.Receipt>().Where(x => !string.IsNullOrWhiteSpace(x.Reference)).Select(x => x.Reference).ToArray();
                foreach (var e in references)
                {
                    if (string.IsNullOrWhiteSpace(e)) continue;
                    var s = string.Join("", e.ToCharArray().Where(x => char.IsDigit(x)));
                    if (string.IsNullOrWhiteSpace(s)) continue;
                    long i = 0;
                    if (long.TryParse(s, out i))
                    {
                        if (i >= nextReceiptReference) nextReceiptReference = i + 1;
                    }
                }
            }
            if (paymentTemplate.AutomaticReference)
            {
                var references = ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.Payment>().Where(x => !string.IsNullOrWhiteSpace(x.Reference)).Select(x => x.Reference).ToArray();
                foreach (var e in references)
                {
                    if (string.IsNullOrWhiteSpace(e)) continue;
                    var s = string.Join("", e.ToCharArray().Where(x => char.IsDigit(x)));
                    if (string.IsNullOrWhiteSpace(s)) continue;
                    long i = 0;
                    if (long.TryParse(s, out i))
                    {
                        if (i >= nextPaymentReference) nextPaymentReference = i + 1;
                    }
                }
            }

            var foreignCurrency = ApplicationData.Businesses.Get(Business).SingleOrDefault<ForeignCurrency>(ApplicationData.Businesses.Get(Business).SingleOrDefault<BankOrCashAccount>(bankAccount)?.Currency);
            var exchangeRates = ApplicationData.Businesses.Get(Business).OfType<ExchangeRate>().Where(x => x.Currency.HasValue && x.Currency == foreignCurrency?.Key).OrderByDescending(x => x.Date).ToArray();

            foreach (var bankStatementLine in bankStatementLines.OrderBy(x => x.Date))
            {
                if (bankStatementLine.Amount == 0m) continue;

                if (bankStatementLine.Amount > 0)
                {
                    var o = ProtoBuf.Serializer.DeepClone<Receipt>(receiptTemplate);
                    o.Key = Guid.CreateVersion7();
                    o.AmountsAreTaxExclusive = false;
                    o.ReceivedIn = bankAccount;
                    o.Reference = bankStatementLine.Reference;
                    o.Date = bankStatementLine.Date;
                    o.Cleared = ManagerServer.Model.Enums.BankAccountClearStatus.OnTheSameDate;
                    o.Description = bankStatementLine.Description;
                    o.Lines = bankStatementLine.LineItems.Select(x => new ManagerServer.Model.Receipt.Line() { LineDescription = x.Description, Amount = x.Amount, Qty = x.Qty }).ToArray();                    
                    o.HasLineDescription = o.Lines.Any(x => !string.IsNullOrWhiteSpace(x.LineDescription));
                    if (o.AutomaticReference)
                    {
                        o.Reference = nextReceiptReference.ToString();
                        nextReceiptReference++;
                        o.AutomaticReference = false;
                    }
                    o.QuantityColumn = o.Lines.Any(x => x.Qty.HasValue);
                    o.UnitPriceColumn = false;
                    o.FixedTotal = true;
                    o.FixedTotalAmount = bankStatementLine.Amount;

                    list.Add(o);
                }
                if (bankStatementLine.Amount < 0)
                {
                    var o = ProtoBuf.Serializer.DeepClone<Payment>(paymentTemplate);
                    o.Key = Guid.CreateVersion7();
                    o.AmountsAreTaxExclusive = false;
                    o.PaidFrom = bankAccount;
                    o.Reference = bankStatementLine.Reference;
                    o.Date = bankStatementLine.Date;
                    o.Cleared = ManagerServer.Model.Enums.BankAccountClearStatus.OnTheSameDate;
                    o.Description = bankStatementLine.Description;
                    o.Lines = bankStatementLine.LineItems.Select(x => new ManagerServer.Model.Payment.Line() { LineDescription = x.Description, Amount = x.Amount * -1, Qty = x.Qty }).ToArray();                    
                    o.HasLineDescription = o.Lines.Any(x => !string.IsNullOrWhiteSpace(x.LineDescription));
                    if (o.AutomaticReference)
                    {
                        o.Reference = nextPaymentReference.ToString();
                        nextPaymentReference++;
                        o.AutomaticReference = false;
                    }
                    o.QuantityColumn = o.Lines.Any(x => x.Qty.HasValue);
                    o.UnitPriceColumn = false;
                    o.FixedTotal = true;
                    o.FixedTotalAmount = bankStatementLine.Amount * -1m;
                    list.Add(o);
                }
            }

            if (foreignCurrency != null)
            {
                foreach (var e in list.Cast<IForeignCurrencyTransaction>())
                {
                    var exchangeRate = exchangeRates.FirstOrDefault(x => x.Date <= e.Date);
                    if (exchangeRate != null)
                    {
                        e.ExchangeRate = exchangeRate.ExchangeRateValue;
                        e.ExchangeRateIsInverse = exchangeRate.ExchangeRateIsInverse;
                    }
                    else
                    {
                        e.ExchangeRate = 1m;
                    }
                }
            }

            ApplicationData.Businesses.Process(Business, list.ToArray(), GetUserName());

            Response.Redirect(new BankAndCashAccounts() { Business = Business }.ToUrl());
        }

        [ProtoContract]
        public sealed class ParsedBankStatementLine
        {
            [ProtoMember(1)] public DateTime Date;
            [ProtoMember(2)] public string Description;
            [ProtoMember(3)] public string Reference;            
            [ProtoMember(5)] public LineItem[] LineItems;

            public decimal Amount
            {
                get
                {
                    return LineItems.Sum(x => x.Amount);
                }
            }

            [ProtoContract]
            public sealed class LineItem
            {
                [ProtoMember(1)] public string Description;
                [ProtoMember(2)] public decimal Amount;
                [ProtoMember(3)] public decimal? Qty;
            }
        }

        public sealed class RawBankStatementLine
        {
            public string Date;
            public decimal Amount;
            public decimal? Quantity;
            public string Description;
            public string Reference;
            public List<LineItem> LineItems = new List<LineItem>();

            public sealed class LineItem
            {
                public string Description;
                public decimal Amount;
            }
        }

        public ParsedBankStatementLine[] ParseBankStatementLines(string fileName, byte[] buffer)
        {
            var list = new List<ParsedBankStatementLine>();
            if (fileName.ToLowerInvariant().EndsWith(".qif"))
            {
                System.IO.StreamReader r = new System.IO.StreamReader(new MemoryStream(buffer));

                var innerList = new List<RawBankStatementLine>();
                var o = new RawBankStatementLine();
                RawBankStatementLine.LineItem lineItem = new RawBankStatementLine.LineItem();
                while (!r.EndOfStream)
                {
                    string value = r.ReadLine();
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        continue;
                    }
                    else if (value.StartsWith("^"))
                    {
                        if (o.Amount != 0m && !string.IsNullOrWhiteSpace(o.Date))
                        {
                            if (lineItem.Amount != 0m) o.LineItems.Add(lineItem);
                            innerList.Add(o);
                            lineItem = new RawBankStatementLine.LineItem();
                        }
                        o = new RawBankStatementLine();
                    }
                    else if (value.StartsWith("D"))
                    {
                        var text = value.Substring(1);
                        if (!string.IsNullOrWhiteSpace(text))
                        {
                            text = text.Replace('-', '/');
                            if (text.Contains('/') && text.All(x => char.IsDigit(x) || x == '/') && text.Split('/').Length == 3)
                            {
                                var parts = text.Split('/');
                                o.Date = parts[0] + "/" + parts[1] + "/" + parts[2];
                            }
                            else
                            {
                                text = string.Join("", text.Where(x => char.IsDigit(x)).ToArray());
                                if (text.Length == 8)
                                {
                                    o.Date = text.Substring(0, 2) + "/" + text.Substring(2, 2) + "/" + text.Substring(4);
                                }
                                else if (text.Length == 6)
                                {
                                    o.Date = text.Substring(0, 2) + "/" + text.Substring(2, 2) + "/" + text.Substring(4);
                                }
                            }
                        }
                    }
                    else if (value.StartsWith("T"))
                    {
                        if (value == "T..." || value == "T") { }
                        else
                        {
                            var text = value.Substring(1);
                            foreach (var e in new[] { "$", "£", "¥", "+", " ", "," }) if (text.Contains(e)) text = text.Replace(e, "");
                            var amount = 0m;
                            if (decimal.TryParse(text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out amount))
                            {
                                o.Amount = amount;
                            }
                        }
                    }
                    else if (value.StartsWith("$"))
                    {
                        var text = value.Substring(1);
                        foreach (var e in new[] { "$", "£", "¥", "+", " ", "," }) if (text.Contains(e)) text = text.Replace(e, "");
                        var amount = 0m;
                        if (decimal.TryParse(text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out amount))
                        {
                            lineItem.Amount = amount;
                        }

                        if (lineItem.Amount != 0m) o.LineItems.Add(lineItem);
                        lineItem = new RawBankStatementLine.LineItem();
                    }
                    else if (value.StartsWith("S"))
                    {
                        lineItem.Description = value.Substring(1);
                    }
                    else if (value.StartsWith("N"))
                    {
                        if (string.IsNullOrWhiteSpace(o.Description)) o.Description = value.Substring(1);
                        else o.Description += " " + value.Substring(1);
                        o.Description = Regex.Replace(o.Description, @"\s+", " ");
                    }
                    else if (value.StartsWith("A"))
                    {
                        if (string.IsNullOrWhiteSpace(o.Description)) o.Description = value.Substring(1);
                        else o.Description += " " + value.Substring(1);
                    }
                    else if (value.StartsWith("P"))
                    {
                        if (string.IsNullOrWhiteSpace(o.Description)) o.Description = value.Substring(1);
                        else o.Description += " " + value.Substring(1);
                        o.Description = Regex.Replace(o.Description, @"\s+", " ");
                    }
                    else if (value.StartsWith("M"))
                    {
                        if (string.IsNullOrWhiteSpace(o.Description)) o.Description = value.Substring(1);
                        else o.Description += " " + value.Substring(1);
                        o.Description = Regex.Replace(o.Description, @"\s+", " ");
                    }
                }

                innerList = innerList.Where(x => !string.IsNullOrWhiteSpace(x.Date)).ToList();

                if (innerList.Count == 0) throw new ArgumentException("File you are trying to upload appears to be empty.");

                var usFormat = innerList.All(x => int.Parse(x.Date.Split('/')[0]) <= 12);
                var gbFormat = innerList.All(x => int.Parse(x.Date.Split('/')[1]) <= 12);
                var isoFormat = innerList.All(x => x.Date.Split('/')[0].Length == 4 && int.Parse(x.Date.Split('/')[1]) <= 12 && int.Parse(x.Date.Split('/')[2]) <= 31);

                if (!usFormat && !gbFormat && !isoFormat) throw new ArgumentException("File you are trying to upload appears to be corrupted.");

                if (isoFormat)
                {
                    usFormat = false;
                    gbFormat = false;
                }

                if (usFormat && gbFormat)
                {
                    var usDates = innerList.Select(x => int.Parse(GetYear(x.Date.Split('/')[2]) + x.Date.Split('/')[0] + x.Date.Split('/')[1])).ToArray();
                    var gbDates = innerList.Select(x => int.Parse(GetYear(x.Date.Split('/')[2]) + x.Date.Split('/')[1] + x.Date.Split('/')[0])).ToArray();

                    if (usFormat && gbFormat)
                    {
                        var today = int.Parse(DateTime.Today.ToString("yyyyMMdd"));
                        if (gbDates.Max() > today) gbFormat = false;
                        if (usDates.Max() > today) usFormat = false;
                    }

                    if (usFormat && gbFormat)
                    {
                        usFormat = usDates.OrderByDescending(x => x).SequenceEqual(usDates) || usDates.OrderBy(x => x).SequenceEqual(usDates);
                        gbFormat = gbDates.OrderByDescending(x => x).SequenceEqual(gbDates) || gbDates.OrderBy(x => x).SequenceEqual(gbDates);
                    }

                    if (usFormat && gbFormat)
                    {
                        if (usDates.SequenceEqual(gbDates)) usFormat = false;
                    }                    

                    if (usFormat && gbFormat)
                    {
                        if (gbDates.Max() > usDates.Max()) usFormat = false;
                        if (usDates.Max() > gbDates.Max()) gbFormat = false;                        
                    }

                    if (usFormat && gbFormat)
                    {
                        throw new ArgumentException("File you are trying to upload has ambiguous date format. Import bank statement covering longer period.");
                    }
                }

                foreach (var e in innerList)
                {
                    DateTime date = default(DateTime);
                    var dateElements = e.Date.Split('/');
                    if (isoFormat)
                    {
                        date = new DateTime(int.Parse(dateElements[0]), int.Parse(dateElements[1]), int.Parse(dateElements[2]));
                    }
                    else
                    {
                        var year = int.Parse(dateElements[2]);
                        if (year < 100) year += 2000;
                        if (usFormat)
                        {
                            date = new DateTime(year, int.Parse(dateElements[0]), int.Parse(dateElements[1]));
                        }
                        if (gbFormat)
                        {
                            date = new DateTime(year, int.Parse(dateElements[1]), int.Parse(dateElements[0]));
                        }
                    }
                    if (e.Amount == 0m) continue;
                    var line = new ParsedBankStatementLine();
                    line.Date = date;
                    if (e.LineItems != null && e.LineItems.Any())
                    {
                        line.LineItems = e.LineItems.Select(x => new ParsedBankStatementLine.LineItem() { Amount = x.Amount, Description = x.Description }).ToArray();
                    }
                    else
                    {
                        line.LineItems = new ParsedBankStatementLine.LineItem[] { new ParsedBankStatementLine.LineItem() { Amount = e.Amount } };
                    }
                    line.Description = e.Description;
                    list.Add(line);
                }
            }
            else if (fileName.ToLowerInvariant().EndsWith(".ofx") || fileName.ToLowerInvariant().EndsWith(".qfx") || fileName.ToLowerInvariant().EndsWith(".qbo"))
            {
                System.IO.StreamReader r = new System.IO.StreamReader(new MemoryStream(buffer));
                string input = r.ReadToEnd();

                var index = 0;
                while (input.IndexOf("<STMTTRN>", index) != -1)
                {
                    index = input.IndexOf("<STMTTRN>", index) + "<STMTTRN>".Length;
                    var subset = input.Substring(index, input.IndexOf("</STMTTRN>", index) - index + 1);                    

                    var innerIndex = subset.IndexOf("<DTPOSTED>") + "<DTPOSTED>".Length;
                    var date = subset.Substring(innerIndex, 8);

                    if (!date.All(x => char.IsDigit(x))) continue;

                    innerIndex = subset.IndexOf("<TRNAMT>") + "<TRNAMT>".Length;
                    string amount = subset.Substring(innerIndex, subset.IndexOf("<", innerIndex) - innerIndex);
                    amount = amount.Replace("\n", string.Empty);
                    amount = amount.Replace(" ", string.Empty);

                    innerIndex = subset.IndexOf("<NAME><![CDATA[");
                    string name = null;
                    if (innerIndex != -1)
                    {
                        innerIndex += "<NAME><![CDATA[".Length;
                        name = subset.Substring(innerIndex, subset.IndexOf("<", innerIndex) - innerIndex).Trim();
                        name = name.Substring(0, name.Length - 3);
                    }

                    if (string.IsNullOrWhiteSpace(name))
                    {
                        innerIndex = subset.IndexOf("<NAME>");
                        if (innerIndex != -1)
                        {
                            innerIndex += "<NAME>".Length;
                            name = subset.Substring(innerIndex, subset.IndexOf("<", innerIndex) - innerIndex).Trim();
                        }
                    }

                    innerIndex = subset.IndexOf("<MEMO><![CDATA[");
                    string memo = null;
                    if (innerIndex != -1)
                    {
                        innerIndex += "<MEMO><![CDATA[".Length;
                        memo = subset.Substring(innerIndex, subset.IndexOf("<", innerIndex) - innerIndex).Trim();
                        memo = memo.Substring(0, memo.Length - 3);
                    }

                    if (string.IsNullOrWhiteSpace(memo))
                    {
                        innerIndex = subset.IndexOf("<MEMO>");
                        if (innerIndex != -1)
                        {
                            innerIndex += "<MEMO>".Length;
                            memo = subset.Substring(innerIndex, subset.IndexOf("<", innerIndex) - innerIndex).Trim();
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(memo) && !string.IsNullOrWhiteSpace(name))
                    {
                        if (memo.Contains(name)) name = string.Empty;
                        if (name.Contains(memo)) memo = string.Empty;
                    }

                    innerIndex = subset.IndexOf("<CHECKNUM>");
                    string checknum = null;
                    if (innerIndex != -1)
                    {
                        innerIndex += +"<CHECKNUM>".Length;
                        checknum = subset.Substring(innerIndex, subset.IndexOf("<", innerIndex) - innerIndex);
                    }

                    innerIndex = subset.IndexOf("<TRNTYPE>");
                    string trntype = null;
                    if (innerIndex != -1)
                    {
                        innerIndex += +"<TRNTYPE>".Length;
                        trntype = subset.Substring(innerIndex, subset.IndexOf("<", innerIndex) - innerIndex);
                    }

                    var description = string.Empty;
                    if (!string.IsNullOrWhiteSpace(checknum) && checknum != "0") description += " CHEQUE #" + checknum;
                    if (!string.IsNullOrWhiteSpace(name)) description += " " + name;
                    if (!string.IsNullOrWhiteSpace(memo)) description += " " + memo;
                    if (!string.IsNullOrWhiteSpace(trntype)) description += " " + trntype;
                    description = description.Trim();
                    description = Regex.Replace(description, @"\s+", " ");

                    if (decimal.TryParse(amount, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal parsedAmount))
                    {
                        list.Add(new ParsedBankStatementLine()
                        {
                            Description = description,
                            Date = new DateTime(int.Parse(date.Substring(0, 4)), int.Parse(date.Substring(4, 2)), int.Parse(date.Substring(6, 2))),
                            LineItems = new ParsedBankStatementLine.LineItem[] { new ParsedBankStatementLine.LineItem() { Amount = parsedAmount } }
                        });
                    }
                }
            }
            else if (fileName.ToLowerInvariant().EndsWith(".sta") || fileName.ToLowerInvariant().EndsWith(".swi") || fileName.ToLowerInvariant().EndsWith(".940"))
            {
                var ms = new MemoryStream();
                var w = new System.IO.StreamWriter(ms);
                var r = new System.IO.StreamReader(new System.IO.MemoryStream(buffer));
                var lastLine = "";
                while (!r.EndOfStream)
                {
                    var line = r.ReadLine();
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    if (line == "-}{5:}") line = "-";
                    w.WriteLine(line);
                    lastLine = line;
                }
                if (lastLine != "-") w.WriteLine("-");
                w.Flush();
                ms.Position = 0;

                var culture = System.Globalization.CultureInfo.InvariantCulture.Clone() as CultureInfo;
                culture.NumberFormat.NumberDecimalSeparator = ",";
                culture.NumberFormat.NumberGroupSeparator = ".";
                var items = Raptorious.SharpMt940Lib.Mt940Parser.Parse(new Raptorious.SharpMt940Lib.Mt940Format.AbnAmro(), new System.IO.StreamReader(ms), culture);
                foreach (var e in items)
                {
                    foreach (var e2 in e.Transactions)
                    {
                        var amount = e2.Amount.Value;
                        if (e2.DebitCredit == Raptorious.SharpMt940Lib.DebitCredit.Debit) amount = amount * -1;
                        list.Add(new ParsedBankStatementLine()
                        {
                            Date = e2.ValueDate,
                            Description = Regex.Replace((e2.Description + " " + e2.Reference).Trim(), @"\s+", " "),
                            LineItems = new ParsedBankStatementLine.LineItem[] { new ParsedBankStatementLine.LineItem() { Amount = amount } }
                        });
                    }
                }
            }
            else if (fileName.ToLowerInvariant().EndsWith(".camt053") || fileName.ToLowerInvariant().EndsWith(".camt052") || fileName.ToLowerInvariant().EndsWith(".xml"))
            {
                var xml = new System.Xml.XmlDocument();
                xml.LoadXml(System.Text.UTF8Encoding.UTF8.GetString(buffer));

                var json = Newtonsoft.Json.JsonConvert.SerializeXmlNode(xml);
                var camt053 = Newtonsoft.Json.JsonConvert.DeserializeObject<Camt053>(json);
                var lines = camt053?.Document?.BkToCstmrStmt?.Stmt?.Where(x => x.Ntry != null).SelectMany(x => x.Ntry).ToArray();

                if (lines != null)
                {
                    foreach (var e in lines)
                    {
                        var amount = decimal.Parse(e.Amt.text, CultureInfo.InvariantCulture);
                        if (e.CdtDbtInd == "DBIT") amount *= -1m;

                        list.Add(new ParsedBankStatementLine()
                        {
                            Date = e.BookgDt.GetDate().Value,
                            Description = e.GetDescription(),
                            LineItems = new ParsedBankStatementLine.LineItem[] { new ParsedBankStatementLine.LineItem() { Amount = amount } }
                        });
                    }
                }
                else
                {
                    var camt052 = Newtonsoft.Json.JsonConvert.DeserializeObject<Camt052>(json);
                    var lines2 = camt052?.Document?.BkToCstmrAcctRpt?.Rpt?.Where(x => x.Ntry != null).SelectMany(x => x.Ntry).ToArray();
                    if (lines2 != null)
                    {
                        foreach (var e in lines2)
                        {
                            var amount = decimal.Parse(e.Amt.text, CultureInfo.InvariantCulture);
                            if (e.CdtDbtInd == "DBIT") amount *= -1m;

                            list.Add(new ParsedBankStatementLine()
                            {
                                Date = e.BookgDt.GetDate().Value,
                                Description = e.GetDescription(),
                                LineItems = new ParsedBankStatementLine.LineItem[] { new ParsedBankStatementLine.LineItem() { Amount = amount } }
                            });
                        }
                    }
                }
            }
            else if (fileName.ToLowerInvariant().EndsWith(".csv"))
            {
                var csvLines = new List<CsvLine>();
                using (var r = new StreamReader(new MemoryStream(buffer)))
                {
                    var csvConfiguration = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        BadDataFound = x => { },
                        MissingFieldFound = x => { },
                        DetectDelimiter = true,
                        TrimOptions = CsvHelper.Configuration.TrimOptions.Trim,
                        PrepareHeaderForMatch = x => x.Header.Trim()
                        //Mode = CsvMode.Escape - this breaks some bank statements
                    };
                    using (var csv = new CsvHelper.CsvReader(r, csvConfiguration))
                    {
                        var headerValidationException = default(HeaderValidationException);
                        while (csv.Read())
                        {
                            csv.ReadHeader();
                            try
                            {
                                csv.ValidateHeader<CsvLine>();
                                break;
                            }
                            catch (HeaderValidationException ex)
                            {
                                if (headerValidationException == null) headerValidationException = ex;
                            }
                        }

                        while (csv.Read())
                        {
                            var csvLine = csv.GetRecord<CsvLine>();
                            if (csvLine != null) csvLines.Add(csv.GetRecord<CsvLine>());
                        }

                        if (csvLines.Count == 0 && headerValidationException != null) throw headerValidationException;
                    }
                }

                var innerList = new List<RawBankStatementLine>();
                foreach (var e in csvLines)
                {
                    var o = new RawBankStatementLine();

                    o.Date = e.Date ?? string.Empty;
                    o.Date = o.Date.Replace('.', '/');
                    o.Date = o.Date.Replace('-', '/');
                    o.Date = o.Date.Replace(" 12:00:00 AM", string.Empty);
                    o.Date = o.Date.Replace("Jan", "01");
                    o.Date = o.Date.Replace("Feb", "02");
                    o.Date = o.Date.Replace("Mar", "03");
                    o.Date = o.Date.Replace("Apr", "04");
                    o.Date = o.Date.Replace("May", "05");
                    o.Date = o.Date.Replace("Jun", "06");
                    o.Date = o.Date.Replace("Jul", "07");
                    o.Date = o.Date.Replace("Aug", "08");
                    o.Date = o.Date.Replace("Sep", "09");
                    o.Date = o.Date.Replace("Oct", "10");
                    o.Date = o.Date.Replace("Nov", "11");
                    o.Date = o.Date.Replace("Dec", "12");
                    o.Date = string.Join(string.Empty, o.Date.Where(x => char.IsDigit(x) || x == '/').ToArray());
                    if (o.Date.Length > 10) o.Date = o.Date.Substring(0, 10);

                    o.Description = e.Description+" "+e.Contact;

                    var amountText = e.Amount ?? string.Empty;
                    amountText = amountText.Replace(" ", string.Empty);
                    amountText = amountText.Replace("$", string.Empty);
                    amountText = amountText.Replace("R", string.Empty);

                    if (!string.IsNullOrWhiteSpace(amountText))
                    {
                        var parsedAmount = 0m;
                        if (decimal.TryParse(amountText, NumberStyles.Any, CultureInfo.InvariantCulture, out parsedAmount))
                        {
                            if (parsedAmount != 0m) o.Amount = parsedAmount;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(e.Quantity))
                    {
                        if (decimal.TryParse(e.Quantity, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal parsedQuantity))
                        {
                            if (parsedQuantity != 0m) o.Quantity = parsedQuantity;
                        }
                    }

                    foreach (var e2 in new[] { e.Debit, e.Debit2 })
                    {
                        if (!string.IsNullOrWhiteSpace(e2))
                        {
                            var amountText2 = e2;
                            amountText2 = amountText2.Replace("$", string.Empty);
                            amountText2 = amountText2.Replace("R", string.Empty);

                            if (!string.IsNullOrWhiteSpace(amountText2))
                            {
                                var parsedAmount = 0m;
                                if (decimal.TryParse(amountText2, NumberStyles.Any, CultureInfo.InvariantCulture, out parsedAmount))
                                {
                                    if (parsedAmount != 0m) o.Amount = parsedAmount * -1m;
                                }
                            }
                        }
                    }

                    if (e.IsDeposit.HasValue && !e.IsDeposit.Value)
                    {
                        o.Amount *= -1m;
                    }
                    
                    o.Reference = e.Reference;

                    if (string.IsNullOrWhiteSpace(o.Date)) continue;

                    innerList.Add(o);
                }

                if (innerList.Count == 0) throw new ArgumentException("File you are trying to upload appears to be empty.");
                if (innerList.Any(x => x.Date.Split('/').Length != 3)) throw new ArgumentException("Date in CSV file must be in format dd/mm/yyyy, mm/dd/yyyy or yyyy/mm/dd");
                if (innerList.SelectMany(x => x.Date.Split('/')).Any(x => !int.TryParse(x, out int result))) throw new ArgumentException("Date in CSV file must be in format dd/mm/yyyy, mm/dd/yyyy or yyyy/mm/dd");

                var isUsFormat = IsUsFormat(innerList.Select(x => x.Date).ToArray());

                foreach (var e in innerList)
                {
                    DateTime date = default(DateTime);
                    var dateElements = e.Date.Split('/');
                    if (isUsFormat)
                    {
                        var year = int.Parse(dateElements[2]);
                        if (year < 100) year += 2000;
                        date = new DateTime(year, int.Parse(dateElements[0]), int.Parse(dateElements[1]));
                    }
                    else
                    {
                        if (dateElements[0].Length == 4)
                        {
                            var year = int.Parse(dateElements[0]);
                            date = new DateTime(year, int.Parse(dateElements[1]), int.Parse(dateElements[2]));
                        }
                        else
                        {
                            var year = int.Parse(dateElements[2]);
                            if (year < 100) year += 2000;
                            date = new DateTime(year, int.Parse(dateElements[1]), int.Parse(dateElements[0]));
                        }
                    }
                    if (e.Amount == 0) continue;
                    var line = new ParsedBankStatementLine();
                    line.Date = date;
                    if (e.LineItems != null && e.LineItems.Any())
                    {
                        line.LineItems = e.LineItems.Select(x => new ParsedBankStatementLine.LineItem() { Amount = x.Amount, Description = x.Description }).ToArray();
                    }
                    else
                    {
                        line.LineItems = new ParsedBankStatementLine.LineItem[] { new ParsedBankStatementLine.LineItem() { Amount = e.Amount, Qty = e.Quantity } };
                    }
                    line.Description = string.Join(" ", new[] { e.Reference, e.Description }.Where(x => !string.IsNullOrWhiteSpace(x)));
                    list.Add(line);
                }
            }
            else if (fileName.ToLowerInvariant().EndsWith(".iif"))
            {
                System.IO.StreamReader r = new System.IO.StreamReader(new MemoryStream(buffer));

                int? dateIndex1 = null;
                int? payee1 = null;
                int? description1 = null;
                int? amount1 = null;
                int? dateIndex2 = null;
                int? payee2 = null;
                int? description2 = null;
                int? amount2 = null;

                var delimiter = '\t';
                if (!r.ReadToEnd().Contains(delimiter)) delimiter = ',';
                r.BaseStream.Position = 0;

                while (!r.EndOfStream)
                {
                    var header = r.ReadLine().Split(delimiter).Select(x => x.ToLowerInvariant().Trim()).ToArray();
                    if (header[0] == "!trns")
                    {
                        for (int i = 0; i < header.Length; i++)
                        {
                            if (header[i] == "date") dateIndex1 = i;
                            if (header[i] == "name") payee1 = i;
                            if (header[i] == "memo") description1 = i;
                            if (header[i] == "amount") amount1 = i;
                        }
                    }
                    if (header[0] == "!spl")
                    {
                        for (int i = 0; i < header.Length; i++)
                        {
                            if (header[i] == "date") dateIndex2 = i;
                            if (header[i] == "name") payee2 = i;
                            if (header[i] == "memo") description2 = i;
                            if (header[i] == "amount") amount2 = i;
                        }
                    }
                    if (header[0] == "!endtrns") break;
                }

                if (!dateIndex1.HasValue) throw new ArgumentException(@"IIF file is missing ""Date"" column");
                //if (!description1.HasValue) throw new ArgumentException(@"IIF file is missing ""Description"" column");
                if (!amount1.HasValue) throw new ArgumentException(@"IIF file is missing ""Amount"" column");
                if (!dateIndex2.HasValue) throw new ArgumentException(@"IIF file is missing ""Date"" column");
                //if (!description2.HasValue) throw new ArgumentException(@"IIF file is missing ""Description"" column");
                if (!amount2.HasValue) throw new ArgumentException(@"IIF file is missing ""Amount"" column");

                var innerList = new List<RawBankStatementLine>();
                var o = new RawBankStatementLine();
                while (!r.EndOfStream)
                {
                    string line = r.ReadLine();
                    if (line == null) continue;
                    var values = line.Split(delimiter).ToArray();
                    for (int i = 0; i < values.Length; i++)
                    {
                        if (values[i].StartsWith(@"""") && values[i].EndsWith(@"""")) values[i] = values[i].Substring(1, values[i].Length - 2);
                    }
                    if (values[0] == "TRNS")
                    {
                        o.Date = values[dateIndex1.Value] ?? string.Empty;
                        o.Date = o.Date.Replace('.', '/');
                        o.Date = o.Date.Replace('-', '/');

                        var parsedAmount = 0m;
                        if (decimal.TryParse(values[amount1.Value], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out parsedAmount))
                        {
                            o.Amount = parsedAmount;
                        }

                        if (description1.HasValue && values.Length > description1.Value) o.Description = values[description1.Value];
                        if (payee1.HasValue) o.Description += " " + values[payee1.Value];
                    }
                    if (values[0] == "SPL")
                    {
                        var lineItem = new RawBankStatementLine.LineItem();

                        var parsedAmount = 0m;
                        if (decimal.TryParse(values[amount2.Value], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out parsedAmount))
                        {
                            lineItem.Amount = parsedAmount;
                        }

                        lineItem.Amount *= -1;
                        if (payee2.HasValue && values.Length > payee2.Value) lineItem.Description = values[payee2.Value];
                        if (description1.HasValue && values.Length > description2.Value) lineItem.Description += " " + values[description2.Value];
                        lineItem.Description = (lineItem.Description ?? string.Empty).Trim();
                        o.LineItems.Add(lineItem);
                    }
                    if (values[0] == "ENDTRNS")
                    {
                        innerList.Add(o);
                        o = new RawBankStatementLine();
                    }
                }

                if (innerList.Count == 0) throw new ArgumentException("File you are trying to upload appears to be empty.");

                bool usFormat = innerList.All(x => int.Parse(x.Date.Split('/')[0]) <= 12) && innerList.All(x => new DateTime(int.Parse(x.Date.Split('/')[2]), int.Parse(x.Date.Split('/')[0]), int.Parse(x.Date.Split('/')[1])) <= DateTime.Today);
                bool gbFormat = innerList.All(x => int.Parse(x.Date.Split('/')[1]) <= 12) && innerList.All(x => new DateTime(int.Parse(x.Date.Split('/')[2]), int.Parse(x.Date.Split('/')[1]), int.Parse(x.Date.Split('/')[0])) <= DateTime.Today);

                if (usFormat && gbFormat) throw new ArgumentException("File you are trying to upload has ambiguous date format.");
                if (!usFormat && !gbFormat) throw new ArgumentException("File you are trying to upload appears to be corrupted.");

                foreach (var e in innerList)
                {
                    DateTime date = default(DateTime);
                    var dateElements = e.Date.Split('/');
                    var year = int.Parse(dateElements[2]);
                    if (year < 100) year += 2000;
                    if (usFormat)
                    {
                        date = new DateTime(year, int.Parse(dateElements[0]), int.Parse(dateElements[1]));
                    }
                    if (gbFormat)
                    {
                        date = new DateTime(year, int.Parse(dateElements[1]), int.Parse(dateElements[0]));
                    }
                    if (e.Amount == 0) continue;
                    var line = new ParsedBankStatementLine();
                    line.Date = date;
                    if (e.LineItems != null && e.LineItems.Any())
                    {
                        line.LineItems = e.LineItems.Select(x => new ParsedBankStatementLine.LineItem() { Amount = x.Amount, Description = x.Description }).ToArray();
                    }
                    else
                    {
                        line.LineItems = new ParsedBankStatementLine.LineItem[] { new ParsedBankStatementLine.LineItem() { Amount = e.Amount } };
                    }
                    line.Description = e.Description;
                    list.Add(line);
                }
            }
            else
            {
                throw new ArgumentException("File format not supported. Only .qif, .ofx, .qfx, .sta, .xml or .camt053 formats can be accepted.");
            }
            return list.ToArray();
        }

        public bool IsUsFormat(string[] dates)
        {
            var usFormat = dates.All(x => int.Parse(x.Split('/')[0]) <= 12);
            var gbFormat = dates.All(x => int.Parse(x.Split('/')[1]) <= 12);
            //var isoFormat = dates.All(x => x.Split('/')[0].Length == 4 && int.Parse(x.Split('/')[1]) <= 12 && int.Parse(x.Split('/')[2]) <= 31);

            /*
            if (!usFormat && !gbFormat && !isoFormat)
            {
                throw new ArgumentException("File you are trying to upload appears to be corrupted.");
            }
            */

            /*
            if (isoFormat)
            {
                usFormat = false;
                gbFormat = false;
            }
            */

            if (usFormat && gbFormat)
            {
                var usDates = dates.Select(x => int.Parse(x.Split('/')[2] + x.Split('/')[0] + x.Split('/')[1])).ToArray();
                var gbDates = dates.Select(x => int.Parse(x.Split('/')[2] + x.Split('/')[1] + x.Split('/')[0])).ToArray();

                if (usFormat && gbFormat)
                {
                    var today = int.Parse(DateTime.Today.ToString("yyyyMMdd"));
                    if (gbDates.Max() > today) gbFormat = false;
                    if (usDates.Max() > today) usFormat = false;
                }

                if (usFormat && gbFormat)
                {
                    usFormat = usDates.OrderByDescending(x => x).SequenceEqual(usDates) || usDates.OrderBy(x => x).SequenceEqual(usDates);
                    gbFormat = gbDates.OrderByDescending(x => x).SequenceEqual(gbDates) || gbDates.OrderBy(x => x).SequenceEqual(gbDates);
                }

                if (usFormat && gbFormat)
                {
                    if (usDates.SequenceEqual(gbDates)) usFormat = false;
                }

                if (usFormat && gbFormat)
                {
                    if (gbDates.Max() > usDates.Max()) usFormat = false;
                    if (usDates.Max() > gbDates.Max()) gbFormat = false;
                }

                if (usFormat && gbFormat)
                {
                    if (gbDates.Min() > usDates.Min()) usFormat = false;
                    if (usDates.Min() > gbDates.Min()) gbFormat = false;
                }
            }

            if (usFormat && gbFormat)
            {
                throw new ArgumentException("File you are trying to upload has ambiguous date format.");
            }

            if (usFormat) return true;
            if (gbFormat) return false;

            throw new ArgumentException("File you are trying to upload appears to be corrupted.");
        }

        private string GetYear(string s)
        {
            if (s.Length == 2) return $"20{s}";
            return s;
        }

        public enum DateFormat
        {
            UsFormat,
            GbFormat,
            Both
        }

        public sealed class Camt053
        {
            public DocumentElement Document { get; set; }
            public sealed class DocumentElement
            {
                public BkToCstmrStmtElement BkToCstmrStmt { get; set; }
                public sealed class BkToCstmrStmtElement
                {
                    [Newtonsoft.Json.JsonConverter(typeof(CustomArrayConverter<StmtElement>))]
                    public StmtElement[] Stmt { get; set; }
                    public sealed class StmtElement
                    {
                        [Newtonsoft.Json.JsonConverter(typeof(CustomArrayConverter<NtryElement>))]
                        public NtryElement[] Ntry { get; set; }
                        public sealed class NtryElement
                        {
                            public string CdtDbtInd { get; set; }
                            public string AddtlNtryInf { get; set; }

                            public AmtElement Amt { get; set; }
                            public sealed class AmtElement
                            {
                                [Newtonsoft.Json.JsonProperty("@Ccy")] public string Ccy { get; set; }
                                [Newtonsoft.Json.JsonProperty("#text")] public string text { get; set; }
                            }

                            public BookgDtElement BookgDt { get; set; }
                            public sealed class BookgDtElement
                            {
                                public string Dt { get; set; }
                                public string DtTm { get; set; }

                                public DateTime? GetDate()
                                {
                                    if (!string.IsNullOrWhiteSpace(Dt)) return DateTime.Parse(Dt, CultureInfo.InvariantCulture).Date;
                                    if (!string.IsNullOrWhiteSpace(DtTm)) return DateTime.Parse(DtTm, CultureInfo.InvariantCulture).Date;
                                    return null;
                                }
                            }

                            [Newtonsoft.Json.JsonConverter(typeof(CustomArrayConverter<NtryDtlsElement>))]
                            public NtryDtlsElement[] NtryDtls { get; set; }
                            public sealed class NtryDtlsElement
                            {
                                [Newtonsoft.Json.JsonConverter(typeof(CustomArrayConverter<TxDtlsElement>))]
                                public TxDtlsElement[] TxDtls { get; set; }
                                public sealed class TxDtlsElement
                                {
                                    public RmtInfElement RmtInf { get; set; }
                                    public sealed class RmtInfElement
                                    {
                                        [Newtonsoft.Json.JsonConverter(typeof(CustomArrayConverter<string>))]
                                        public string[] Ustrd { get; set; }
                                    }

                                    public RltdPtiesElement RltdPties { get; set; }
                                    public sealed class RltdPtiesElement
                                    {
                                        public DbtrElement Dbtr { get; set; }
                                        public sealed class DbtrElement
                                        {
                                            public string Nm { get; set; }
                                        }

                                        public CdtrElement Cdtr { get; set; }
                                        public sealed class CdtrElement
                                        {
                                            public string Nm { get; set; }
                                        }
                                    }
                                }
                            }

                            public BkTxCdElement BkTxCd { get; set; }
                            public sealed class BkTxCdElement
                            {
                                public PrtryElement Prtry { get; set; }
                                public sealed class PrtryElement
                                {
                                    public string Cd { get; set; }
                                }
                            }

                            public string GetDescription()
                            {
                                var parts = new List<string>();
                                parts.Add(AddtlNtryInf);
                                var txDtls = NtryDtls?.SelectMany(x => x.TxDtls ?? new NtryDtlsElement.TxDtlsElement[0]);
                                if (txDtls != null)
                                {
                                    foreach (var tx in txDtls)
                                    {
                                        parts.Add(tx.RltdPties?.Dbtr?.Nm);
                                        parts.Add(tx.RltdPties?.Cdtr?.Nm);
                                        parts.AddRange(tx.RmtInf?.Ustrd ?? new string[0]);
                                    }
                                }
                                parts.Add(BkTxCd?.Prtry?.Cd);

                                parts = parts.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().Select(x => x.Trim()).OrderByDescending(x => x.Length).ToList();

                                var description = new System.Text.StringBuilder();
                                foreach (var e in parts.ToArray())
                                {
                                    if (description.ToString().Contains(e)) continue;
                                    if (description.Length > 0) description.Append(' ');
                                    description.Append(e);
                                }

                                return description.ToString();
                            }
                        }
                    }
                }
            }
        }

        public sealed class Camt052
        {
            public DocumentElement Document { get; set; }
            public sealed class DocumentElement
            {
                public BkToCstmrAcctRptElement BkToCstmrAcctRpt { get; set; }
                public sealed class BkToCstmrAcctRptElement
                {
                    [Newtonsoft.Json.JsonConverter(typeof(CustomArrayConverter<RptElement>))]
                    public RptElement[] Rpt { get; set; }
                    public sealed class RptElement
                    {
                        [Newtonsoft.Json.JsonConverter(typeof(CustomArrayConverter<NtryElement>))]
                        public NtryElement[] Ntry { get; set; }
                        public sealed class NtryElement
                        {
                            public string CdtDbtInd { get; set; }
                            public string AddtlNtryInf { get; set; }

                            public AmtElement Amt { get; set; }
                            public sealed class AmtElement
                            {
                                [Newtonsoft.Json.JsonProperty("@Ccy")] public string Ccy { get; set; }
                                [Newtonsoft.Json.JsonProperty("#text")] public string text { get; set; }
                            }

                            public BookgDtElement BookgDt { get; set; }
                            public sealed class BookgDtElement
                            {
                                public string Dt { get; set; }
                                public string DtTm { get; set; }

                                public DateTime? GetDate()
                                {
                                    if (!string.IsNullOrWhiteSpace(Dt)) return DateTime.Parse(Dt, CultureInfo.InvariantCulture).Date;
                                    if (!string.IsNullOrWhiteSpace(DtTm)) return DateTime.Parse(DtTm, CultureInfo.InvariantCulture).Date;
                                    return null;
                                }
                            }

                            [Newtonsoft.Json.JsonConverter(typeof(CustomArrayConverter<NtryDtlsElement>))]
                            public NtryDtlsElement[] NtryDtls { get; set; }
                            public sealed class NtryDtlsElement
                            {
                                [Newtonsoft.Json.JsonConverter(typeof(CustomArrayConverter<TxDtlsElement>))]
                                public TxDtlsElement[] TxDtls { get; set; }
                                public sealed class TxDtlsElement
                                {
                                    public RmtInfElement RmtInf { get; set; }
                                    public sealed class RmtInfElement
                                    {
                                        [Newtonsoft.Json.JsonConverter(typeof(CustomArrayConverter<string>))]
                                        public string[] Ustrd { get; set; }
                                    }

                                    public RltdPtiesElement RltdPties { get; set; }
                                    public sealed class RltdPtiesElement
                                    {
                                        public DbtrElement Dbtr { get; set; }
                                        public sealed class DbtrElement
                                        {
                                            public string Nm { get; set; }
                                        }

                                        public CdtrElement Cdtr { get; set; }
                                        public sealed class CdtrElement
                                        {
                                            public string Nm { get; set; }
                                        }
                                    }
                                }
                            }

                            public BkTxCdElement BkTxCd { get; set; }
                            public sealed class BkTxCdElement
                            {
                                public PrtryElement Prtry { get; set; }
                                public sealed class PrtryElement
                                {
                                    public string Cd { get; set; }
                                }
                            }

                            public string GetDescription()
                            {
                                var parts = new List<string>();
                                parts.Add(AddtlNtryInf);
                                var txDtls = NtryDtls?.SelectMany(x => x.TxDtls ?? new NtryDtlsElement.TxDtlsElement[0]);
                                if (txDtls != null)
                                {
                                    foreach (var tx in txDtls)
                                    {
                                        parts.Add(tx.RltdPties?.Dbtr?.Nm);
                                        parts.Add(tx.RltdPties?.Cdtr?.Nm);
                                        parts.AddRange(tx.RmtInf?.Ustrd ?? new string[0]);
                                    }
                                }
                                parts.Add(BkTxCd?.Prtry?.Cd);

                                parts = parts.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().Select(x => x.Trim()).OrderByDescending(x => x.Length).ToList();

                                var description = new System.Text.StringBuilder();
                                foreach (var e in parts.ToArray())
                                {
                                    if (description.ToString().Contains(e)) continue;
                                    if (description.Length > 0) description.Append(' ');
                                    description.Append(e);
                                }

                                return description.ToString();
                            }
                        }
                    }
                }
            }
        }

        public sealed class CsvLine
        {
            [Optional]
            [Name("Reference", "#", "Bank Reference Number", "id")]
            public string Reference { get; set; }

            [Name("Date", "DATE", "Statement Date", "Transaction Date", "TransactionDate", "Trade Date", "Payment Date", "Created (UTC)", "Transaction date", "transaction_date")]
            public string Date { get; set; }

            [Optional]
            [Name("Payee", "Name")]
            public string Contact { get; set; }

            [Optional]            
            [Name("Description", "details", "DESCRIPTION", "Extended Text", "Comment", "Product name", "Investment", "Transaction ID", "Counter Party", "Details", "Transaction Details")]
            public string Description { get; set; }

            [Optional]
            [Name("Units", "units", "Quantity")]
            public string Quantity { get; set; }

            [Name("Amount", "AMOUNT", "amount", "Credit", "Credits", "Credit Amount", "Net", "In amount(R)", "Total", "Value", "Deposited", "Amount (GBP)", "Amount ($)", "Transaction Amount")]
            public string Amount { get; set; }

            [Optional]
            [Name("Debit", "Debits", "Debit Amount", "Out amount", "Fee amount")]
            public string Debit { get; set; }

            [Optional]
            [Name("Money in/out", "Type")]
            [TypeConverter(typeof(NullableBooleanConverter))]
            public bool? IsDeposit { get; set; }

            [Optional]
            [Name("Fee amount")]
            public string Debit2 { get; set; }
        }

        internal class CustomArrayConverter<T> : Newtonsoft.Json.JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return (objectType == typeof(T[]));
            }

            public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
            {
                var token = Newtonsoft.Json.Linq.JToken.Load(reader);
                if (token.Type == Newtonsoft.Json.Linq.JTokenType.Null || token.Type == Newtonsoft.Json.Linq.JTokenType.None)
                {
                    return new T[0];
                }
                if (token.Type == Newtonsoft.Json.Linq.JTokenType.Array)
                {
                    return token.ToObject<T[]>();
                }
                return new T[] { token.ToObject<T>() };
            }

            public override bool CanWrite
            {
                get
                {
                    return false;
                }
            }

            public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }

        public class NullableBooleanConverter : DefaultTypeConverter
        {
            public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
            {
                if (string.IsNullOrWhiteSpace(text)) return null;

                var trueValues = new[] { "Money In", "C" };
                var falseValues = new[] { "Money Out", "D" };

                if (Array.Exists(trueValues, v => v.Equals(text, StringComparison.OrdinalIgnoreCase)))
                {
                    return true;
                }

                if (Array.Exists(falseValues, v => v.Equals(text, StringComparison.OrdinalIgnoreCase)))
                {
                    return false;
                }

                // If the value doesn't match, return null
                return null;
            }
        }
    }
}
