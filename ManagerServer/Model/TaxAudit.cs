using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("a43c996d-707a-48cb-91c8-191feab4411f")]
    public sealed class TaxAudit : Object, IComparable<TaxAudit>, IHasCustomTheme
    {
        [Guide("Add an optional description or note that will appear on the report.")]
        [ProtoMember(5)] public string Description { get; set; }
        [Guide("The starting date for the tax audit period.")]
        [ProtoMember(2)] public DateTime FromDate { get; set; }
        [Guide("The ending date for the tax audit period.")]
        [ProtoMember(3)] public DateTime ToDate { get; set; }
        [Guide("Select whether to use accrual or cash basis accounting for this report.")]
        [ProtoMember(4)] public AccountingBasis AccountingMethod { get; set; }

        [ProtoMember(6), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(7), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }

        int IComparable<TaxAudit>.CompareTo(TaxAudit other)
        {
            if (other == null) return 1;
            return (other.FromDate, other.ToDate, other.Description).CompareTo((this.FromDate, this.ToDate, this.Description));
        }
    }
}
