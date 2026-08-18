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
    [Guid("fb6fdbfd-b39f-4674-8928-10c2bdd87e58")]
    [Singleton]
    public sealed class ProfitAndLossStatementAccountFixedAssetDepreciation : NamedObject, IProfitAndLossAccount, ICode
    {
        [Guide("Enter the name for this account. The default name is `Fixed_assets_depreciation`, but you can rename it to better suit your business needs.")]
        [ProtoMember(1), NoWrap, Placeholder(nameof(Strings.Fixed_assets_depreciation))] public string Name { get; set; }
        [Guide("Optionally, enter an account code. Codes help organize accounts and can be used for searching and sorting in reports.")]
        [ProtoMember(11), Short, Placeholder(nameof(Strings.Optional))] public string Code { get; set; }
        [Guide("Select the `ProfitAndLossStatement` group where this account should appear. This determines its placement on the profit and loss statement.")]
        [ProtoMember(3), Autocomplete(typeof(ProfitAndLossStatementGroup)), Prepend(nameof(Strings.ProfitAndLossStatement))] public Guid? Group { get; set; }
        [ProtoMember(10)] public int Position { get; set; }

        public override string GetName()
        {
            if (!string.IsNullOrWhiteSpace(Name)) return Name;
            return Strings.Fixed_assets_depreciation;
        }

        Guid IGeneralLedgerAccount.Key => Key;
        string IGeneralLedgerAccount.Name => Name;
        string IGeneralLedgerAccount.Code => Code;
        CashFlowStatementCategory IGeneralLedgerAccount.CashFlowStatementCategory => CashFlowStatementCategory.OperatingActivities;
        string ICode.Code => Code;

        public string GetCode()
        {
            return Code;
        }

        public override string GetCodeAndName()
        {
            return NameWithCode;
        }

        public string NameWithCode
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Code)) return Code + " - " + GetName();
                else return GetName();
            }
        }
    }
}
