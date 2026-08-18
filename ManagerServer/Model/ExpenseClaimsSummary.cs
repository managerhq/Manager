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
    [Guid("faa1756c-5aaf-4646-9f33-555e45e37efb")]
    public sealed class ExpenseClaimsSummary : Object, IHasCustomTheme
    {
        [Guide("Enter the start date for the summary period.")]
        [ProtoMember(1)] public DateTime FromDate { get; set; }
        [Guide("Enter the end date for the summary period.")]
        [ProtoMember(2)] public DateTime ToDate { get; set; }
        [Guide("Enter an optional description for this report.")]
        [ProtoMember(3)] public string Description { get; set; }

        [ProtoMember(4), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(5), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }
    }
}
