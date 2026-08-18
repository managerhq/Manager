/*
using System;
using System.Collections.Generic;
using System.Linq;
using Manager.Globalization;
using Manager.Attributes;
using Manager.Model;

namespace ManagerServer.HttpHandlers.Businesses.Business.Customers
{
    [ProtoContract]
    [Title(nameof(Strings.Customer), nameof(Strings.QtyToInvoice))]
    internal sealed class CustomerQtyToInvoice : NakedObjectsWithCustomFields<Tuple<Manager.Model.InventoryItem, decimal>>
    {
        [ProtoMember(1)] public Guid Customer;

        internal override string GetTitle()
        {
            return Manager.ApplicationData.Businesses.Get(FileID)?.SingleOrDefault<Manager.Model.Customer>(Customer)?.NameWithCode + " — " + Strings.QtyToInvoice;
        }

        protected override void InnerGet4(Context context)
        {
            var database = Manager.ApplicationData.Businesses.Get(FileID);

            var list = new List<Tuple<InventoryItem, decimal>>();

            list.AddRange(database.OfType<DeliveryNote>()
                .SelectMany(x => x.GetGeneralLedgerTransactions(database))
                .Where(x => x.Customer?.Key == Customer)
                .Where(x => x.InventoryItem != null)
                .Where(x => x.QtyToDeliver != 0m)
                .Select(x => new Tuple<InventoryItem, decimal>(x.InventoryItem, x.QtyToDeliver)));

            list.AddRange(database.OfType<SalesInvoice>()
                .SelectMany(x => x.GetGeneralLedgerTransactions(database))
                .Where(x => x.Customer?.Key == Customer)
                .Where(x => x.InventoryItem != null)
                .Where(x => x.QtyToDeliver != 0m)
                .Select(x => new Tuple<InventoryItem, decimal>(x.InventoryItem, x.QtyToDeliver)));

            list.AddRange(database.OfType<CreditNote>()
                .SelectMany(x => x.GetGeneralLedgerTransactions(database))
                .Where(x => x.Customer?.Key == Customer)
                .Where(x => x.InventoryItem != null)
                .Where(x => x.QtyToDeliver != 0m)
                .Select(x => new Tuple<InventoryItem, decimal>(x.InventoryItem, x.QtyToDeliver)));

            var balances = list.GroupBy(x => x.Item1).Select(x => new Tuple<InventoryItem, decimal>(x.Key, x.Sum(y => y.Item2)*-1m)).ToArray();
            balances = balances.Where(x => x.Item2 >= 0m).ToArray();
            balances = balances.OrderByDescending(x => x.Item2).ToArray();

            context.Set<Array>(balances);

            base.InnerGet4(context);
        }

        [Default]
        [Guid("c87fce65-2b7e-4c33-9f8f-ca06945f6169")]
        public NamedObject[] GetInventoryItem(Tuple<Manager.Model.InventoryItem, decimal>[] rows)
        {
            return rows.Select(x => x.Item1).ToArray();
        }

        [Default, Right, Sum, Bold]
        [Guid("9b3f04ca-0e41-489f-8229-dfe2d66fa2ad")]
        public Tuple<decimal, BusinessTemplate>[] GetQtyToDeliver(Tuple<Manager.Model.InventoryItem, decimal>[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new Tuple<decimal, BusinessTemplate>(x.Item2, new InventoryItems.InventoryItemQtyToDeliverTransactions() { FileID = FileID, Customer = Customer, InventoryItem = x.Item1.Key, Referrer = referrer })).ToArray();
        }
    }
}
*/