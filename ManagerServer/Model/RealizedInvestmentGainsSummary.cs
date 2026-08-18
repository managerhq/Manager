using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("2a58eef3-1eb1-4bd9-907b-aca2352bae77")]
    public sealed class RealizedInvestmentGainsSummary : Object, IHasCustomTheme
    {
        [Guide("Add an optional description or note that will appear on the report.")]
        [ProtoMember(1)] public string Description { get; set; }
        [Guide("The starting date for calculating realized investment gains and losses.")]
        [ProtoMember(2)] public DateTime FromDate { get; set; }
        [Guide("The ending date for calculating realized investment gains and losses.")]
        [ProtoMember(3)] public DateTime ToDate { get; set; }

        [ProtoMember(4), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(5), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }
    }
}
