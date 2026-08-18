using System;
using ManagerServer.Api.Businesses.Business.Projects;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Projects
{
    [ProtoContract]
    [Title(nameof(Strings.Project))]
    [Guide("The `Project` view displays comprehensive information about a selected project, including its details, related transactions, and financial summary.")]
    [Guide("This view provides a complete overview of your project's current state and history, making it easy to track progress and monitor financial performance.")]
    [Header("Available Actions")]
    [Guide("From this view, you can:")]
    [Guide("• Click the `Edit` button to modify project details such as name, description, or status")]
    [Guide("• View all transactions associated with the project, including income and expenses")]
    [Guide("• Attach and manage project-related documents and files")]
    [Guide("• Review the complete transaction history to understand how the project has evolved over time")]
    [Header("Understanding Project Data")]
    [Guide("The project view consolidates all relevant information in one place, showing financial summaries, transaction lists, and any attached documents.")]
    [Guide("Use this view to get a quick snapshot of project performance or to drill down into specific transaction details.")]
    [LinkGuide("To learn about creating and editing projects, see:", typeof(ProjectForm))]
    internal sealed class ProjectView : DefaultView<GetProjectView>
    {        
    }
}
