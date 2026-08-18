using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses
{
    [ProtoContract]
    [Title(nameof(Strings.Error))]
    [Guide("This error appears when you try to access a tab that is not available in your current business.")]
    [Guide("There are two possible reasons why you cannot access this tab:")]
    [Guide("1. The tab has been disabled in the business settings")]
    [Guide("2. You do not have permission to view this tab")]
    [Guide("To resolve this issue, you can:")]
    [Guide("- Contact your administrator to enable the tab under `Settings`")]
    [Guide("- Ask your administrator to grant you permission to access this tab")]
    [Guide("- If you are the administrator, go to `Settings` and enable the required tab")]
    internal sealed class NoTab : Template
    {
        protected override Task InnerGet()
        {
            using (Div(@class: "p-8 mx-auto max-w-prose"))
            {
                using (Div(@class: "card"))
                {
                    using (Div(@class: "card-body p-8"))
                    {
                        Write("The administrator has not set up your permissions for this business.");
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
