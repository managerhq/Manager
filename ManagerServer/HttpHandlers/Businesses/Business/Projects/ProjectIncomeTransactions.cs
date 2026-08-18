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
    [Title(nameof(Strings.Project), nameof(Strings.Income))]
    [Guide("The **Project - Income** screen displays all income transactions recorded for a specific project.")]
    [Guide("This view helps you track revenue and income associated with your project, including *sales invoices*, *receipts*, and other income-generating transactions.")]
    [Guide("Each transaction shows the date, description, account, and amount, allowing you to monitor the financial performance of your project.")]
    [Guide("Use this screen to analyze your project's revenue streams and ensure all income is properly recorded and allocated.")]
    internal sealed class ProjectIncomeTransactions : TransactionViewer
    {
        [ProtoMember(1)] public Guid Project;

        protected override bool MultipleByOne()
        {
            return true;
        }

        protected override IEnumerable<ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction> GetTransactions()
        {
            return new ManagerServer.Query.GeneralLedger.GeneralLedger(Business).Where(x => x.Project?.Key == Project && !x.IsProjectCost);
        }
    }
}