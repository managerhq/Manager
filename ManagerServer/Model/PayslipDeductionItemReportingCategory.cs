using System;
using ProtoBuf;
using ManagerServer.Model.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("1ccb2c74-e9ed-4642-9687-bdf9f3403f3b")]
    public sealed class PayslipDeductionItemReportingCategory : NamedObject, IReportingCategory
    {
        [Guide("Enter a name for this reporting category, such as 'Tax Deductions' or 'Voluntary Deductions'.")]
        [ProtoMember(1), TableColumn] public string Name { get; set; }
        [Guide("Check this box to make the reporting category inactive. Inactive categories won't appear in selection lists.")]
        [ProtoMember(2)] public bool Inactive { get; set; }

        public override bool OnAutocomplete(Object filter)
        {
            if (Inactive) return false;
            return true;
        }

        public bool ContainsGeneralLedgerTransactions => true;

        public override string GetName()
        {
            return Strings.PayslipDeductionItem + " - " + Name;
        }
    }
}