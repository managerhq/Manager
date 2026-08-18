using System;
using ManagerServer.Model.Enums;

namespace ManagerServer.Model
{
    public interface IRecurringTransactionDestination : IObject
    {
        DateTime Date { get; set; }
    }
    
    public interface IRecurringTransaction : IObject
    {
        DateTime? NextIssueDate { get; set; }
        int? Interval { get; }
        Period PeriodType { get; }
        MonthDay MonthDay { get; }
        ExpirationType ExpirationType { get; }
        DateTime? UntilDate { get; }

        bool CanBeIssued()
        {
            if (!NextIssueDate.HasValue) return false;
            if (Interval.HasValue && Interval.Value <= 0) return false;
            if (NextIssueDate.Value > DateTime.Today) return false;
            return true;
        }
    }

    public interface IRecurringTransactionFor<T> : IRecurringTransaction where T : ManagerServer.Model.Object, IRecurringTransactionDestination, new()
    {        
    }
}