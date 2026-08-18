using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.CustomFields.ImageCustomFields
{
    [ProtoContract]
    [NamespaceEntry]
    [Guid("b3e013ab-5916-4be4-ad95-b0d6070d5dfc")]
    [Title(nameof(Strings.ImageCustomFields))]
    [Guide("Image custom fields allow users to upload and attach pictures, photos, and other visual content to various forms throughout the system.")]
    [Guide("Once created, these fields will appear on the forms you assign them to, enabling users to upload images directly within those forms.")]
    [Guide("Image fields are useful for attaching product photos, receipts, identification documents, or any other visual information relevant to your records.")]
    [Columns]
    internal sealed class ImageCustomFields : NakedObjectsWithAutomaticRows<ManagerServer.Model.ImageCustomField>
    {
        protected override void OnGetNewButton()
        {
            Write(Strings.NewCustomField);
        }

        [Default]
        [Guid("82369275-2829-4ab1-9d84-4809062e7bd9")]
        [Guide("The name identifies each image custom field and will be displayed as the field label on forms.")]
        [Guide("Choose descriptive names that clearly indicate what type of image should be uploaded, such as \"Product Photo\", \"Receipt Image\", or \"ID Document\".")]
        public string[] GetName(ManagerServer.Model.ImageCustomField[] rows)
        {
            return rows.Select(x => x.Name).ToArray();
        }

        [Default]
        [Guid("bec25999-16b0-411c-8397-ef1c8cc43104")]
        [Guide("Shows which forms include this image field, allowing users to upload images when creating or editing those specific records.")]
        [Guide("You can assign the same image field to multiple forms if you need the same type of image across different areas of the system.")]
        public string[] GetPlacement(ManagerServer.Model.ImageCustomField[] rows)
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