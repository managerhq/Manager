using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.ObsoleteFeatures.ClassicCustomFields
{
    [ProtoContract]
    [NamespaceEntry]
    [Title(nameof(Strings.ClassicCustomFields))]
    [Guide("Classic custom fields were the initial version of custom fields in Manager. This feature is now obsolete and has been replaced with an improved custom fields system.")]
    [Header("Important Notice")]
    [Guide("We strongly advise against using classic custom fields. The new custom fields system offers better functionality and improved performance.")]
    [LinkGuide("Learn more about the new custom fields:", typeof(CustomFields.CustomFields))]
    [Header("Upgrading Your Custom Fields")]
    [Guide("To convert your classic custom fields to the new system, click the **Upgrade** button located in the bottom-right corner of the Classic Custom Fields screen.")]
    [SmallBottomButtonScreenshot(nameof(Strings.Upgrade))]
    [LinkGuide("For detailed upgrade instructions, see:", typeof(UpgradeClassicCustomField))]
    [Guid("9d751302-1d1e-4eef-b895-50fff4d8c1a1")]
    internal sealed class ClassicCustomFields : NakedObjectsWithAutomaticRows<ManagerServer.Model.CustomField>
    {
        protected override void OnGetNewButton()
        {
            Write(Strings.NewCustomField);
        }

        [Default]
        [Guid("f2ee8cac-d548-485f-b76f-817e27c5e497")]
        public string[] GetName(ManagerServer.Model.CustomField[] rows)
        {
            return rows.Select(x => x.Name).ToArray();
        }

        [Default]
        [Guid("03c552d9-f667-4a64-be02-b45d5585a3c3")]
        public string[] GetPlacement(ManagerServer.Model.CustomField[] rows)
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

        protected override void OnFooterEndSection(Context context)
        {
            var httpHandler = new UpgradeClassicCustomField() { Business = Business, Referrer = this.ToUrl() };
            using (A(href: httpHandler.ToUrl(), @class: "btn btn-xs"))
            {
                Write(Strings.Upgrade);
            }
            
            base.OnFooterEndSection(context);
        }
    }
}
