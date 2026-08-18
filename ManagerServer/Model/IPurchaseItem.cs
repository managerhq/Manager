using System;

namespace ManagerServer.Model
{
    public interface IPurchaseItem : IItem
    {
        public Guid? PurchaseItemAccount { get; }
    }
}
