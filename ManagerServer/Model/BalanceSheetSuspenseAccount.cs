using System;
using ManagerServer.Model.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model.Enums;
using ProtoBuf;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("11211c9e-0988-4d16-8bf2-fa39487123aa")]
    [Singleton]
    public sealed class BalanceSheetSuspenseAccount : NamedObject, IBalanceSheetAccount
    {
        public override string GetName()
        {
            return Strings.Suspense;
        }

        public string NameWithCode
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(GetCode())) return GetCode() + " - " + GetName();
                else return GetName();
            }
        }

        public override string GetCodeAndName()
        {
            return NameWithCode;
        }        

        public string GetCode()
        {
            return null;
        }

        Guid IGeneralLedgerAccount.Key => Key;
        string IGeneralLedgerAccount.Name => GetName();
        string IGeneralLedgerAccount.Code => GetCode();
        CashFlowStatementCategory IGeneralLedgerAccount.CashFlowStatementCategory => CashFlowStatementCategory.OperatingActivities;
    }
}
