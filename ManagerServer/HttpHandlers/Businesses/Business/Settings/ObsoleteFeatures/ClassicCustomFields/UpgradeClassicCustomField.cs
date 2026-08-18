using System;using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.ObsoleteFeatures.ClassicCustomFields
{
    [ProtoContract]
    [Title(nameof(Strings.ClassicCustomFields), nameof(Strings.Upgrade))]
    [Guide("*Classic custom fields* are now outdated. You can update them to the new custom fields through this screen.")]
    [Header("Upgrade Process")]
    [Guide("Choose the classic custom field you want to upgrade, and then click the **Upgrade** button.")]
    [PrimaryButtonScreenshot(nameof(Strings.Upgrade))]
    [Guide("Your existing custom fields will be transferred to the new custom fields system.")]
    [LinkGuide("Learn more:", typeof(CustomFields.CustomFields))]
    [Header("After Upgrading")]
    [Guide("Please ensure that the upgrade has not caused any issues. If you encounter any problems, you can reverse the operation by selecting **Undo** in the **History** section.")]
    [LinkGuide("Learn more:", typeof(History))]
    internal sealed class UpgradeClassicCustomField : BusinessTemplate
    {
        protected override void InnerGet2()
        {
            using (PostForm())
            {
                using (Div(@class: "card"))
                {
                    using (Div(@class: "card-header"))
                    {
                        using (Div(@class: "card-title")) Write(Strings.ClassicCustomFields);
                    }

                    using (Div(@class: "card-form"))
                    {
                        using (Div(@class: "form-group"))
                        {
                            using (Label()) Write(Strings.CustomField);
                            using (Div(@class: "flex"))
                            {
                                using (Select(name: "Key", @class: "form-select", style: "width: 300px"))
                                {
                                    Option();
                                    foreach (var e in ApplicationData.Businesses.Get(Business).OfType<CustomField>().Where(x => x.Type == ManagerServer.Model.Enums.CustomFieldStyle.Date || x.Type == ManagerServer.Model.Enums.CustomFieldStyle.SingleLineText || x.Type == ManagerServer.Model.Enums.CustomFieldStyle.ParagraphText || x.Type == ManagerServer.Model.Enums.CustomFieldStyle.Number || x.Type == ManagerServer.Model.Enums.CustomFieldStyle.DropdownList))
                                    {
                                        Option(value: e.Key.ToString(), text: e.Name);
                                    }
                                }
                            }
                        }
                    }

                    using (Div(@class: "card-header"))
                    {
                        using (Div()) using (PrimaryButton()) Write(Strings.Upgrade);
                    }
                }
            }
        }

        protected override async Task InnerPost()
        {
            var form = await Request.ReadFormAsync();

            if (!Guid.TryParse(form["Key"], out Guid key))
            {
                Response.Redirect(this.ToUrl());
                return;
            }

            var database = ApplicationData.Businesses.Get(Business);
            var classicCustomField = database.SingleOrDefault<CustomField>(key);

            if (classicCustomField == null)
            {
                Response.Redirect(this.ToUrl());
                return;
            }

            var list = new List<ManagerServer.Model.Object>();

            ICustomField customField = null;
            if (classicCustomField.Type == ManagerServer.Model.Enums.CustomFieldStyle.Date) customField = new DateCustomField() { Key = classicCustomField.Key, Name = classicCustomField.Name, Description = classicCustomField.Description, DisplayOnView = classicCustomField.DisplayOnView, Placement = classicCustomField.Placement, Inactive = classicCustomField.Inactive, Position = classicCustomField.Position, ShowAtTheTop = classicCustomField.ShowAtTheTop };
            if (classicCustomField.Type == ManagerServer.Model.Enums.CustomFieldStyle.Number) customField = new NumberCustomField() { Key = classicCustomField.Key, Name = classicCustomField.Name, Description = classicCustomField.Description, DisplayOnView = classicCustomField.DisplayOnView, Placement = classicCustomField.Placement, Inactive = classicCustomField.Inactive, Position = classicCustomField.Position, ShowAtTheTop = classicCustomField.ShowAtTheTop };
            if (classicCustomField.Type == ManagerServer.Model.Enums.CustomFieldStyle.ParagraphText) customField = new TextCustomField() { Key = classicCustomField.Key, Name = classicCustomField.Name, Description = classicCustomField.Description, DisplayOnView = classicCustomField.DisplayOnView, Placement = classicCustomField.Placement, Inactive = classicCustomField.Inactive, Position = classicCustomField.Position, ShowAtTheTop = classicCustomField.ShowAtTheTop, Type = ManagerServer.Model.Enums.TextCustomFieldType.ParagraphText };
            if (classicCustomField.Type == ManagerServer.Model.Enums.CustomFieldStyle.SingleLineText) customField = new TextCustomField() { Key = classicCustomField.Key, Name = classicCustomField.Name, Description = classicCustomField.Description, DisplayOnView = classicCustomField.DisplayOnView, Placement = classicCustomField.Placement, Inactive = classicCustomField.Inactive, Position = classicCustomField.Position, ShowAtTheTop = classicCustomField.ShowAtTheTop, Type = ManagerServer.Model.Enums.TextCustomFieldType.SingleLineText };
            if (classicCustomField.Type == ManagerServer.Model.Enums.CustomFieldStyle.DropdownList) customField = new TextCustomField() { Key = classicCustomField.Key, Name = classicCustomField.Name, Description = classicCustomField.Description, DisplayOnView = classicCustomField.DisplayOnView, Placement = classicCustomField.Placement, Inactive = classicCustomField.Inactive, Position = classicCustomField.Position, ShowAtTheTop = classicCustomField.ShowAtTheTop, OptionsForDropdownList = classicCustomField.OptionsForDropdownList, Type = ManagerServer.Model.Enums.TextCustomFieldType.DropdownList };

            list.Add((ManagerServer.Model.Object)customField);

            if (classicCustomField.Placement != null)
            {
                foreach (var e in classicCustomField.Placement)
                {
                    var type = ManagerServer.Model.Attributes.GuidAttribute.GetTypeByGuid(e);
                    if (type != null)
                    {
                        if (type.IsNested)
                        {
                            var parentType = type.DeclaringType;
                            var fields = parentType.GetFieldsAndProperties().Where(x => x.GetMemberType().IsArray && x.GetMemberType().GetElementType() == type).ToArray();
                            if (fields.Length == 0) throw new Exception(type.Name);
                            foreach (var e2 in database.UnorderedOfType<ManagerServer.Model.Object>().Where(x => x.GetType() == parentType).OfType<ICustomFields>())
                            {
                                foreach (var e3 in fields)
                                {
                                    var array = e3.GetMemberValue(e2) as Array;
                                    if (array != null)
                                    {
                                        for (int i = 0; i < array.Length; i++)
                                        {
                                            var element = array.GetValue(i) as ITransactionLine;
                                            if (element != null)
                                            {
                                                if (element.GetCustomFields2() == null)
                                                {
                                                    element.GetType().GetFieldOrProperty("CustomFields2").SetMemberValue(element, new ManagerServer.Model.CustomFields());
                                                }
                                                if (Convert(element.GetCustomFields(), element.GetCustomFields2(), customField))
                                                {
                                                    list.Add(e2 as ManagerServer.Model.Object);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (var e2 in database.UnorderedOfType<ManagerServer.Model.Object>().Where(x => x.GetType() == type).OfType<ICustomFields>())
                            {
                                if (e2.CustomFields == null)
                                {
                                    e2.GetType().GetFieldOrProperty("CustomFields2").SetMemberValue(e2, new ManagerServer.Model.CustomFields());
                                }
                                if (Convert(e2.ClassicCustomFields, e2.CustomFields, customField))
                                {
                                    list.Add(e2 as ManagerServer.Model.Object);
                                }
                            }
                        }
                    }
                }
            }

            ApplicationData.Businesses.Process(Business, list.Distinct().ToArray(), GetUserName());

            ApplicationData.Businesses.Get(Business).Invalidate<ManagerServer.Model.CustomField>();

            Response.Redirect(this.ToUrl());
        }

        private bool Convert(Dictionary<Guid, string> classicCustomFields, ManagerServer.Model.CustomFields customFields, ICustomField customField)
        {
            if (classicCustomFields != null && classicCustomFields.TryGetValue(customField.Key, out string value))
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (customField is DateCustomField dateCustomField)
                    {
                        if (DateTime.TryParse(value, System.Globalization.CultureInfo.InvariantCulture, out DateTime date))
                        {
                            if (customFields.Dates == null) customFields.Dates = new Dictionary<Guid, DateTime?>();
                            customFields.Dates[dateCustomField.Key] = date;
                            return true;
                        }
                    }
                    if (customField is TextCustomField textCustomField)
                    {
                        if (customFields.Strings == null) customFields.Strings = new Dictionary<Guid, string>();
                        customFields.Strings[textCustomField.Key] = value;
                        return true;
                    }
                    if (customField is NumberCustomField numberCustomField)
                    {
                        if (decimal.TryParse(value, System.Globalization.CultureInfo.InvariantCulture, out decimal number))
                        {
                            if (customFields.Decimals == null) customFields.Decimals = new Dictionary<Guid, decimal?>();
                            customFields.Decimals[numberCustomField.Key] = number;
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
