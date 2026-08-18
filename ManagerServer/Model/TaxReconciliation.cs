using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using ManagerServer.Model.Enums;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("82fb1232-ba69-4310-b443-22df87ef18fa")]
    public sealed class TaxReconciliation : Object, IComparable<TaxReconciliation>, IHasCustomTheme
    {
        [Guide("Add an optional description or note that will appear on the report.")]
        [ProtoMember(1)] public string Description { get; set; }
        [Guide("Select whether to use accrual or cash basis accounting for this report.")]
        [ProtoMember(3)] public AccountingBasis AccountingMethod { get; set; }
        [Guide("Add one or more periods to compare tax reconciliation across different time frames.")]
        [ProtoMember(2), AddLineLabel(nameof(Strings.AddComparativeColumn))] public Period[] Periods { get; set; }

        [ProtoMember(4), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(5), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }

        [ProtoContract]
        public sealed class Period
        {
            [ProtoMember(1)] public DateTime FromDate { get; set; }
            [ProtoMember(2)] public DateTime ToDate { get; set; }
        }

        int IComparable<TaxReconciliation>.CompareTo(TaxReconciliation other)
        {
            if (other == null) return 1;
            return (other.Periods?[0]?.FromDate, other.Periods?[0]?.ToDate, other.Description).CompareTo((this.Periods?[0]?.FromDate, this.Periods?[0]?.ToDate, this.Description));
        }
    }
}
