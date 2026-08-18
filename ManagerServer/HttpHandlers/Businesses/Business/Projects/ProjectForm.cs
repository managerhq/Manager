using System;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Query;
using ManagerServer.Helpers;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Projects
{
    [ProtoContract]
    [Title(nameof(Strings.Project), nameof(Strings.Edit))]
    [Guide("The `Project` form is used to create or edit projects for tracking income and expenses.")]
    [Guide("Projects help you monitor profitability and costs for specific jobs, contracts, or activities.")]
    [Guide("Use projects to track financial performance for individual customer relationships, specific work clusters, or time-limited contracts.")]
    [Header("Setting Up a Project")]
    [Guide("When creating a project, provide a descriptive name that clearly identifies the project, such as 'Website Redesign for ABC Corp' or 'Q4 Marketing Campaign'.")]
    [Guide("You can optionally add an image to help visually identify the project in lists and reports.")]
    [Header("Using Projects")]
    [Guide("After creating a project, you can assign it to transactions when recording income or expenses. This allows you to track the financial performance of each project separately.")]
    [Guide("Projects can be linked to various transaction types including `Sales Invoices`, `Purchase Invoices`, `Receipts`, `Payments`, and `Purchase Orders`.")]
    [LinkGuide("To view all projects and their financial summaries, see:", typeof(Projects))]
    [LinkGuide("To view detailed profitability analysis for a project, see:", typeof(ProjectReportView))]
    [Fields(typeof(ManagerServer.Model.Project))]
    internal sealed class ProjectForm : NakedVueForm<ManagerServer.Model.Project>
    {
        protected override bool CanHaveImage()
        {
            return true;
        }
    }
}