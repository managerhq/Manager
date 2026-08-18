using System;
using ProtoBuf;
using ManagerServer.Model.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("3de1fae6-48ea-4901-8e1b-507483bfc4f3")]
    public sealed class PayslipEarningsItemReportingCategory : NamedObject, IReportingCategory
    {
        [Guide("Enter a name for this reporting category, such as 'Regular Earnings' or 'Overtime and Bonuses'.")]
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
            return Strings.PayslipEarningsItem + " - " + Name;
        }
    }
}
