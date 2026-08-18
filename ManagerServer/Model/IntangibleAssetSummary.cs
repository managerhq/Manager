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
    [Guid("d76b3a1a-bb37-4767-9f65-f0389ec9d5df")]
    public sealed class IntangibleAssetSummary : Object, IHasCustomTheme
    {
        [Guide("Enter the start date for the summary period.")]
        [ProtoMember(1)] public DateTime FromDate { get; set; }
        [Guide("Enter the end date for the summary period.")]
        [ProtoMember(2)] public DateTime ToDate { get; set; }
        [Guide("Enter an optional description for this report.")]
        [ProtoMember(3), Placeholder(nameof(Strings.Optional))] public string Description { get; set; }

        [ProtoMember(4), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(5), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }
    }
}
