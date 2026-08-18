using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagerServer.Query
{
    public static class CustomFieldExtensions
    {
        public static Dictionary<Guid, string> CopyCustomFields<T>(string entityId, Dictionary<Guid, string> source) where T : ManagerServer.Model.Object, new()
        {
            var customFields = new Dictionary<Guid, string>();
            var formDefault = ApplicationData.Instance.Businesses.Get(entityId).Single<T>();
            var customFieldsField = formDefault.GetType().GetFieldOrProperty("CustomFields");
            if (customFieldsField != null)
            {
                var customFieldsObject = customFieldsField.GetMemberValue(formDefault);
                if (customFieldsObject != null && customFieldsObject is Dictionary<Guid, string>)
                {
                    foreach (var e in (Dictionary<Guid, string>)customFieldsObject)
                    {
                        if (string.IsNullOrWhiteSpace(e.Value)) continue;
                        customFields.Add(e.Key, e.Value);
                    }
                }
            }

            if (source != null)
            {
                var destinationCustomFieldsByName = new Dictionary<string, Guid>();
                foreach (var e in ApplicationData.Instance.Businesses.Get(entityId).OfType<ManagerServer.Model.CustomField>().Where(x => x.Contains(typeof(T))).ToArray())
                {
                    if (!destinationCustomFieldsByName.ContainsKey(e.Name ?? string.Empty)) destinationCustomFieldsByName.Add(e.Name ?? string.Empty, e.Key);
                }

                foreach (var sourceCustomField in ApplicationData.Instance.Businesses.Get(entityId).OfType<ManagerServer.Model.CustomField>().Where(x => source.ContainsKey(x.Key) && !string.IsNullOrWhiteSpace(source[x.Key])))
                {
                    var sourceCustomFieldName = sourceCustomField.Name ?? string.Empty;
                    if (!destinationCustomFieldsByName.ContainsKey(sourceCustomFieldName)) continue;
                    if (!customFields.ContainsKey(destinationCustomFieldsByName[sourceCustomFieldName])) customFields.Add(destinationCustomFieldsByName[sourceCustomFieldName], string.Empty);
                    customFields[destinationCustomFieldsByName[sourceCustomFieldName]] = source[sourceCustomField.Key];
                }
            }

            return customFields;
        }
    }
}
