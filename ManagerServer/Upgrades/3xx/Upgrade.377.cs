using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Model.Enums;
using ManagerServer.Globalization;
using System.Text;
using System.IO;
using ManagerServer.Model;
using System.Reflection;
using ManagerServer.Model.Attributes;
using ManagerServer.Model.Obsolete;
using System.Threading.Tasks;

namespace ManagerServer
{
    public static partial class Upgrade
    {
        private static async Task<IEnumerable<Model.Object>> Upgrade377(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var withholdingTaxReceivable = objects.OfType<SalesInvoice>().Any(x => x.WithholdingTax) || objects.OfType<CreditNote>().Any(x => x.WithholdingTax);
            var withholdingTaxPayable = objects.OfType<PurchaseInvoice>().Any(x => x.WithholdingTax);

            if (withholdingTaxReceivable || withholdingTaxPayable)
            {
                return new[] {
                    new WithholdingTax() {
                        Key = new Guid("96f2f394-8ac1-4e93-a926-5761ce8f0732"),
                        WithholdingTaxReceivable = withholdingTaxReceivable,
                        WithholdingTaxPayable = withholdingTaxPayable
                    }
                };
            }

            return null;
        }
    }
}
