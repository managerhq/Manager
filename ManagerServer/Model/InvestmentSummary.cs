using ManagerServer.Model.Attributes;
using ProtoBuf;
using System;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("363c1dc2-a1f9-4221-8beb-d00e5dd5d3d4")]
    public sealed class InvestmentSummary : ManagerServer.Model.Object
    {
        [Guide("Select the date for which you want to view the investment summary. The report will show investment values and calculate gains/losses as of this date.")]
        [ProtoMember(1)] public DateTime Date { get; set; }

        [Guide("Enter optional footer text that will appear at the bottom of the investment summary report. You can use this for disclaimers, notes, or additional information.")]
        [ProtoMember(3), Textarea, Long] public string Footer { get; set; }

        [ProtoMember(2)] public Item[] Obsolete_MarketPrices { get; set; }

        [ProtoContract]
        public sealed class Item
        {
            [ProtoMember(1)] public Guid? Investment { get; set; }
            [ProtoMember(2)] public decimal MarketPrice { get; set; }
        }
    }
}
