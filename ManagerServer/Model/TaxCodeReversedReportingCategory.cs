using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
using ManagerServer.Model.Attributes;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("6027ab88-f804-4827-beff-26b76bb94587")]
    public sealed class TaxCodeReversedReportingCategory : NamedObject, IReportingCategory
    {
        [Guide("Enter a descriptive name for this reversed tax code reporting category.")]
        [ProtoMember(1), TableColumn] public string Name { get; set; }
        [Guide("Check to make this reporting category inactive. Inactive categories won't appear in selection lists but existing data remains unchanged.")]
        [ProtoMember(2)] public bool Inactive { get; set; }

        public override bool OnAutocomplete(Object filter)
        {
            if (Inactive) return false;
            return true;
        }

        public bool ContainsGeneralLedgerTransactions => true;

        public override string GetName()
        {
            return Name;
        }
    }
}
