using ManagerServer.Api.Businesses.Business.ProductionOrders;
using ManagerServer.Globalization;
using ManagerServer.Model;
using System.Collections.Generic;

namespace ManagerServer.HttpHandlers.Businesses.Business.ProductionOrders
{
    [ProtoContract]
    [Title(nameof(Strings.ProductionOrder), nameof(Strings.TransactionJournal))]
    internal sealed class ProductionOrderTransactionJournalView : DefaultView<GetProductionOrderTransactionJournal>
    {
        protected override Guid? GetCustomTheme()
        {
            return null;
        }
    }
}
