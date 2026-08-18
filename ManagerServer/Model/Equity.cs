using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Singleton]
    [Guid("9275ff4c-4cff-41d0-b7b5-f31c783f03d8")]
    public sealed class Equity : BalanceSheetAbstractGroup
    {
        [Guide("Enter a custom name for the equity section. Leave blank to use the default 'Equity' label.")]
        [Guide("The equity section represents the owners' residual interest in the business after all liabilities are paid.")]
        [Guide("Alternative names might include 'Shareholders' Equity', 'Members' Equity', 'Partners' Capital', or 'Net Worth'.")]
        [Guide("This name appears as a main heading on the `BalanceSheet` report.")]
        [ProtoMember(1), Placeholder(nameof(Strings.Equity))] public string Name { get; set; }

        public override string GetName()
        {
            if (!string.IsNullOrWhiteSpace(Name)) return Name;
            return Strings.Equity;
        }
    }
}