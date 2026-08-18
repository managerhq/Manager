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
        private static async Task<IEnumerable<Model.Object>> Upgrade308(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete74.ReportTransformation>().ToArray())
            {
                var extension = new ManagerServer.Model.ScriptExtension()
                {
                    Name = e.Name,
                    Script = @"document.getElementById('" + e.Key.ToString() + "').style.display = 'block'",
                    Location = LocationType.Custom,
                    CustomLocation = "reports-view"
                };

                if (e.Key == new Guid("11acbfe1-0d24-4161-b366-fe905f2bcfd9")) extension.Key = new Guid("da06cb58-591d-41df-92fe-a5be3f16c93a"); // Business Activity Statement
                if (e.Key == new Guid("92b38154-38fc-479a-a296-2019f656d1e2")) extension.Key = new Guid("de9ae1f1-df47-4298-a702-dc509f75eb06"); // PAYG payment summary — individual non-business
                if (e.Key == new Guid("07332ba3-3e82-4dc1-9451-1350f5d84e24")) extension.Key = new Guid("afb2997f-2213-4db4-b871-7192d4ef4564"); // Single Touch Payroll Worksheet
                if (e.Key == new Guid("c4a0ccf7-9171-4e8e-b390-97f7052b1479")) extension.Key = new Guid("72766a67-2dab-4cb3-95ce-4faeba933b25"); // Taxable Payments Annual Report (TPAR)

                if (e.Key == new Guid("b755a3ef-32aa-4eab-8936-0e48b057f627")) extension.Key = new Guid("3df91926-ee38-4c0a-91ef-41f82bbf6e32"); // Concept BTW Aangifte
                if (e.Key == new Guid("994cef79-6da3-4fa1-9998-ad029a4358f0")) extension.Key = new Guid("f7fb2f55-8930-4ae0-9e91-9fb1ae0b51e1"); // GST Return
                if (e.Key == new Guid("734f9a89-b048-46c5-b792-e652057c381f")) extension.Key = new Guid("40dc02e6-b9e2-456e-a3b0-f3bb45aae9b4"); // VAT Return Form
                if (e.Key == new Guid("12e5e9fb-d8e8-4fce-aa33-8ba564117550")) extension.Key = new Guid("732d52fa-05f0-439c-a601-cc22a46ea795"); // VAT Calculation Worksheet
                if (e.Key == new Guid("3c7e1105-c3ef-4aa0-9c9e-282c548dd29e")) extension.Key = new Guid("9c780537-d36d-4b72-a0cc-36219801f111"); // ДДВ-04

                if (objects.SingleOrDefault<ManagerServer.Model.ScriptExtension>(extension.Key) == null) list.Add(extension);
            }
            return list;
        }
    }
}
