using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using ManagerServer.Model.Enums;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("56423a58-cdaf-4b99-b215-13e3202d5167")]
    public sealed class TaxTotals : Object, IComparable<TaxTotals>, IHasCustomTheme
    {
        [Header("Report Details")]
        [Guide("Add an optional description or note that will appear on the report.")]
        [Guide("This can be useful for documenting the purpose of the report or any special circumstances.")]
        [ProtoMember(1)] public string Description { get; set; }
        [Header("Reporting Period")]
        [Guide("Enter the starting date for the tax totals period.")]
        [Guide("This determines the beginning of the date range for which tax transactions will be included in the report.")]
        [ProtoMember(2), NoWrap] public DateTime FromDate { get; set; }
        [Guide("Enter the ending date for the tax totals period.")]
        [Guide("This determines the end of the date range for which tax transactions will be included in the report.")]
        [ProtoMember(3), NoWrap] public DateTime ToDate { get; set; }
        [Header("Accounting Method")]
        [Guide("Select whether to use accrual or cash basis accounting for this report.")]
        [Guide("`AccrualBasis` recognizes transactions when they occur, regardless of payment.")]
        [Guide("`CashBasis` recognizes transactions only when payment is made or received.")]
        [ProtoMember(4)] public AccountingBasis AccountingMethod { get; set; }
        [Header("Filtering Options")]
        [Guide("Select a specific division to filter the tax totals, or leave blank to include all divisions.")]
        [Guide("This is useful when you need tax information for a particular segment of your business.")]
        [ProtoMember(5), Autocomplete(typeof(ManagerServer.Model.Division))] public Guid? Division { get; set; }

        [ProtoMember(6), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(7), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }

        int IComparable<TaxTotals>.CompareTo(TaxTotals other)
        {
            if (other == null) return 1;
            return (other.FromDate, other.ToDate, other.Description).CompareTo((this.FromDate, this.ToDate, this.Description));
        }
    }
}
