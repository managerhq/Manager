using ManagerServer.Model.Attributes;
using ProtoBuf;
using System;
using ManagerServer.Globalization;
using ManagerServer.Query.GeneralLedger;
using ManagerServer.Attributes;
using ManagerServer.Model.Enums;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("47074f31-7800-4cb9-8b69-ef9b01173e3e")]
    [Title(nameof(Strings.StartingBalance))]
    public sealed class EmployeeStartingBalance : ManagerServer.Model.Transaction
    {
        [Guide("Select the employee for whom you want to enter a starting balance. This list shows all employees you have created under the `Employees` tab.")]
        [ProtoMember(1), Autocomplete(typeof(Employee))] public Guid? Employee { get; set; }
        [Guide("Choose whether the starting balance is a debit or credit:")]
        [Guide("• `Debit` - The employee owes money to the business (e.g., salary advances, loans to employees)")]
        [Guide("• `Credit` - The business owes money to the employee (e.g., unpaid wages, expense reimbursements due)")]
        [ProtoMember(2), NoWrap, Label(nameof(Strings.StartingBalance))] public DebitCredit DebitCredit { get; set; }
        [Guide("Enter the starting balance amount. This represents the net amount owed to or from the employee as of your starting date in Manager.")]
        [ProtoMember(3), EmptyLabel, AppendCurrency(nameof(Employee))] public decimal StartingBalance { get; set; }

        public override string GetReference()
        {
            return string.Empty;
        }

        public override string GetName()
        {
            return null;
        }

        public override string GetDescriptionOrNull()
        {
            return null;
        }

        public override bool IsGeneralLedgerTransaction()
        {
            return true;
        }

        public override string TransactionTitle => Strings.StartingBalance;

        public override GeneralLedgerTransaction[] CreateGeneralLedgerTransactions(Database database)
        {
            GeneralLedgerTransaction transaction = null;

            var employee = database.SingleOrDefault<Employee>(Employee);

            if (employee != null)
            {
                var startingBalance = DebitCredit == DebitCredit.Debit ? StartingBalance : StartingBalance * -1m;
                var baseCurrency = database.Single<BaseCurrency>();
                var currency = (Currency)database.SingleOrDefault<ForeignCurrency>(employee.Currency) ?? database.Single<BaseCurrency>();
                var exchangeRate = database.Single<StartingExchangeRates>().GetExchangeRate(currency);
                var baseAmount = baseCurrency.GetBaseAmount(startingBalance, exchangeRate.ExchangeRate, exchangeRate.ExchangeRateIsInverse, currency);

                transaction = new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                    database: database,
                    date: DateTime.MinValue,
                    generalLedgerAccount: database.Single<BalanceSheetEmployeeClearingAccount>(),
                    transactionAmount: startingBalance,
                    transactionCurrency: currency,
                    transaction: this,
                    exchangeRate: exchangeRate.ExchangeRate,
                    isExchangeRateInverse: exchangeRate.ExchangeRateIsInverse,
                    baseAmount: baseAmount,
                    employee: employee
                );
            }

            if (transaction != null)
            {
                return
                [
                    transaction,
                    new GeneralLedgerTransaction(
                        database: database,
                        date: DateTime.MinValue,
                        generalLedgerAccount: database.Single<BalanceSheetRetainedEarningsAccount>(),
                        transactionAmount: transaction.TransactionAmount * -1m,
                        transactionCurrency: transaction.TransactionCurrency,
                        transaction: this,
                        exchangeRate: transaction.ExchangeRate,
                        isExchangeRateInverse: transaction.IsExchangeRateInverse,
                        baseAmount: -transaction.BaseAmount,
                        isBalancing: true
                    )
                ];
            }

            return [];
        }
    }
}