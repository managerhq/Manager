using ManagerServer.Query.GeneralLedger;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ManagerServer
{
    public sealed class GeneralLedgerTransactions
    {
        private Database database;
        private ImmutableDictionary<Guid, ManagerServer.Query.GeneralLedger.GeneralLedgerTransactionContainer> generalLedgerTransactions;
        private GeneralLedgerAggregations aggregations = new GeneralLedgerAggregations();

        public ImmutableDictionary<Guid, ManagerServer.Query.GeneralLedger.GeneralLedgerTransactionContainer> GetAll()
        {
            return generalLedgerTransactions;
        }

        public GeneralLedgerAggregations GetAggregations()
        {
            return aggregations;
        }

        public GeneralLedgerTransactions(Database database, Dictionary<Guid, GeneralLedgerTransactionContainer> generalLedgerTransactions)
        {
            this.database = database;
            aggregations.Update(generalLedgerTransactions.Values.SelectMany(x => x.GetLines()), false);
            this.generalLedgerTransactions = generalLedgerTransactions.ToImmutableDictionary();
        }

        public ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction[] GetGeneralLedgerTransactions(ManagerServer.Model.Transaction transaction)
        {
            if (generalLedgerTransactions.TryGetValue(transaction.Key, out var value))
            {
                return value.GetLines();
            }
            return default;
        }

        public Guid[] GetTransactionsByForeignKey(Guid foreignKey)
        {
            return generalLedgerTransactions
                .AsParallel()
                .Where(x => x.Key != foreignKey)
                .Where(x => x.Value.ContainsForeignKey(foreignKey))
                .Select(x => x.Key)
                .ToArray();
        }

        public void Invalidate(Guid[] keys)
        {
            lock (database)
            {
                var transactionKeys = generalLedgerTransactions
                    .AsParallel()
                    .Where(x => x.Value.ContainsAnyForeignKey(keys))
                    .Select(x => x.Key)
                    .ToArray();

                var transactionsToInvalidate = transactionKeys.Union(keys).Distinct();

                foreach (var e in transactionsToInvalidate)
                {
                    var oldTransactions = generalLedgerTransactions.GetValueOrDefault(e);
                    if (oldTransactions != null)
                    {
                        aggregations.Update(oldTransactions.GetLines(), true);
                        generalLedgerTransactions = generalLedgerTransactions.Remove(e);
                    }

                    var newTransaction = database.SingleOrDefault(e) as ManagerServer.Model.Transaction;
                    if (newTransaction != null)
                    {
                        var output = newTransaction.CreateGeneralLedgerTransactions(database);
                        generalLedgerTransactions = generalLedgerTransactions.SetItem(newTransaction.Key, new GeneralLedgerTransactionContainer(output));
                        aggregations.Update(output, false);
                    }
                }
            }
        }        
    }
}