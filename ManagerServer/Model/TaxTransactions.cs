using ManagerServer.Model.Attributes;
using ManagerServer.Model.Enums;
using ProtoBuf;
using System;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("9a441483-1a09-46d3-aecd-477c91c13ae1")]
    public sealed class TaxTransactions : Object, IHasCustomTheme
    {
        [Guide("Add an optional description or note that will appear on the report.")]
        [ProtoMember(4)] public string Description { get; set; }
        [Guide("The starting date for transactions to include in the report.")]
        [ProtoMember(1)] public DateTime FromDate { get; set; }
        [Guide("The ending date for transactions to include in the report.")]
        [ProtoMember(2)] public DateTime ToDate { get; set; }
        [Guide("Select whether to use accrual or cash basis accounting for this report.")]
        [ProtoMember(3)] public AccountingBasis AccountingMethod { get; set; }

        [ProtoMember(5), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(6), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }
    }
}
