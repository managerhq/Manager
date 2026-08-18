using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Query.GeneralLedger;

namespace ManagerServer.Model
{
    public abstract class Transaction : NamedObject
    {
        public GeneralLedgerTransaction[] GetGeneralLedgerTransactions(Database database)
        {
            return database.GetGeneralLedgerTransactions().GetGeneralLedgerTransactions(this) ?? [];
        }

        public bool CanBeRealizedCurrencyTransaction(Database database)
        {
            return GetGeneralLedgerTransactions(database)
                            .Where(x => x.AccountCurrency is ForeignCurrency).GroupBy(x => x.AccountCurrency)
                            .Any(x => x.Sum(x => x.AccountAmount) != 0m);
        }

        public abstract GeneralLedgerTransaction[] CreateGeneralLedgerTransactions(Database database);
        public abstract string GetDescriptionOrNull();

        public string GetNameAndDescription()
        {
            var name = GetTransactionName();
            var description = GetDescriptionOrNull();
            if (string.IsNullOrWhiteSpace(description)) return name;
            return name + " — " + description;
        }

        public abstract string GetReference();

        public virtual string TransactionTitle
        {
            get
            {
                return null;
            }
        }

        public virtual bool GetHasLineDescription() => true;
        public virtual ManagerServer.Model.Enums.DiscountType? GetLineDicountType() => null;
        public virtual bool HasLineQty() => true;
        public virtual bool HasLineUnitPrice() => true;
        public bool HasLineProject() => true;

        public string GetTransactionName()
        {
            var transactionTitle = TransactionTitle;
            if (string.IsNullOrWhiteSpace(transactionTitle)) transactionTitle = ManagerServer.Globalization.Strings.GetPropertyValue(this.GetType().Name);
            return transactionTitle + (!string.IsNullOrWhiteSpace(GetName()) ? " — " + GetName() : null);
        }

        public static long GetReferenceNumber(string reference)
        {
            if (string.IsNullOrWhiteSpace(reference)) return 0;
            var digits = string.Join(string.Empty, reference.Where(x => char.IsDigit(x)));
            if (long.TryParse(digits, out long result)) return result;
            return 0;
        }

        public decimal? CostOfSales(Database database)
        {
            if (!GetGeneralLedgerTransactions(database).Where(x => x.IsCostOfGoodsSold).Any()) return null;
            return GetGeneralLedgerTransactions(database).Where(x => x.IsCostOfGoodsSold && x.GeneralLedgerAccount.IsInventoryOnHand).Sum(x => x.BaseAmount)*-1m;
        }

        public abstract bool IsGeneralLedgerTransaction();
    }
}