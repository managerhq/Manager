using System;

namespace ManagerServer.Model
{
    public interface IItem
    {
        Guid Key { get; }
        bool HasDefaultQty { get; }
        decimal? DefaultQty { get; }
        string GetNameWithCode();
        string GetDisplayName();
        string GetCode();
        string GetUnitName();
    }
}
