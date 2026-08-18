using System;
using ManagerServer.Attributes;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Model.Enums;
using ProtoBuf;
using ManagerServer.Globalization;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("5770616c-0e01-46ca-a172-f7042275da6c")]
    public sealed class ProfitAndLossStatementGroup : ChartOfAccountsGroup
    {
        [Guide("The name of the group as it will appear in reports. Choose a descriptive name that clearly identifies the category (e.g., 'Sales Revenue', 'Operating Expenses', 'Cost of Goods Sold').")]
        [ProtoMember(1)] public string Name { get; set; }
        [Guide("Select whether this is a top-level group or a subgroup of another group. Subgroups help create more detailed categorization within main groups.")]
        [ProtoMember(6), NoWrap] public ProfitAndLossStatementGroupType Type { get; set; }
        [Guide("Select the parent group that this subgroup belongs to. This creates a hierarchical structure in your reports.")]
        [ProtoMember(3), Autocomplete(typeof(ManagerServer.Model.ProfitAndLossStatementGroup)), EmptyLabel, IfEnum(nameof(Type), (int)ProfitAndLossStatementGroupType.SubgroupOf)] public Guid? Group { get; set; }
        [Guide("The display order of this group in reports. Lower numbers appear first. Use this to customize the sequence of groups (e.g., show revenue groups before expense groups).")]
        [ProtoMember(5)] public int Position { get; set; }

        [ProtoMember(4)] public bool Obsolete_ExpenseGroup { get; set; }
        [ProtoMember(2)] public int? Obsolete_Code { get; set; }

        public override string GetName()
        {
            return Name;
        }
    }
}
