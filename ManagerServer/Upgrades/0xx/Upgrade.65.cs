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
        private static async Task<IEnumerable<Model.Object>> Upgrade65(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            var AustraliaGstLiability = new Guid("48f4c50a-306e-4865-9a2d-b43dc6ce4e14");
            var NewZealandGstLiability = new Guid("89e85fe7-06e0-4766-bf50-858842a1d81e");
            var UnitedKingdomVatLiability = new Guid("2eb47ed8-6d5b-45b9-b5d6-26d51a3bcc82");
            var SouthAfricaVatLiability = new Guid("a22efcc4-b1cd-4f0b-9c1b-4bd4404b4cc5");
            var PhilippinesVatLiability = new Guid("59ac02c4-f5e9-497b-bcd9-7f632845c0c8");
            var NorwayVatLiability = new Guid("a9ee722e-8cbe-43db-ab3f-4464556a5359");
            var BelgiumVatLiability = new Guid("b870ee01-c71d-4394-9f32-97d020c06086");
            var IndiaTaxDeductedAtSource = new Guid("60d54473-7124-4e3a-b2af-36d40384f966");
            var IndiaServiceTax = new Guid("f2fb1e42-84ef-476c-9fa2-e8b81b372f68");
            var IndiaCstLiability = new Guid("bfc49269-2843-4c6a-8ba5-c54c2f1be019");
            var taxLiabilityAccounts = new HashSet<Guid>(new[] { AustraliaGstLiability, NewZealandGstLiability, UnitedKingdomVatLiability, SouthAfricaVatLiability, PhilippinesVatLiability, NorwayVatLiability, BelgiumVatLiability, IndiaCstLiability, IndiaServiceTax, IndiaTaxDeductedAtSource });

            foreach (var e in objects.OfType<Model.Obsolete.Obsolete33.Receipt33>().Where(x => x.Lines != null).ToArray())
            {
                var dirty = false;
                foreach (var e2 in e.Lines)
                {
                    if (e2.Account.HasValue && taxLiabilityAccounts.Contains(e2.Account.Value))
                    {
                        e2.Account = new Guid("6ae01b5d-70fd-42ab-9a4c-cd9ad76c5f71");
                        dirty = true;
                    }
                }
                if (dirty) list.Add(e);
            }
            foreach (var e in objects.OfType<Model.Obsolete.Obsolete33.Payment33>().Where(x => x.Lines != null).ToArray())
            {
                var dirty = false;
                foreach (var e2 in e.Lines)
                {
                    if (e2.Account.HasValue && taxLiabilityAccounts.Contains(e2.Account.Value))
                    {
                        e2.Account = new Guid("6ae01b5d-70fd-42ab-9a4c-cd9ad76c5f71");
                        dirty = true;
                    }
                }
                if (dirty) list.Add(e);
            }
            foreach (var e in objects.OfType<Model.JournalEntry>().Where(x => x.Lines != null).ToArray())
            {
                var dirty = false;
                foreach (var e2 in e.Lines)
                {
                    if (e2.Account.HasValue && taxLiabilityAccounts.Contains(e2.Account.Value))
                    {
                        e2.Account = new Guid("6ae01b5d-70fd-42ab-9a4c-cd9ad76c5f71");
                        dirty = true;
                    }
                }
                if (dirty) list.Add(e);
            }
            foreach (var e in objects.OfType<Model.ExpenseClaim>().Where(x => x.Lines != null).ToArray())
            {
                var dirty = false;
                foreach (var e2 in e.Lines)
                {
                    if (e2.Account.HasValue && taxLiabilityAccounts.Contains(e2.Account.Value))
                    {
                        e2.Account = new Guid("6ae01b5d-70fd-42ab-9a4c-cd9ad76c5f71");
                        dirty = true;
                    }
                }
                if (dirty) list.Add(e);
            }
            return list;
        }
    }
}
