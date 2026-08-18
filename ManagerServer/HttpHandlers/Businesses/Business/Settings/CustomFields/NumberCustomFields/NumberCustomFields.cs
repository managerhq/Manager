using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.CustomFields.NumberCustomFields
{
    [ProtoContract]
    [NamespaceEntry]
    [Guid("af2a2016-2e03-4455-a0cd-b9771b9aa0d1")]
    [Title(nameof(Strings.NumberCustomFields))]
    [Guide("Number custom fields capture numerical data like quantities, measurements, scores, or ratings.")]
    [Guide("Number fields on line items automatically calculate totals, making them perfect for tracking additional quantities.")]
    [Guide("Use them for weights, dimensions, hours, units, or any measurable values specific to your business.")]
    [Columns]
    internal sealed class NumberCustomFields : NakedObjectsWithAutomaticRows<ManagerServer.Model.NumberCustomField>
    {
        protected override void OnGetNewButton()
        {
            Write(Strings.NewCustomField);
        }

        [Default]
        [Guid("fa8ee9b6-876d-4d26-ab3f-7a23e1f78d62")]
        [Guide("The name of each *number custom field* as displayed on forms.")]
        [Guide("Include units in the name when applicable, like 'Weight (kg)' or 'Hours Worked'.")]
        public string[] GetName(ManagerServer.Model.NumberCustomField[] rows)
        {
            return rows.Select(x => x.Name).ToArray();
        }

        [Default]
        [Guid("582480f9-dcd3-4a59-96b5-c8148c2f53ab")]
        [Guide("Shows which forms include this *number field*.")]
        [Guide("Number fields on *line items* will show totals; fields on headers capture single values.")]
        public string[] GetPlacement(ManagerServer.Model.NumberCustomField[] rows)
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