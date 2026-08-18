using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.CustomFields.TextCustomFields
{
    [ProtoContract]
    [NamespaceEntry]
    [Guid("e2876967-d8f9-4043-b436-66c6756d9b38")]
    [Title(nameof(Strings.TextCustomFields))]
    [Guide("Text custom fields are the most versatile custom field type for capturing textual information.")]
    [Guide("Use them for reference numbers, descriptions, notes, codes, or any text-based data that needs to be recorded on transactions and documents.")]
    [Guide("Text fields can be configured as single-line for short entries, paragraph for longer text, or dropdown lists for standardized choices.")]
    [Columns]
    internal sealed class TextCustomFields : NakedObjectsWithAutomaticRows<ManagerServer.Model.TextCustomField>
    {
        protected override void OnGetNewButton()
        {
            Write(Strings.NewCustomField);
        }

        [Default]
        [Guid("24719d5e-b02f-49a6-8ee6-bb9cc9e89a10")]
        [Guide("The name of each text custom field as it appears on forms and in reports.")]
        [Guide("Choose clear, descriptive names that indicate what information should be entered.")]
        public string[] GetName(ManagerServer.Model.TextCustomField[] rows)
        {
            return rows.Select(x => x.Name).ToArray();
        }

        [Default]
        [Guid("a9c40652-88d3-467d-ab5a-313a36eba270")]
        [Guide("Shows which forms and documents include this text field.")]
        [Guide("A single field can appear on multiple forms. For example, a *Project Code* field might appear on both sales invoices and expense claims.")]
        public string[] GetPlacement(ManagerServer.Model.TextCustomField[] rows)
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