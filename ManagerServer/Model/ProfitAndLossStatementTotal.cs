using ManagerServer.Model.Attributes;
using ManagerServer.Attributes;
using ProtoBuf;
using System;
using ManagerServer.Globalization;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("00250863-18d4-42ca-9e1b-adef10965570")]
    public sealed class ProfitAndLossStatementTotal : NamedObject
    {
        [Guide("Enter a custom name for the final total line. Common options include 'Net Profit (Loss)', 'Net Income' or leave blank to use the default.")]
        [ProtoMember(1), Placeholder(nameof(Strings.Net_profit_loss))] public string Name { get; set; }

        public override string GetName()
        {
            if (string.IsNullOrWhiteSpace(Name)) return Strings.Net_profit_loss;
            return Name;
        }
    }
}
