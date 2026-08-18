using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Model.Enums;
using ProtoBuf;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("9601ce49-6058-4dac-9405-82f35005ea90")]
    public sealed class Subtotal : NamedObject
    {
        [Guide("Enter the name that will appear as the subtotal label in reports.")]
        [Guide("This label represents an intermediate sum of accounts grouped together.")]
        [Guide("Examples: 'Total Operating Expenses', 'Gross Profit', 'Current Assets'.")]
        [Guide("Subtotals help organize financial reports and calculate meaningful interim totals.")]
        [ProtoMember(1)] public string Name { get; set; }
        [Guide("Enter a number to control the order of this subtotal relative to other subtotals and accounts in reports.")]
        [Guide("Lower numbers appear first, higher numbers appear later in the report.")]
        [Guide("Use consistent numbering increments (e.g., 10, 20, 30) to allow easy insertion of new items.")]
        [Guide("Accounts are automatically positioned between subtotals based on their assigned ranges.")]
        [ProtoMember(3)] public int Position { get; set; }

        [ProtoMember(2)] public int? Obsolete_Code { get; set; }

        public override string GetName()
        {
            return Name;
        }
    }
}
