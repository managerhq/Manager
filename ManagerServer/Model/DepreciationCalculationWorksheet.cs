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
    [Guid("105604f5-5bc1-4bfa-a101-3c339c22989a")]
    public sealed class DepreciationCalculationWorksheet : Object, IHasCustomTheme
    {
        [Guide("Enter the start date for the depreciation calculation period.")]
        [ProtoMember(1)] public DateTime FromDate { get; set; }
        [Guide("Enter the end date for the depreciation calculation period.")]
        [ProtoMember(2)] public DateTime ToDate { get; set; }
        [Guide("Enter an optional description for this worksheet.")]
        [ProtoMember(3), Placeholder(nameof(Strings.Optional))] public string Description { get; set; }

        [ProtoMember(4), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(5), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }
    }
}