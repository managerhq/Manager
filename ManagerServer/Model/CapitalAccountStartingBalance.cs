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
    [Guid("c29b5c78-58d1-4678-9f38-ea0b8b21e4cf")]
    [Title(nameof(Strings.StartingBalance))]
    public sealed class CapitalAccountStartingBalance : ManagerServer.Model.Transaction
    {
        [Guide("Select capital account that you have created under `CapitalAccounts` tab.")]
        [ProtoMember(1), Autocomplete(typeof(CapitalAccount))] public Guid? CapitalAccount { get; set; }
        [Guide("Select whether starting balance represents debit or credit amount. Typically, select `Debit` if capital account represents asset on your `BalanceSheet` and select `Credit` if capital account represents liability on your `BalanceSheet`.")]
        [ProtoMember(2), NoWrap, Label(nameof(Strings.StartingBalance))] public DebitCredit DebitCredit { get; set; }
        [Guide("Enter the opening balance amount for this capital account. This represents the capital account balance at the beginning of your accounting period in Manager.")]
        [ProtoMember(3), EmptyLabel, AppendBaseCurrency] public decimal StartingBalance { get; set; }

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

            var capitalAccount = database.SingleOrDefault<CapitalAccount>(CapitalAccount);

            if (capitalAccount != null)
            {
                transaction = new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                    database: database,
                    date: DateTime.MinValue,
                    generalLedgerAccount: database.Single<BalanceSheetCapitalAccountsAccount>(),
                    transactionAmount: DebitCredit == DebitCredit.Debit ? StartingBalance : StartingBalance * -1m,
                    transactionCurrency: database.Single<BaseCurrency>(),
                    transaction: this,
                    capitalAccount: capitalAccount
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
                        isBalancing: true
                    )
                ];
            }

            return [];
        }
    }
}