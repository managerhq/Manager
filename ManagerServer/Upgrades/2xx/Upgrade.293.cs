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
        private static async Task<IEnumerable<Model.Object>> Upgrade293(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.PayslipEarningsItem>())
            {
                if (e.CustomFields == null) continue;
                if (!e.CustomFields.ContainsKey(new Guid("8198caf2-1125-4f8e-9469-527583e3ea5e"))) continue;
                if (e.CustomFields[new Guid("8198caf2-1125-4f8e-9469-527583e3ea5e")] == "Lump sum A - Redundancy")
                {
                    e.CustomFields[new Guid("8198caf2-1125-4f8e-9469-527583e3ea5e")] = "Lump sum A";
                    list.Add(e);
                }
                if (e.CustomFields[new Guid("8198caf2-1125-4f8e-9469-527583e3ea5e")] == "Lump sum A - Termination")
                {
                    e.CustomFields[new Guid("8198caf2-1125-4f8e-9469-527583e3ea5e")] = "Lump sum A";
                    list.Add(e);
                }
            }

            var atoReportingCategory = objects.SingleOrDefault<CustomField>(new Guid("8198caf2-1125-4f8e-9469-527583e3ea5e"));
            if (atoReportingCategory != null)
            {
                var atoReportingCategory2 = Localizations.Localizations.Json.SelectMany(x => x.Value).SingleOrDefault(x => x.Key == new Guid("8198caf2-1125-4f8e-9469-527583e3ea5e"));
                if (atoReportingCategory2 != null) list.Add(atoReportingCategory2);
            }

            var reportTransformation = objects.SingleOrDefault<ManagerServer.Model.Obsolete.Obsolete74.ReportTransformation>(new Guid("07332ba3-3e82-4dc1-9451-1350f5d84e24"));
            if (reportTransformation != null)
            {
                var givenName = new Guid("0713b751-ee07-4d22-8b17-ad0131029ca0");
                var middleName = new Guid("91a2b722-99cd-41e1-b638-532793aae782");
                var familyName = new Guid("11acbfb3-9557-487a-8b8b-2528a2c43c53");

                var address1 = new Guid("57a258c7-3296-4b4a-bc6c-82ba5755c222");
                var address2 = new Guid("ce735064-e907-478f-83b0-f319b2a9a7fb");
                var suburb = new Guid("f6326e38-61c9-41c6-8bda-7a9f2d594150");
                var state = new Guid("050a3828-8ba0-4db9-b560-f24a3c5e413b");
                var postcode = new Guid("dc15193e-9883-4e89-a78b-7e6751db5240");
                var country = new Guid("343a8633-5d10-46ca-9d20-0beed32ebab8");

                var localizations = Localizations.Localizations.Json.SelectMany(x => x.Value).OfType<ManagerServer.Model.CustomField>().Where(x => x.Obsolete_FormType == ManagerServer.Model.Object.GetGuidByType(typeof(Employee))).ToDictionary(x => x.Key);
                foreach (var e in new Guid[] { givenName, middleName, familyName, address1, address2, suburb, state, postcode, country })
                {
                    if (localizations.ContainsKey(e)) list.Add(localizations[e]);
                }

                foreach (var e in objects.OfType<Employee>())
                {
                    if (e.CustomFields == null) e.CustomFields = new Dictionary<Guid, string>();

                    if (!e.CustomFields.ContainsKey(givenName) && !e.CustomFields.ContainsKey(middleName) && !e.CustomFields.ContainsKey(familyName))
                    {
                        if (!string.IsNullOrWhiteSpace(e.Name))
                        {
                            var nameParts = e.Name.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                            if (nameParts.Length > 0) e.CustomFields.Add(givenName, nameParts.First());
                            if (nameParts.Length > 1) e.CustomFields.Add(familyName, nameParts.Last());
                            if (nameParts.Length > 2) e.CustomFields.Add(middleName, string.Join(' ', nameParts.Skip(1).Take(nameParts.Length - 2)));
                        }
                    }

                    if (!e.CustomFields.ContainsKey(address1))
                    {
                        if (!string.IsNullOrWhiteSpace(e.Address))
                        {
                            var addressParts = e.Address.Split('\n');
                            if (addressParts.Length >= 2)
                            {
                                e.CustomFields.Add(address1, addressParts[0]);
                                if (addressParts.Length >= 3) e.CustomFields.Add(address2, addressParts[1]);

                                var lastLineParts = addressParts.Last().Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                                if (lastLineParts.Length >= 3)
                                {
                                    e.CustomFields.Add(postcode, lastLineParts.Last());
                                    e.CustomFields.Add(suburb, string.Join(' ', lastLineParts.Take(lastLineParts.Length - 2)));
                                    e.CustomFields.Add(state, lastLineParts.Skip(lastLineParts.Length - 2).First());
                                    e.CustomFields.Add(country, "au — Australia");
                                }
                            }
                        }
                    }

                    list.Add(e);

                }
            }
            return list;
        }
    }
}
