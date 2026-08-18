using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.CustomFields.CheckboxCustomFields
{
    [ProtoContract]
    [NamespaceEntry]
    [Guid("e6ec9e1a-51bd-494b-92a8-69713c6183b3")]
    [Title(nameof(Strings.CheckboxCustomFields))]
    [Guide("Checkbox custom fields create simple yes/no switches for tracking binary attributes across your business data.")]
    [Guide("Use them for flags such as *Tax Exempt*, *Requires Approval*, *Priority Customer*, or *Special Handling Required*.")]
    [Guide("Checkboxes make data entry quick and ensure consistent recording of yes/no information throughout the system.")]
    [Columns]
    internal sealed class CheckboxCustomFields : NakedObjectsWithAutomaticRows<ManagerServer.Model.CheckboxCustomField>
    {
        protected override void OnGetNewButton()
        {
            Write(Strings.NewCustomField);
        }

        [Default]
        [Guid("45e24c1d-3387-441e-bf5e-52435ee466f3")]
        [Guide("The name identifies each checkbox field and should be phrased as a clear statement or question.")]
        [Guide("Examples: *Tax Exempt*, *Requires Manager Approval*, *Is Fragile*, *Preferred Customer*.")]
        public string[] GetName(ManagerServer.Model.CheckboxCustomField[] rows)
        {
            return rows.Select(x => x.Name).ToArray();
        }

        [Default]
        [Guid("23e09472-9377-4196-a22e-f1d4c5ea173b")]
        [Guide("Shows which forms display this checkbox field.")]
        [Guide("The same checkbox can appear on multiple forms, allowing you to track consistent attributes across different areas of your business.")]
        public string[] GetPlacement(ManagerServer.Model.CheckboxCustomField[] rows)
        {
            var output = new string[rows.Length];
            for (int i = 0; i < rows.Length; i++)
            {
                var values = new List<string>();
                if (rows[i].Placement != null)
                {
                    foreach (var e in rows[i].Placement)
                    {
                        var type = ManagerServer.Model.Attributes.GuidAttribute.GetTypeByGuid(e);
                        if (type != null)
                        {
                            values.Add(ManagerServer.Globalization.Strings.GetPropertyValue(type));
                        }
                        else
                        {
                            values.Add(e.ToString());
                        }
                    }
                }
                output[i] = string.Join(", ", values);
            }
            return output;
        }
    }
}