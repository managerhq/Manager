using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("997f3bd6-37ee-4b36-b066-2bb0a402ebab")]
    public sealed class SalesInvoiceTotalsByCustomer : Object, IHasCustomTheme
    {
        [Guide("Add an optional description or note that will appear on the report.")]
        [ProtoMember(2)] public string Description { get; set; }
        [Guide("Add one or more periods to compare sales totals across different time frames.")]
        [ProtoMember(1), AddLineLabel(nameof(Strings.AddComparativeColumn))] public Period[] Periods { get; set; }

        [ProtoMember(3), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(4), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }

        [ProtoContract]
        public sealed class Period
        {
            [ProtoMember(1)] public DateTime FromDate { get; set; }
            [ProtoMember(2)] public DateTime ToDate { get; set; }
            [ProtoMember(3), Short] public string ColumnName { get; set; }
        }       
    }
}
