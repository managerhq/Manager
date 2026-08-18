using System;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.WithholdingTax
{
    [ProtoContract]
    [NamespaceEntry]
    [Title(nameof(Strings.WithholdingTax))]
    [Guide("The `WithholdingTax` feature found in the `Settings` tab enables businesses to handle tax withholding requirements where customers or suppliers are required to withhold tax from payments.")]
    [SettingsItemScreenshot("fa-percent", nameof(Strings.WithholdingTax))]
    [Guide("Withholding tax is a government requirement in many jurisdictions where the payer must deduct tax from payments and remit it directly to tax authorities. This feature allows you to track both withholding tax receivable (when customers withhold tax from payments to you) and withholding tax payable (when you withhold tax from payments to suppliers). The withheld amounts are recorded separately from the main transaction, ensuring accurate tracking for tax compliance.")]
    [Guide("Enable withholding tax receivable if your customers are required to withhold tax from their payments to you. Enable withholding tax payable if you need to withhold tax when paying suppliers. Once enabled, withholding tax fields will appear on relevant invoices, and the system will automatically calculate and track these amounts.")]
    [Guide("The form includes these fields:")]
    [Fields(typeof(ManagerServer.Model.WithholdingTax))]
    internal sealed class WithholdingTaxForm : NakedVueForm<ManagerServer.Model.WithholdingTax>
    {
        internal override bool IsEmpty(TabsExtensions.Item[] tabs)
        {
            var o = ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.WithholdingTax>();
            return !o.WithholdingTaxReceivable && !o.WithholdingTaxPayable;
        }
    }
}
