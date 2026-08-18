using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("3278903e-bf63-4f6f-b04d-a9e5ebd5a055")]
    public sealed class PayslipSummary : Object, IHasCustomTheme
    {
        [Guide("Enter an optional description for this report, such as 'Monthly Payroll Summary' or 'Q1 Payroll Report'.")]
        [ProtoMember(3)] public string Description { get; set; }
        [Guide("Select the starting date for the period you want to report on. This will include all payslips dated on or after this date.")]
        [ProtoMember(1)] public DateTime FromDate { get; set; }
        [Guide("Select the ending date for the period you want to report on. This will include all payslips dated on or before this date.")]
        [ProtoMember(2)] public DateTime ToDate { get; set; }

        [ProtoMember(4), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(5), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }
    }
}
