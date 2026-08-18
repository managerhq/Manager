using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using ManagerServer.Model.Enums;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("e5dc98ef-4662-4a68-8a9d-b3e2d12b55d6")]
    public sealed class TrialBalance : Object, IHasCustomTheme
    {
        [Guide("Enter a custom title for the report, or leave blank to use the default title.")]
        [ProtoMember(8), Placeholder(nameof(Strings.TrialBalance))] public string Title { get; set; }
        [Guide("Add an optional description or subtitle that will appear under the report title.")]
        [ProtoMember(4), Placeholder(nameof(Strings.Optional))] public string Description { get; set; }
        [Guide("Select whether to use accrual or cash basis accounting for this report.")]
        [ProtoMember(9)] public AccountingBasis AccountingMethod { get; set; }
        [Guide("Add one or more periods to compare account balances across different time frames.")]
        [ProtoMember(7), AddLineLabel(nameof(Strings.AddComparativeColumn))] public Period[] Periods { get; set; }
        [Guide("Check to display account codes alongside account names in the report.")]
        [ProtoMember(6)] public bool AccountCodes { get; set; }
        [Guide("Check to exclude accounts with zero balances from the report.")]
        [ProtoMember(10)] public bool ExcludeZeroBalances { get; set; }

        [ProtoMember(11), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(12), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }

        [ProtoContract]
        public sealed class Period
        {
            [ProtoMember(2)] public DateTime FromDate { get; set; }
            [ProtoMember(3)] public DateTime ToDate { get; set; }
            [ProtoMember(4), Autocomplete(typeof(Division)), Short] public Guid? Division { get; set; }
            [ProtoMember(1), Short, Placeholder(nameof(Strings.Automatic))] public string ColumnName { get; set; }
        }

        [ProtoMember(2)] public DateTime Obsolete_From { get; set; }
        [ProtoMember(3)] public DateTime? Obsolete_To { get; set; }
    }
}
