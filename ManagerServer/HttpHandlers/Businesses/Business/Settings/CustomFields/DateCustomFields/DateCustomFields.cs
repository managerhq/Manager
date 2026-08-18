using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.CustomFields.DateCustomFields
{
    [ProtoContract]
    [NamespaceEntry]
    [Guid("af87db0d-19b0-4207-955e-94db714e2ab3")]
    [Title(nameof(Strings.DateCustomFields))]
    [Guide("Date custom fields capture time-based information with an easy-to-use calendar picker.")]
    [Guide("Perfect for tracking expiry dates, contract dates, review dates, or any date-based milestones.")]
    [Guide("Date fields help you monitor time-sensitive information and can be used in reports for date-based analysis.")]
    [Header("Setting up Date Custom Fields")]
    [Guide("To create a new date custom field, click the **New Custom Field** button.")]
    [Guide("Each date field you create can be placed on specific forms and documents where you need to track date information.")]
    [Columns]
    internal sealed class DateCustomFields : NakedObjectsWithAutomaticRows<ManagerServer.Model.DateCustomField>
    {
        protected override void OnGetNewButton()
        {
            Write(Strings.NewCustomField);
        }

        [Default]
        [Guid("6b05dace-7ef3-497d-8a99-aaaa996aab04")]
        [Guide("The name of each date custom field as it appears on forms.")]
        [Guide("Use descriptive names like *Contract Expiry Date* or *Next Review Date* that clearly indicate what date is being tracked.")]
        public string[] GetName(ManagerServer.Model.DateCustomField[] rows)
        {
            return rows.Select(x => x.Name).ToArray();
        }

        [Default]
        [Guid("d2336bfd-2ec8-4d15-a5a3-b6632bba0b8a")]
        [Guide("Shows which forms and documents include this date field.")]
        [Guide("Date fields can track different dates on different forms — warranty dates on assets, review dates on customers, or renewal dates on contracts.")]
        [Guide("Select multiple forms if you want the same date field to appear across different transaction types.")]
        public string[] GetPlacement(ManagerServer.Model.DateCustomField[] rows)
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