using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagerServer.Query.GeneralLedger
{
    public sealed class GeneralLedgerTransactionContainer
    {
        private GeneralLedgerTransaction[] GeneralLedgerTransactionLines;
        private Lazy<HashSet<Guid>> ForeignKeys;

        public Lazy<DateTime> MaxDate { get; init; }
        public Lazy<bool> ContainsGeneralLedgerTransaction { get; init; }
        public Lazy<bool> ContainsInventoryOnHandTransaction { get; init; }

        public GeneralLedgerTransactionContainer(params IEnumerable<GeneralLedgerTransaction> generalLedgerTransactionLines)
        {
            GeneralLedgerTransactionLines = generalLedgerTransactionLines?.ToArray() ?? [];
            ForeignKeys = new Lazy<HashSet<Guid>>(() =>
            {
                return [.. GeneralLedgerTransactionLines
                    .SelectMany(x => x.GetKeys())
                    .Distinct()
                    .Where(x => x != Guid.Empty)
                    ];
            });

            MaxDate = new Lazy<DateTime>(() =>
            {
                if (GeneralLedgerTransactionLines.Length == 0) return DateTime.MinValue;
                return GeneralLedgerTransactionLines.Max(x => x.Date);
            });

            ContainsGeneralLedgerTransaction = new Lazy<bool>(() =>
            {
                if (GeneralLedgerTransactionLines.Length == 0) return false;
                return GeneralLedgerTransactionLines.Any(x => x.Transaction.IsGeneralLedgerTransaction());
            });

            ContainsInventoryOnHandTransaction = new Lazy<bool>(() =>
            {
                if (GeneralLedgerTransactionLines.Length == 0) return false;
                return GeneralLedgerTransactionLines.Any(x => x.GeneralLedgerAccount.IsInventoryOnHand);
            });
        }

        public GeneralLedgerTransaction[] GetLines()
        {
            return GeneralLedgerTransactionLines;
        }

        public bool ContainsForeignKey(Guid foreignKey)
        {
            return ForeignKeys.Value.Contains(foreignKey);
        }

        public bool ContainsAnyForeignKey(Guid[] foreignKeys)
        {
            var set = ForeignKeys.Value;
            return foreignKeys.Any(x => set.Contains(x));
        }
    }
}
