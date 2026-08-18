using System;

namespace ManagerServer.Model
{
    public interface ISaleItem : IItem
    {
        public Guid? SaleItemAccount { get; }
        public bool HasCostOfGoodsSold { get; }
    }
}