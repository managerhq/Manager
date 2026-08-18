using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using ManagerServer.Model.Attributes;
using ManagerServer.Globalization;
using static ManagerServer.Model.Attributes.ExpressionAttribute.Operators;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [CustomFields]
    [ProtoContract]
    [Guid("1d103fa7-6fc1-4951-811e-972968b842cc")]
    [Currency(nameof(employee))]
    public sealed class Payslip : Transaction, IHasAutomaticReference, IRecurringTransactionDestination, IComparable<Payslip>, ICustomFields, IForeignCurrencyTransaction, ICode, IHasCustomTheme
    {
        [Guide("Enter the date for this payslip. This determines the pay period and when wages are earned.")]
        [ProtoMember(1), NoWrap] public DateTime Date { get; set; }
        [Guide("Enter a reference number for this payslip. This could be a payroll number or pay period identifier.")]
        [ProtoMember(12)] public string Reference { get; set; }
        [Guide("Optionally, add a description for this payslip, such as pay period details or special notes.")]
        [ProtoMember(11), Long, Placeholder(nameof(Strings.Optional))] public string description { get; set; }
        [Guide("Select the employee receiving this payslip. The employee's settings determine currency and default pay rates.")]
        [ProtoMember(2), Autocomplete(typeof(Employee))] public Guid? employee { get; set; }
        [Guide("If the employee is paid in a foreign currency, enter the exchange rate to convert to base currency.")]
        [ProtoMember(21), Placeholder(nameof(Strings.Autofill)), NoWrap, IfNotNull(nameof(employee), nameof(Employee.Currency)), Prepend("1 {{ (ExchangeRateIsInverse ? baseCurrency.code : getCurrencyCode()) }} = "), Append("{{ (ExchangeRateIsInverse ? getCurrencyCode() : baseCurrency.code) }}")] public decimal ExchangeRate { get; set; }
        [ProtoMember(22), IfNotNull(nameof(employee), nameof(Employee.Currency)), Icon("fa-right-left")] public bool ExchangeRateIsInverse { get; set; }
        [Guide("Enter the employee's earnings for this pay period. Each line represents different types of wages or benefits.")]
        [ProtoMember(3), FirstColumnLabel] public Earned[] Earnings { get; set; }
        [Guide("Enter any deductions from the employee's pay, such as taxes, insurance, or loan repayments.")]
        [ProtoMember(4), FirstColumnLabel] public Deduction[] Deductions { get; set; }
        [Guide("Enter employer contributions that are not deducted from employee pay, such as employer pension contributions.")]
        [ProtoMember(5), FirstColumnLabel] public Contribution[] Contributions { get; set; }
        [Guide("Check this box to show year-to-date or period-to-date totals on the payslip for transparency.")]
        [ProtoMember(14)] public bool ShowTotalsForThePeriod { get; set; }
        [Guide("If showing period totals, enter the start date of the period for which totals should be calculated.")]
        [ProtoMember(15), IfTrue(nameof(ShowTotalsForThePeriod)), NoLabel, Prepend(nameof(Strings.FromDate))] public DateTime? TotalsPeriodStart { get; set; }
        [ProtoMember(9), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(10), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }
        [ProtoMember(13), DoNotCopy] public bool AutomaticReference { get; set; }
        [ProtoMember(19), Label(nameof(Strings.CustomTitle))] public bool HasPayslipCustomTitle { get; set; }
        [ProtoMember(20), NoLabel, IfTrue(nameof(HasPayslipCustomTitle)), Placeholder(nameof(Strings.Payslip))] public string PayslipCustomTitle { get; set; }
        [ProtoMember(16), Label(nameof(Strings.Footers))] public bool HasPayslipFooters { get; set; }
        [ProtoMember(17), Autocomplete(typeof(ManagerServer.Model.PayslipFooter)), NoLabel, IfTrue(nameof(HasPayslipFooters))] public Guid[] PayslipFooters { get; set; }
        [ProtoMember(8)] public Dictionary<Guid, string> CustomFields { get; set; }
        [ProtoMember(18)] public CustomFields CustomFields2 { get; set; }

        [ProtoMember(6)] public string Obsolete_Notes { get; set; }
        [ProtoMember(7)] public Guid? Obsolete_TrackingCode { get; set; }

        string IHasAutomaticReference.Reference { get => Reference; set => Reference = value; }
        bool IHasAutomaticReference.AutomaticReference { get => AutomaticReference; set => AutomaticReference = value; }
        DateTime IRecurringTransactionDestination.Date { get => Date; set => Date = value; }
        Dictionary<Guid, string> ICustomFields.ClassicCustomFields => CustomFields;
        CustomFields ICustomFields.CustomFields => CustomFields2;
        DateTime IForeignCurrencyTransaction.Date => Date;
        Guid? IForeignCurrencyTransaction.Currency => employee;
        decimal IForeignCurrencyTransaction.ExchangeRate { get => ExchangeRate; set => ExchangeRate = value; }
        bool IForeignCurrencyTransaction.ExchangeRateIsInverse { get => ExchangeRateIsInverse; set => ExchangeRateIsInverse = value; }
        string ICode.Code => Reference;

        public override string GetReference() => Reference;

        [ProtoContract]
        public sealed class Earned
        {
            [Guide("Select the type of earnings, such as regular pay, overtime, or bonus. This determines the account posting.")]
            [ProtoMember(4), Autocomplete(typeof(PayslipEarningsItem))] public Guid? Item { get; set; }
            [Guide("Optionally, add a description for this earning, such as 'Overtime for weekend shift' or 'Performance bonus'.")]
            [ProtoMember(6), Placeholder(nameof(Strings.Optional))] public string Description { get; set; }
            [Guide("Enter the number of units (usually hours) for this earning. Leave blank if the earning is a fixed amount.")]
            [ProtoMember(7)] public decimal? Units { get; set; }
            [Guide("Enter the rate per unit (usually hourly rate) or the fixed amount if units are not used.")]
            [ProtoMember(3)] public decimal UnitPrice { get; set; }
            [Expression(Zero, Plus, nameof(UnitPrice), Times, nameof(Units), Round), Sum, AppendCurrency(nameof(Payslip.employee)), Label(nameof(Strings.Amount))] public object EarningsAmount { get; }
            [Guide("Optionally assign this earning to a specific division for divisional reporting and cost allocation.")]
            [ProtoMember(5), Autocomplete(typeof(Division)), Short] public Guid? Division { get; set; }
            [Guide("Optionally assign this earning to a specific project for project cost tracking.")]
            [ProtoMember(8), Autocomplete(typeof(Project)), Short] public Guid? Project { get; set; }

            [ProtoMember(2)] public decimal? Obsolete_Units { get; set; }
            [ProtoMember(1)] public string Obsolete_Description { get; set; }
        }

        [ProtoContract]
        public sealed class Deduction
        {
            [Guide("Select the type of deduction, such as income tax, social security, or health insurance.")]
            [ProtoMember(3), Autocomplete(typeof(PayslipDeductionItem))] public Guid? Item { get; set; }
            [Guide("Optionally, add a description for this deduction, such as 'Federal income tax' or 'Loan repayment - Car'.")]
            [ProtoMember(4), Placeholder(nameof(Strings.Optional))] public string Description { get; set; }
            [Guide("Enter the amount to deduct from the employee's gross pay. This reduces their net pay.")]
            [ProtoMember(2), Sum, AppendCurrency(nameof(Payslip.employee)), Label(nameof(Strings.Amount))] public decimal DeductionAmount { get; set; }
            [Guide("Optionally assign this deduction to a specific division for reporting purposes.")]
            [ProtoMember(5), Autocomplete(typeof(Division)), Short] public Guid? Division { get; set; }

            [ProtoMember(1)] public string Obsolete_Description { get; set; }
        }

        [ProtoContract]
        public sealed class Contribution
        {
            [Guide("Select the type of employer contribution, such as pension matching or employer-paid insurance.")]
            [ProtoMember(3), Autocomplete(typeof(PayslipContributionItem))] public Guid? Item { get; set; }
            [Guide("Optionally, add a description for this contribution, such as 'Employer 401k match' or 'Employer health insurance'.")]
            [ProtoMember(5), Placeholder(nameof(Strings.Optional))] public string Description { get; set; }
            [Guide("Enter the amount of the employer contribution. This is paid by the employer, not deducted from employee.")]
            [ProtoMember(2), Sum, AppendCurrency(nameof(Payslip.employee)), Label(nameof(Strings.Amount))] public decimal ContributionAmount { get; set; }
            [Guide("Optionally assign this contribution to a specific division for cost allocation.")]
            [ProtoMember(4), Autocomplete(typeof(Division)), Short] public Guid? Division { get; set; }

            [ProtoMember(1)] public string Obsolete_Description { get; set; }
        }

        public override string GetDescriptionOrNull()
        {
            if (!string.IsNullOrWhiteSpace(description)) return description;
            return null;
        }

        public override string GetName()
        {
            return Reference;
        }

        public override bool IsGeneralLedgerTransaction()
        {
            return true;
        }

        int IComparable<Payslip>.CompareTo(Payslip other)
        {
            return (other.Date, other.Reference).CompareTo((Date, Reference));
        }

        public override ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] CreateGeneralLedgerTransactions(Database database)
        {
            var employee = database.SingleOrDefault<Employee>(this.employee);
            if (employee == null) return [];

            var trackingCode = database.SingleOrDefault<Division>(employee.Division);

            var baseCurrency = database.Single<BaseCurrency>();
            Currency currency = database.SingleOrDefault<ForeignCurrency>(employee.Currency);
            if (currency == null) currency = baseCurrency;

            var list = new List<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction>();
            if (Earnings != null)
            {
                foreach (var e in Earnings)
                {
                    var total = currency.Round((e.Units ?? 1m) * e.UnitPrice);
                    if (total != 0m)
                    {
                        IGeneralLedgerAccount generalLedgerAccount = database.Single<BalanceSheetSuspenseAccount>();
                        var payslipEarningsItem = database.SingleOrDefault<PayslipEarningsItem>(e.Item);
                        if (payslipEarningsItem != null)
                        {
                            var profitAndLossAccount = database.SingleOrDefault<ProfitAndLossStatementAccount>(payslipEarningsItem.ExpenseAccount);
                            if (profitAndLossAccount != null) generalLedgerAccount = profitAndLossAccount;
                        }

                        var baseTotal = currency.GetBaseAmount(total, ExchangeRate, ExchangeRateIsInverse, baseCurrency);

                        list.Add(new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                            database: database,
                            transaction: this,
                            date: Date,
                            generalLedgerAccount: generalLedgerAccount,
                            employee: employee,
                            transactionAmount: total,
                            baseAmount: baseTotal,
                            exchangeRate: ExchangeRate,
                            isExchangeRateInverse: ExchangeRateIsInverse,
                            trackingCode: database.SingleOrDefault<Division>(e.Division),
                            project: database.SingleOrDefault<Project>(e.Project),
                            transactionCurrency: currency,
                            payslipEarningsItem: payslipEarningsItem,
                            isPayslipEarningsLine: true,
                            reportingCategory: payslipEarningsItem?.ReportingCategory
                        ));
                    }
                }
            }
            if (Deductions != null)
            {
                foreach (var e in Deductions)
                {
                    if (e.DeductionAmount != 0m)
                    {
                        var deductionAmount = currency.Round(e.DeductionAmount);
                        var baseDeductionAmount = currency.GetBaseAmount(deductionAmount, ExchangeRate, ExchangeRateIsInverse, baseCurrency);

                        IGeneralLedgerAccount generalLedgerAccount = database.Single<BalanceSheetSuspenseAccount>();
                        var payslipDeductionItem = database.SingleOrDefault<PayslipDeductionItem>(e.Item);
                        if (payslipDeductionItem != null)
                        {
                            var balanceSheetAccount = database.SingleOrDefault<BalanceSheetAccount>(payslipDeductionItem.Account);
                            var profitAndLossStatementAccount = database.SingleOrDefault<ProfitAndLossStatementAccount>(payslipDeductionItem.Account);
                            if (balanceSheetAccount != null) generalLedgerAccount = balanceSheetAccount;
                            if (profitAndLossStatementAccount != null) generalLedgerAccount = profitAndLossStatementAccount;
                        }

                        list.Add(new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                            database: database,
                            transaction: this,
                            date: Date,
                            generalLedgerAccount: generalLedgerAccount,
                            employee: employee,
                            transactionAmount: deductionAmount * -1m,
                            baseAmount: baseDeductionAmount * -1m,
                            exchangeRate: ExchangeRate,
                            isExchangeRateInverse: ExchangeRateIsInverse,
                            transactionCurrency: currency,
                            payslipDeductionItem: payslipDeductionItem,
                            isPayslipDeductionLine: true,
                            reportingCategory: payslipDeductionItem?.ReportingCategory,
                            trackingCode: database.SingleOrDefault<Division>(e.Division)
                        ));
                    }
                }
            }

            list.Add(new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                database: database,
                transaction: this,
                date: Date,
                generalLedgerAccount: database.Single<BalanceSheetEmployeeClearingAccount>(),
                employee: employee,
                transactionAmount: list.Sum(x => x.TransactionAmount) * -1m,
                baseAmount: list.Sum(x => x.BaseAmount) * -1m,
                isBalancing: true,
                transactionCurrency: currency,
                trackingCode: trackingCode
            ));

            if (Contributions != null)
            {
                foreach (var e in Contributions)
                {
                    if (e.ContributionAmount != 0m)
                    {
                        var contributionAmount = currency.Round(e.ContributionAmount);
                        var baseContributionAmount = currency.GetBaseAmount(contributionAmount, ExchangeRate, ExchangeRateIsInverse, baseCurrency);

                        IGeneralLedgerAccount expenseAccount = database.Single<BalanceSheetSuspenseAccount>();
                        IGeneralLedgerAccount liabilityAccount = database.Single<BalanceSheetSuspenseAccount>();

                        var payslipContributionItem = database.SingleOrDefault<PayslipContributionItem>(e.Item);
                        if (payslipContributionItem != null)
                        {
                            var balanceSheetAccount = database.SingleOrDefault<BalanceSheetAccount>(payslipContributionItem.LiabilityAccount);
                            if (balanceSheetAccount != null) liabilityAccount = balanceSheetAccount;
                            var profitAndLossAccount = database.SingleOrDefault<ProfitAndLossStatementAccount>(payslipContributionItem.ExpenseAccount);
                            if (profitAndLossAccount != null) expenseAccount = profitAndLossAccount;
                        }

                        list.Add(new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                            database: database,
                            transaction: this,
                            date: Date,
                            generalLedgerAccount: liabilityAccount,
                            employee: employee,
                            transactionAmount: contributionAmount * -1m,
                            baseAmount: baseContributionAmount * -1m,
                            exchangeRate: ExchangeRate,
                            isExchangeRateInverse: ExchangeRateIsInverse,
                            transactionCurrency: currency,
                            trackingCode: trackingCode
                        ));

                        list.Add(new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                            database: database,
                            transaction: this,
                            date: Date,
                            generalLedgerAccount: expenseAccount,
                            employee: employee,
                            transactionAmount: contributionAmount,
                            baseAmount: baseContributionAmount,
                            exchangeRate: ExchangeRate,
                            isExchangeRateInverse: ExchangeRateIsInverse,
                            trackingCode: database.SingleOrDefault<Division>(e.Division),
                            transactionCurrency: currency,
                            payslipContributionItem: payslipContributionItem,
                            isPayslipContributionLine: true,
                            reportingCategory: payslipContributionItem?.ReportingCategory
                        ));
                    }
                }
            }

            return list.ToArray();
        }
    }
}
