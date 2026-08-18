using System;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.PayslipItems
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(Payslips))]
    [Title(nameof(Strings.PayslipItems))]
    [Guide("The **Payslip Items** screen, found under the **Settings** tab, is used to define and manage items that appear on employee payslips.")]
    [Guide("These items represent various types of earnings, deductions, and contributions that make up an employee's pay.")]
    [Guide("Examples include wages, overtime, tax withholdings, pension contributions, and other benefits or deductions.")]
    [SettingsItemScreenshot("fa-tasks-alt", nameof(Strings.PayslipItems))]
    internal sealed class PayslipItems : NakedNamespaces
    {
    }
}
