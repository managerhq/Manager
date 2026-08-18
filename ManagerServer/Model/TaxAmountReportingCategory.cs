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
    [Guid("f58e8724-7e63-422c-8649-a12cf77c2208")]
    public sealed class TaxAmountReportingCategory : NamedObject, IReportingCategory
    {
        [Guide("Enter a descriptive name for this tax amount reporting category.")]
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
