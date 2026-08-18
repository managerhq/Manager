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
        private static async Task<IEnumerable<Model.Object>> Upgrade179(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();

            var customers = objects.OfType<Model.Customer>().ToArray();
            if (customers.Any())
            {
                var telephone = customers.Any(x => !string.IsNullOrWhiteSpace(x.Obsolete_Telephone));
                var fax = customers.Any(x => !string.IsNullOrWhiteSpace(x.Obsolete_Fax));
                var mobile = customers.Any(x => !string.IsNullOrWhiteSpace(x.Obsolete_Mobile));
                var notes = customers.Any(x => !string.IsNullOrWhiteSpace(x.Obsolete_Notes));

                var telephoneCustomFieldKey = new Guid("e8e8dd97-e5f9-4bb7-a5e9-390b8c56923e");
                var faxCustomFieldKey = new Guid("6c267cb6-4f53-437a-ac44-473d1f1d7ba3");
                var mobileCustomFieldKey = new Guid("71d894fb-05ed-4e73-80ed-abb7e6383bb6");
                var notesCustomFieldKey = new Guid("5725953b-8544-478c-baf0-0996d60c399c");

                if (telephone)
                {
                    list.Add(new ManagerServer.Model.CustomField() { Key = telephoneCustomFieldKey, Position = 1, Name = "Telephone", Size = CustomFieldSize.Medium, Type = CustomFieldStyle.SingleLineText, Obsolete_FormType = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.Customer)), Obsolete_DisplayOnList = true, DisplayOnView = true });
                }
                if (fax)
                {
                    list.Add(new ManagerServer.Model.CustomField() { Key = faxCustomFieldKey, Position = 2, Name = "Fax", Size = CustomFieldSize.Medium, Type = CustomFieldStyle.SingleLineText, Obsolete_FormType = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.Customer)) });
                }
                if (mobile)
                {
                    list.Add(new ManagerServer.Model.CustomField() { Key = mobileCustomFieldKey, Position = 3, Name = "Mobile number", Size = CustomFieldSize.Medium, Type = CustomFieldStyle.SingleLineText, Obsolete_FormType = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.Customer)) });
                }
                if (notes)
                {
                    list.Add(new ManagerServer.Model.CustomField() { Key = notesCustomFieldKey, Position = 4, Name = "Notes", Size = CustomFieldSize.Medium, Type = CustomFieldStyle.ParagraphText, Obsolete_FormType = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.Customer)) });
                }

                foreach (var e in customers)
                {
                    if (e.CustomFields == null) e.CustomFields = new Dictionary<Guid, string>();
                    var count = e.CustomFields.Count;
                    if (!string.IsNullOrWhiteSpace(e.Obsolete_Telephone)) e.CustomFields.Add(telephoneCustomFieldKey, e.Obsolete_Telephone);
                    if (!string.IsNullOrWhiteSpace(e.Obsolete_Fax)) e.CustomFields.Add(faxCustomFieldKey, e.Obsolete_Fax);
                    if (!string.IsNullOrWhiteSpace(e.Obsolete_Mobile)) e.CustomFields.Add(mobileCustomFieldKey, e.Obsolete_Mobile);
                    if (!string.IsNullOrWhiteSpace(e.Obsolete_Notes)) e.CustomFields.Add(notesCustomFieldKey, e.Obsolete_Notes);
                    if (e.CustomFields.Count > count) list.Add(e);
                }
            }

            var suppliers = objects.OfType<Model.Supplier>().ToArray();
            if (suppliers.Any())
            {
                var telephone = suppliers.Any(x => !string.IsNullOrWhiteSpace(x.Obsolete_Telephone));
                var fax = suppliers.Any(x => !string.IsNullOrWhiteSpace(x.Obsolete_Fax));
                var mobile = suppliers.Any(x => !string.IsNullOrWhiteSpace(x.Obsolete_Mobile));
                var notes = suppliers.Any(x => !string.IsNullOrWhiteSpace(x.Obsolete_Notes));

                var telephoneCustomFieldKey = new Guid("33e4168b-d19a-47db-8f36-a729f7002a76");
                var faxCustomFieldKey = new Guid("add44037-3015-4b21-8985-198b6e82f249");
                var mobileCustomFieldKey = new Guid("2dc19d65-cb6d-4355-80b3-85d079df269d");
                var notesCustomFieldKey = new Guid("bc796239-897f-45d5-93d4-2471ca64a68e");

                if (telephone)
                {
                    list.Add(new ManagerServer.Model.CustomField() { Key = telephoneCustomFieldKey, Position = 1, Name = "Telephone", Size = CustomFieldSize.Medium, Type = CustomFieldStyle.SingleLineText, Obsolete_FormType = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.Supplier)), Obsolete_DisplayOnList = true, DisplayOnView = true });
                }
                if (fax)
                {
                    list.Add(new ManagerServer.Model.CustomField() { Key = faxCustomFieldKey, Position = 2, Name = "Fax", Size = CustomFieldSize.Medium, Type = CustomFieldStyle.SingleLineText, Obsolete_FormType = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.Supplier)) });
                }
                if (mobile)
                {
                    list.Add(new ManagerServer.Model.CustomField() { Key = mobileCustomFieldKey, Position = 3, Name = "Mobile number", Size = CustomFieldSize.Medium, Type = CustomFieldStyle.SingleLineText, Obsolete_FormType = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.Supplier)) });
                }
                if (notes)
                {
                    list.Add(new ManagerServer.Model.CustomField() { Key = notesCustomFieldKey, Position = 4, Name = "Notes", Size = CustomFieldSize.Medium, Type = CustomFieldStyle.ParagraphText, Obsolete_FormType = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.Supplier)) });
                }

                foreach (var e in suppliers)
                {
                    if (e.CustomFields == null) e.CustomFields = new Dictionary<Guid, string>();
                    var count = e.CustomFields.Count;
                    if (!string.IsNullOrWhiteSpace(e.Obsolete_Telephone)) e.CustomFields.Add(telephoneCustomFieldKey, e.Obsolete_Telephone);
                    if (!string.IsNullOrWhiteSpace(e.Obsolete_Fax)) e.CustomFields.Add(faxCustomFieldKey, e.Obsolete_Fax);
                    if (!string.IsNullOrWhiteSpace(e.Obsolete_Mobile)) e.CustomFields.Add(mobileCustomFieldKey, e.Obsolete_Mobile);
                    if (!string.IsNullOrWhiteSpace(e.Obsolete_Notes)) e.CustomFields.Add(notesCustomFieldKey, e.Obsolete_Notes);
                    if (e.CustomFields.Count > count) list.Add(e);
                }
            }

            var employees = objects.OfType<Model.Employee>().ToArray();
            if (employees.Any())
            {
                var telephone = employees.Any(x => !string.IsNullOrWhiteSpace(x.Obsolete_Telephone));
                var mobile = employees.Any(x => !string.IsNullOrWhiteSpace(x.Obsolete_Mobile));
                var notes = employees.Any(x => !string.IsNullOrWhiteSpace(x.Obsolete_Notes));

                var telephoneCustomFieldKey = new Guid("18327127-5362-4949-8f35-468dd93bc4ca");
                var mobileCustomFieldKey = new Guid("33065dfc-365f-47a8-8bfe-fc7ecd62545f");
                var notesCustomFieldKey = new Guid("69d7fdc3-dcd0-42a9-b840-76be4e326d5f");

                if (telephone)
                {
                    list.Add(new ManagerServer.Model.CustomField() { Key = telephoneCustomFieldKey, Position = 1, Name = "Telephone", Size = CustomFieldSize.Medium, Type = CustomFieldStyle.SingleLineText, Obsolete_FormType = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.Employee)), Obsolete_DisplayOnList = true, DisplayOnView = true });
                }
                if (mobile)
                {
                    list.Add(new ManagerServer.Model.CustomField() { Key = mobileCustomFieldKey, Position = 2, Name = "Mobile number", Size = CustomFieldSize.Medium, Type = CustomFieldStyle.SingleLineText, Obsolete_FormType = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.Employee)), Obsolete_DisplayOnList = true, DisplayOnView = true });
                }
                if (notes)
                {
                    list.Add(new ManagerServer.Model.CustomField() { Key = notesCustomFieldKey, Position = 3, Name = "Notes", Size = CustomFieldSize.Medium, Type = CustomFieldStyle.ParagraphText, Obsolete_FormType = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.Employee)) });
                }

                foreach (var e in employees)
                {
                    if (e.CustomFields == null) e.CustomFields = new Dictionary<Guid, string>();
                    var count = e.CustomFields.Count;
                    if (!string.IsNullOrWhiteSpace(e.Obsolete_Telephone)) e.CustomFields.Add(telephoneCustomFieldKey, e.Obsolete_Telephone);
                    if (!string.IsNullOrWhiteSpace(e.Obsolete_Mobile)) e.CustomFields.Add(mobileCustomFieldKey, e.Obsolete_Mobile);
                    if (!string.IsNullOrWhiteSpace(e.Obsolete_Notes)) e.CustomFields.Add(notesCustomFieldKey, e.Obsolete_Notes);
                    if (e.CustomFields.Count > count) list.Add(e);
                }
            }

            return list;
        }
    }
}
