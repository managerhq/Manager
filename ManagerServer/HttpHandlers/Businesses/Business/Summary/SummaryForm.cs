using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Model.Enums;
using ProtoBuf;
using ManagerServer.HttpHandlers.Businesses.Business.Settings.ChartOfAccounts;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Summary
{
    [ProtoContract]
    [Title(nameof(Strings.Summary), nameof(Strings.Edit))]
    [Guide("The `Summary` tab provides an overview of your business's financial position and key information. When you click the `Edit` button on this tab, you can customize how the summary information is displayed.")]
    [Guide("This customization screen allows you to control which sections appear on your `Summary` tab and how they are organized. You can show or hide different components based on your business needs.")]
    [HeroButtonScreenshot(nameof(Strings.Summary), nameof(Strings.Edit))]
    [Guide("The form below contains various options to configure your summary display. Each field represents a different section or component that can be shown on the `Summary` tab.")]
    [Fields(typeof(ManagerServer.Model.Summary))]
    internal sealed class SummaryForm : NakedVueForm<ManagerServer.Model.Summary>
    {
    }
}
