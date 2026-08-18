using System;
using ProtoBuf;
using ManagerServer.Model.Attributes;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("849c558a-5f58-4779-939b-dc9d2f5ac89f")]
    public sealed class ReportTransformationLabel : NamedObject, IReportingCategory
    {
        [Guide("Enter the text that will appear as a label in report transformations.")]
        [ProtoMember(1), TableColumn] public string Name { get; set; }

        public bool ContainsGeneralLedgerTransactions => false;

        public override string GetName()
        {
            return $@"""{Name}""";
        }
    }
}