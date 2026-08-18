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
        private static async Task<IEnumerable<Model.Object>> Upgrade3(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var contacts = objects.OfType<Model.Obsolete.Obsolete01.Contact01>().ToDictionary(x => x.Key);
            if (contacts.Count == 0) return null;

            var list = new List<Model.Object>();

            var salesInvoices = objects.OfType<ManagerServer.Model.Obsolete.Obsolete02.SalesInvoice02>().Where(x => x.To.HasValue && contacts.ContainsKey(x.To.Value)).ToArray();
            var purchaseInvoices = objects.OfType<ManagerServer.Model.Obsolete.Obsolete02.PurchaseInvoice02>().Where(x => x.From.HasValue && contacts.ContainsKey(x.From.Value)).ToArray();

            foreach (var e in salesInvoices)
            {
                var contact = contacts[e.To.Value];
                var debtor = objects.OfType<Model.Customer>().SingleOrDefault(x => x.Name == contact.Name);
                if (debtor == null)
                {
                    debtor = new Model.Customer() { Name = contact.Name, BillingAddress = contact.Address, Email = contact.Email, Obsolete_Fax = contact.Fax, Key = Guid.CreateVersion7(), Obsolete_Mobile = contact.Mobile, Obsolete_Notes = contact.Notes, Obsolete_Telephone = contact.Telephone };
                    list.Add(debtor);
                }
                e.To = debtor.Key;
                list.Add(e);
            }

            foreach (var e in purchaseInvoices)
            {
                var contact = contacts[e.From.Value];
                var creditor = objects.OfType<Model.Supplier>().SingleOrDefault(x => x.Name == contact.Name);
                if (creditor == null)
                {
                    creditor = new Model.Supplier() { Name = contact.Name, Email = contact.Email, Obsolete_Fax = contact.Fax, Key = Guid.CreateVersion7(), Obsolete_Mobile = contact.Mobile, Obsolete_Notes = contact.Notes, Obsolete_Telephone = contact.Telephone };
                    list.Add(creditor);
                }
                e.From = creditor.Key;
                list.Add(e);
            }

            return list.ToArray();
        }
    }
}
