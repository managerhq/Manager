using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("6c1d3132-7978-45c8-a6e2-2387c7de46b0")]
    public sealed class GeneralLedgerSummary : Object, IComparable<GeneralLedgerSummary>, IHasCustomTheme
    {
        [Guide("Enter an optional description for this report.")]
        [ProtoMember(4)] public string Description { get; set; }
        [Guide("Enter the start date for the summary period.")]
        [ProtoMember(2)] public DateTime FromDate { get; set; }
        [Guide("Enter the end date for the summary period.")]
        [ProtoMember(3)] public DateTime ToDate { get; set; }
        [Guide("Check this box to display account codes alongside account names.")]
        [ProtoMember(6)] public bool AccountCodes { get; set; }
        [Guide("Check this box to hide accounts with zero movement and balance.")]
        [ProtoMember(7)] public bool ExcludeZeroBalances { get; set; }

        [ProtoMember(8), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(9), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }

        int IComparable<GeneralLedgerSummary>.CompareTo(GeneralLedgerSummary other)
        {
            if (other == null) return 1;
            return (other.FromDate, other.ToDate).CompareTo((this.FromDate, this.ToDate));
        }
    }
}
