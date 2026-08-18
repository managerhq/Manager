using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.CustomFields.MultipleValueCustomFields
{
    [ProtoContract]
    [NamespaceEntry]
    [Guid("485344ae-9ed9-46c0-a45c-e70d6e0d9d29")]
    [Title(nameof(Strings.MultipleValueCustomFields))]
    [Guide("*Multiple value custom fields* allow users to select multiple options from a predefined list.")]
    [Guide("These fields are perfect for tags, categories, features, or any attribute where multiple selections apply.")]
    [Guide("Unlike dropdown lists where users can only choose one option, multiple value fields let users select several options simultaneously.")]
    [Guide("For example, you could use them to track multiple product features, service types, or customer interests.")]
    [Columns]
    internal sealed class MultipleValueCustomFields : NakedObjectsWithAutomaticRows<ManagerServer.Model.MultipleValueCustomField>
    {
        protected override void OnGetNewButton()
        {
            Write(Strings.NewCustomField);
        }

        [Default]
        [Guid("8e820419-e3fa-4623-adfc-104c5abef327")]
        [Guide("The name of each multiple value field as it appears on forms throughout the system.")]
        [Guide("Use plural names when appropriate, such as 'Product Features' or 'Service Categories', to indicate that multiple selections are possible.")]
        public string[] GetName(ManagerServer.Model.MultipleValueCustomField[] rows)
        {
            return rows.Select(x => x.Name).ToArray();
        }

        [Default]
        [Guid("4ef5448c-a30d-4a62-8a8d-1dcb5d59a45c")]
        [Guide("Shows which forms include this multiple value field.")]
        [Guide("When users make selections, the chosen values appear as comma-separated lists on forms and in reports.")]
        [Guide("You can use these fields for filtering and grouping data in reports, making it easy to analyze records by selected attributes.")]
        public string[] GetPlacement(ManagerServer.Model.MultipleValueCustomField[] rows)
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
