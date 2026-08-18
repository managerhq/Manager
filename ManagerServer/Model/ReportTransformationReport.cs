using System;
using ProtoBuf;
using ManagerServer.Model.Enums;
using ManagerServer.Model.Attributes;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("bbe8c088-729b-4b56-a7d7-26555270eced")]
    public sealed class ReportTransformationReport : Object, IHasCustomTheme
    {
        [ProtoMember(1), Hidden] public Guid? ReportTransformation { get; set; }
        [Guide("Add an optional description or note that will appear on the report.")]
        [ProtoMember(2)] public string Description { get; set; }
        [Guide("The starting date for the report period.")]
        [ProtoMember(4), NoWrap] public DateTime FromDate { get; set; }
        [Guide("The ending date for the report period.")]
        [ProtoMember(5)] public DateTime ToDate { get; set; }
        [Guide("Select whether to use accrual or cash basis accounting for this report.")]
        [ProtoMember(3), IfTrue(nameof(ReportTransformation), nameof(ManagerServer.Model.ReportTransformation2.HasAccountingMethod))] public AccountingBasis AccountingMethod { get; set; }
        [Guide("Select a specific employee to filter the report data.")]
        [ProtoMember(6), IfTrue(nameof(ReportTransformation), nameof(ManagerServer.Model.ReportTransformation2.HasEmployee)), Autocomplete(typeof(Employee))] public Guid? Employee { get; set; }

        [ProtoMember(7), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(8), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }
    }
}
