using System;
using ProtoBuf;
using ManagerServer.Model.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("ad4c002b-f4ea-4bf5-85cc-f65dd4398794")]
    public sealed class PayslipContributionItemReportingCategory : NamedObject, IReportingCategory
    {
        [Guide("Enter a name for this reporting category, such as 'Retirement Benefits' or 'Health Insurance'.")]
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
            return Strings.PayslipContributionItem+" - "+Name;
        }
    }
}
