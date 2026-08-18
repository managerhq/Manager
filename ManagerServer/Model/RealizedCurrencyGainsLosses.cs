using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("2163a1a0-45db-47f3-a38a-484dd119af8c")]
    public sealed class RealizedCurrencyGainsLosses : Object, IHasCustomTheme
    {
        [Guide("The starting date for calculating realized currency gains and losses.")]
        [ProtoMember(1)] public DateTime FromDate { get; set; }
        [Guide("The ending date for calculating realized currency gains and losses.")]
        [ProtoMember(2)] public DateTime ToDate { get; set; }
        [Guide("Add an optional description or note that will appear on the report.")]
        [ProtoMember(3), Placeholder(nameof(Strings.Optional))] public string Description { get; set; }

        [ProtoMember(4), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(5), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }
    }
}