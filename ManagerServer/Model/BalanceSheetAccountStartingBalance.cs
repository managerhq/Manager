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
    [Guid("db53bb26-6d7d-4170-8373-7d09f9638960")]
    [Title(nameof(Strings.StartingBalance))]
    public sealed class BalanceSheetAccountStartingBalance : ManagerServer.Model.Transaction
    {
        [Guide("Select balance sheet account that you have created under `ChartOfAccounts`.")]
        [ProtoMember(1), Autocomplete(typeof(BalanceSheetAccount))] public Guid? BalanceSheetAccount { get; set; }
        [Guide("Select whether starting balance represents debit or credit amount. Typically, you select `Debit` for asset account and `Credit` for liability accounts.")]
        [ProtoMember(2), NoWrap, Label(nameof(Strings.StartingBalance))] public DebitCredit DebitCredit { get; set; }
        [Guide("Enter the starting balance amount for this account. This represents the account balance at the beginning of your accounting period in Manager.")]
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

            var balanceSheetAccount = database.SingleOrDefault<BalanceSheetAccount>(BalanceSheetAccount);

            if (balanceSheetAccount != null)
            {
                transaction = new ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction(
                    database: database,
                    date: DateTime.MinValue,
                    generalLedgerAccount: balanceSheetAccount,
                    transactionAmount: DebitCredit == DebitCredit.Debit ? StartingBalance : StartingBalance * -1m,
                    transactionCurrency: database.Single<BaseCurrency>(),
                    transaction: this
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