using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("68e0d57b-4a59-453e-b8d4-6166f097eacd")]
    public sealed class TaxSummary : Object, IComparable<TaxSummary>, IHasCustomTheme
    {
        [Header("Report Details")]
        [Guide("Add an optional description or note that will appear on the report.")]
        [Guide("This can be useful for documenting the purpose of the report or any special circumstances.")]
        [ProtoMember(4)] public string Description { get; set; }
        [Header("Reporting Period")]
        [Guide("Enter the starting date for the tax summary period.")]
        [Guide("This determines the beginning of the date range for which tax transactions will be included in the report.")]
        [ProtoMember(1), NoWrap] public DateTime FromDate { get; set; }
        [Guide("Enter the ending date for the tax summary period.")]
        [Guide("This determines the end of the date range for which tax transactions will be included in the report.")]
        [ProtoMember(2), NoWrap] public DateTime ToDate { get; set; }
        [Header("Accounting Method")]
        [Guide("Select whether to use accrual or cash basis accounting for this report.")]
        [Guide("`AccrualBasis` recognizes transactions when they occur, regardless of payment.")]
        [Guide("`CashBasis` recognizes transactions only when payment is made or received.")]
        [ProtoMember(3)] public AccountingBasis AccountingMethod { get; set; }
        [Header("Filtering Options")]
        [Guide("Select a specific division to filter the tax summary, or leave blank to include all divisions.")]
        [Guide("This is useful when you need tax information for a particular segment of your business.")]
        [ProtoMember(6), Autocomplete(typeof(ManagerServer.Model.Division))] public Guid? Division { get; set; }

        [ProtoMember(7), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(8), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }

        [ProtoMember(5)] public Guid? Obsolete_ReportTransformation { get; set; }

        int IComparable<TaxSummary>.CompareTo(TaxSummary other)
        {
            if (other == null) return 1;
            return (other.FromDate, other.ToDate, other.Description).CompareTo((this.FromDate, this.ToDate, this.Description));
        }
    }
}
