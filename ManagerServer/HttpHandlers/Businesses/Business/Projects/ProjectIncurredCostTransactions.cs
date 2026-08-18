using System;
using System.Linq;
using ManagerServer.Globalization;
using System.Collections.Generic;
using ManagerServer.Model;
using ManagerServer.Query;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Projects
{
    [ProtoContract]
    [Title(nameof(Strings.Project), nameof(Strings.DirectCosts))]
    [Guide("The **Direct Costs** screen shows all transactions that represent direct costs incurred for a specific project.")]
    [Guide("Direct costs are expenses that can be directly attributed to a project, such as materials purchased specifically for the project, labor costs of employees working on the project, or services procured exclusively for the project.")]
    [Header("Understanding Direct Costs")]
    [Guide("When you allocate transactions to a project, those transactions appear in this list if they qualify as direct costs. This helps you track the actual costs being incurred against your *project budget*.")]
    [Guide("The list includes details such as *transaction dates*, *descriptions*, *amounts*, and the *accounts* affected. This provides a complete audit trail of all project-related expenses.")]
    [Header("Using This Information")]
    [Guide("Review this screen regularly to monitor your project's cost performance. Compare the actual costs shown here against your *project budget* to identify any overruns early.")]
    [Guide("You can click on individual transactions to view their full details or make corrections if needed.")]
    internal sealed class ProjectIncurredCostTransactions : TransactionViewer
    {
        [ProtoMember(1)] public Guid Project;

        protected override IEnumerable<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction> GetTransactions()
        {
            return new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).Where(x => x.Project?.Key == Project && x.IsProjectCost);
        }
    }
}