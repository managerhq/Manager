using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("563d7f9e-d64c-49ec-a938-e5531e72f4d8")]
    public sealed class ExpenseClaimsPayer : NamedObject, IExpenseClaimPayer
    {
        [Guide("Enter the name of the expense claims payer. This could be a department, cost center, project, or any entity within your organization that reimburses employee expenses.")]
        [ProtoMember(1)] public string Name { get; set; }
        [Guide("Select the division this payer belongs to if you use divisional accounting. This helps track expense reimbursements by division and ensures proper allocation in divisional reports.")]
        [ProtoMember(7), Autocomplete(typeof(Division))] public Guid? Division { get; set; }
        [Guide("Check this box to deactivate this payer. Inactive payers won't appear in dropdown lists when creating new expense claims, but existing expense claims will retain their association with this payer.")]
        [ProtoMember(6)] public bool Inactive { get; set; }

        [ProtoMember(5)] public StartingBalanceType Obsolete_StartingBalance2 { get; set; }
        [ProtoMember(4)] public decimal Obsolete_StartingBalanceAmount2 { get; set; }

        public override string GetName()
        {
            return Name;
        }

        public override bool IsInactive()
        {
            return Inactive;
        }

        public override bool OnAutocomplete(Object filter)
        {
            if (Inactive) return false;
            return true;
        }
    }
}
