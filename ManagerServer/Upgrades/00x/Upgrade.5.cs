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
        private static async Task<IEnumerable<Model.Object>> Upgrade5(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var builtInAccounts = new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>[]
{
new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("e30c259c-c849-46a8-8058-c0f580a5733a"), "Commissions_received", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Income),
new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("5ac43aff-0cdc-4de4-a351-9d2a8ec2891b"), "Consulting_fees", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Income),
new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("0481d5e9-ac7a-4c9c-94be-47b9d1c505dc"), "Government_grants", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Income),
new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("2cff03bd-3d57-4226-a0d4-3092501f16bd"), "Refunds received", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Income),
new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("60332379-3a0e-4be1-978d-dc8e7805a41a"), "Dividends_received", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Income),
new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("e66c0be3-b0b4-485f-ae27-9011875f4cb0"), "Contract income", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Income),
new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("afb59d45-deed-4464-9a8d-b23cf24d186e"), "Insurance proceeds", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Income),
new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("70abb1df-312b-4bbd-8f06-6ef34ec22ada"), "Interest received", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Income),
new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("e37f2e68-5ff9-46fb-b7da-d18cfc9f5875"), "Sales", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Income),
new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("49f223a6-e059-4bb9-a39c-b591847cd032"), "Professional fees", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Income),
new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("4897b38a-b025-4aa2-aa78-c7b352f18a4d"), "Services income", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Income),
new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("04226b15-58ca-44bf-9fad-2520ec138363"), "Rental income", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Income),
new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("79828a85-63cd-4534-a570-b30ab2d9595f"), "Other income", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Income),

new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("ce68103f-ceec-466c-beba-c90cd30fa229"), "Accounting fees", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Expenses),
new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("647b6c96-b2b4-4fd7-ac46-636fff68b726"), "Advertising and promotion", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Expenses),
new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("9fec801b-0eca-4a32-bb94-ffe46a80728a"), "Audit fees", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Expenses),
new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("bd9774df-6106-4465-8e02-f587ed81a201"), "Bank charges", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Expenses),
new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("10c55f2a-25ff-4070-953b-9de8a0b2c234"), "Cleaning", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Expenses),
new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("5bb0cf73-8747-4002-a884-c039dff54b49"), "Commissions_paid", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Expenses),
new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("516c3cd8-9159-4d8f-8074-0ca79e53fd6b"), "Computer equipment", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Expenses),
new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("b75eaec8-585a-4722-a436-8b597e2c1b27"), "Contractors", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Expenses),
new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("9649aa90-f809-4c68-933b-cd66e67d4f81"), "Conferences and seminars", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Expenses),
new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("3cf6d231-2a7c-4dae-b435-e20d03f0fbde"), "Consultants fees", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Expenses),
new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("07326976-7b67-4976-b632-b911449a8bb1"), "Courier fees", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Expenses),
new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("a0702bd1-6e64-407f-adfb-cc7386bea83f"), "Delivery expenses", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Expenses),
new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("37788f37-81bb-469c-ae0a-3413d3c33ff8"), "Donations", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Expenses),
new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("c4edf318-fbcb-4622-b23a-1d335dcc369b"), "Electricity", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Expenses),
new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("d5087847-f8c3-49eb-902b-8a4f211f90b9"), "Entertainment", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Expenses),
new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("46f732bf-20ef-430c-8a2e-ecdde55d2fa8"), "Fees and charges", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Expenses),
new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("da7d16a7-eb17-4d10-8bf1-cabad44a5754"), "Freight and cartage", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Expenses),
new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("2377971c-6265-4ae8-aefc-9f0abb49bdf4"), "Insurance", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Expenses),
new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("85b0a52c-db6f-46e9-ad99-01287325d607"), "Interest paid", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Expenses),
new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("affe0496-27d7-4091-9e89-02ccd38253a6"), "Licensing fees", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Expenses),
new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("5849e2c7-c773-401a-9f17-fc6858492b20"), "Lease_payments", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Expenses),
new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("b0cbec92-c78e-409c-b2a4-3b1b120bffd1"), "Legal_fees", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Expenses),
new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("8cf9d862-2d37-448a-a6b8-a32e1a53ebbe"), "Motor vehicle expenses", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Expenses),
new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("220638c5-0bbe-40c5-847d-cda80a26a461"), "Printing and stationery", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Expenses),
new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("83e3b994-45f1-4ab5-81ea-e8b6029bb55c"), "Purchases", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Expenses),
new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("8cbe039a-d688-40de-b4a1-0932513d8d11"), "Refunds paid", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Expenses),
new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("a11293cf-e3f2-4148-8c21-2fab20a50ecc"), "Rent", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Expenses),
new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("7b687bf2-ae43-4c45-bb29-8f217bcbc50b"), "Repairs and maintenance", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Expenses),
new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("e6798e01-b81c-4343-aa3d-1e71cfcf43ee"), "Staff amenities", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Expenses),
new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("339990cf-d36b-43b9-a4e3-41da12b886d4"), "Staff training", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Expenses),
new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("43df49b6-47f3-4fa5-9b4b-2ab8b06c4c91"), "Storage charges", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Expenses),
new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("05cd4766-b8f0-4cbe-916c-e104c06d4652"), "Subscriptions and memberships", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Expenses),
new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("5f34a5f0-4b2d-4b32-95b5-5b189dbbf5dc"), "Telephone and internet", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Expenses),
new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("24f2dff1-2554-4fbf-ad24-153110bc9457"), "Tools", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Expenses),
new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("f409d777-1b5b-4990-add9-b8f0e23bfb58"), "Travel and accommodation", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Expenses),
new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("5e889c0d-5f9d-4ec0-a34b-612a17d98168"), "Uniforms", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Expenses),
new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("b57d354c-76db-4a33-a2c6-15bf23a00613"), "Wages and salaries", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Expenses),
new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("87cb411b-717b-4db1-b2d7-b0624a19ec0c"), "Other expenses", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Expenses),

new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("6eb8d9f6-2a4c-4975-9470-b750754cec5b"), "Inventory on hand", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Assets),
new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("34466119-a12e-4c75-8472-6483b8e05c6a"), "Land and buildings", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Assets),
new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("f3ba7f5e-abb8-45d5-aa29-4b783f72b421"), "Furniture and equipment", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Assets),
new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("a90fcd36-5215-46da-9ec2-2ca826dadf9d"), "Plant and equipment", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Assets),
new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("b6dd86b4-90d8-4ce0-9527-8916bf44aacc"), "Investments", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Assets),

new Tuple<Guid, string, ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18>(new Guid("5f545a63-f303-4c8c-a73b-61eed135fbb5"), "Intra-company transfer", ManagerServer.Model.Obsolete.Obsolete18.ChartOfAccountsCategory18.Equity),
};

            var accountsInUse = new HashSet<Guid>();
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete02.Payment02>())
            {
                if (e.CreditAccount.HasValue) accountsInUse.Add(e.CreditAccount.Value);
                if (e.Lines != null) foreach (var e2 in e.Lines.Where(x => x.DebitAccount.HasValue)) accountsInUse.Add(e2.DebitAccount.Value);
            }
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete02.Receipt02>())
            {
                if (e.DebitAccount.HasValue) accountsInUse.Add(e.DebitAccount.Value);
                if (e.Lines != null) foreach (var e2 in e.Lines.Where(x => x.CreditAccount.HasValue)) accountsInUse.Add(e2.CreditAccount.Value);
            }

            var list = new List<Model.Object>();
            foreach (var e in accountsInUse)
            {
                if (e == new Guid("e0a84a1d-0ad9-4d77-9ba4-781b955e505f"))
                {
                    list.Add(new Model.Obsolete.Obsolete22.BankAccount22() { Key = e, Name = "Cash at bank" });
                }

                if (e == new Guid("3394ffa0-fa62-4fe5-8124-97a2a3f5508c"))
                {
                    list.Add(new Model.Obsolete.Obsolete22.CashAccount22() { Key = e, Name = "Cash on hand" });
                }

                if (e == new Guid("c6c98c29-23fa-414c-801d-029a63a28cb5"))
                {
                    list.Add(new Model.Obsolete.Obsolete22.CashAccount22() { Key = e, Name = "Petty cash" });
                }

                if (e == new Guid("5660c2c2-3a58-410b-b58d-cf2ea0d5f0e4"))
                {
                    list.Add(new Model.Obsolete.Obsolete22.BankAccount22() { Key = e, Name = "Bank loan" });
                }

                if (e == new Guid("594b82eb-92da-43f7-8a44-b630e5875aad"))
                {
                    list.Add(new Model.Obsolete.Obsolete22.BankAccount22() { Key = e, Name = "Credit card" });
                }

                var builtInAccount = builtInAccounts.SingleOrDefault(x => x.Item1 == e);
                if (builtInAccount != null)
                {
                    list.Add(new Model.Obsolete.Obsolete18.GeneralLedgerAccount18() { Key = e, Name = builtInAccount.Item2, Category = builtInAccount.Item3 });
                }
            }
            return list.ToArray();
        }
    }
}
