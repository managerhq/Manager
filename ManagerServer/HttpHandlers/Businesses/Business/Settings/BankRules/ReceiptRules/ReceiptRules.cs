using ManagerServer.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.BankRules.ReceiptRules
{
    [ProtoContract]
    [NamespaceEntry]
    [IfTab(nameof(Receipts))]
    [Guid("9a00c7b5-fa4c-4f4b-9c2b-7e18bfeca727")]
    [Title(nameof(Strings.ReceiptRules))]
    [Guide("Receipt rules allow you to automatically categorize bank transactions as receipts based on specific conditions.")]
    internal sealed class ReceiptRules : NakedObjectsWithAutomaticRows<ManagerServer.Model.ReceiptRule>
    {
        [Default]
        [Guid("30211401-9973-4c7c-b374-d0c88d69e50e")]
        public string[] GetIfBankAccountIs(ManagerServer.Model.ReceiptRule[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => database.SingleOrDefault<ManagerServer.Model.BankOrCashAccount>(x.IfBankAccountIs)?.GetName()).ToArray();
        }

        [Default]
        [Guid("94391c53-1965-4644-b31a-355391c2e794")]
        public string[] GetAndDescriptionContains(ManagerServer.Model.ReceiptRule[] rows)
        {
            return rows.Select(x => string.Join(", ", x.Conditions?.Select(x => x.AndDescriptionContains) ?? new string[0])).ToArray();
        }
    }
}