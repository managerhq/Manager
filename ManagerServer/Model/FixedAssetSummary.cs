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
    [Guid("0dbf7789-7a6f-4182-b641-6b8e561b4a9c")]
    public sealed class FixedAssetSummary : Object, IHasCustomTheme
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
