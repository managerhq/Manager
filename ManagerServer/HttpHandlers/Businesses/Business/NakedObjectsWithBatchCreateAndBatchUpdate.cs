using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CsvHelper;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Model.Attributes;
using ManagerServer.Helpers;
using MemberInfo = System.Reflection.MemberInfo;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    internal abstract class NakedObjectsWithBatchCreateAndBatchUpdate<T> : NakedObjectsWithBatchRecode<T> where T : ManagerServer.Model.Object, new()
    {
        [InheritedProtoMember(280)] public bool BatchCreate;
        [InheritedProtoMember(281)] public bool BatchUpdate;

        protected override void InnerGet4(Context context)
        {
            if (BatchUpdate)
            {
                if (Request.HasFormContentType)
                {
                    var form = Request.ReadFormAsync().GetAwaiter().GetResult();

                    if (form.ContainsKey("BatchUpdateKey"))
                    {
                        var item = form["BatchUpdateKey"].ToString();
                        if (!string.IsNullOrWhiteSpace(item))
                        {
                            var keys = new HashSet<Guid>(item.Split(',').Select(x => new Guid(Convert.FromBase64String(x))).ToArray());
                            BatchCreateOrUpdate(keys);
                            return;
                        }
                    }
                }

                var cancelHandler = (NakedObjectsWithBatchCreateAndBatchUpdate<T>)this.MemberwiseClone();
                cancelHandler.BatchUpdate = false;

                context.Set(new BatchOperation()
                {
                    Name = Strings.BatchUpdate,
                    Cancel = cancelHandler
                });
            }
            else if (BatchCreate)
            {
                BatchCreateOrUpdate(null);
                return;
            }

            base.InnerGet4(context);
        }

        public override Tuple<string, byte[]>[] GetBatchOperation(T[] rows)
        {
            if (BatchUpdate)
            {
                var list = new List<Tuple<string, byte[]>>();
                foreach (var e in rows)
                {
                    list.Add(new Tuple<string, byte[]>("BatchUpdateKey", e.Key.ToByteArray()));
                }
                return list.ToArray();
            }
            return base.GetBatchOperation(rows);
        }

        private void BatchCreateOrUpdate(HashSet<Guid> keys)
        {
            using (Div(@class: "card"))
            {
                using (Div(@class: "card-header"))
                {
                    using (Div(@class: "card-title")) Write(BatchCreate ? Strings.BatchCreate : Strings.BatchUpdate);
                }

                using (Div(@class: "card-form"))
                {
                    using (Div(@class: "flex items-center gap-4 pb-4"))
                    {
                        I(@class: "fas fa-1 text-neutral-400", style: "font-size: 24px");
                        if (BatchCreate)
                        {
                            using (Div(@class: "font-semibold")) Write(Strings.OpenEmptyTemplateInYourSpreadsheetProgram);
                        }
                        else
                        {
                            using (Div(@class: "font-semibold")) Write(Strings.OpenDataInYourSpreadsheetProgram);
                        }
                        using (Button(id: "export-button", type: "button", @class: "btn", onclick: "javascript:copyToClipboard()")) Write(Strings.Copy_to_clipboard);
                    }

                    using (Div(@class: "flex items-center gap-4 py-4"))
                    {
                        I(@class: "fas fa-2 text-neutral-400", style: "font-size: 24px");
                        if (BatchCreate)
                        {
                            using (Div(@class: "font-semibold")) Write(Strings.FillInDataInYourSpreadsheetProgram);
                        }
                        else
                        {
                            using (Div(@class: "font-semibold")) Write(Strings.UpdateDataInYourSpreadsheetProgram);
                        }
                    }

                    using (Div(@class: "flex items-center gap-4 py-4"))
                    {
                        I(@class: "fas fa-3 text-neutral-400", style: "font-size: 24px");
                        using (Div(@class: "font-semibold")) Write(Strings.CopyDataFromSpreadsheatAndPasteBelow);
                    }

                    using (Div(@class: "pt-4"))
                    {
                        Textarea(id: "Content", style: "width: 100%; height: 200px; font-family: monospace", wrap: "off", @class: "form-control");
                    }
                }
                using (Div(@class: "card-header"))
                {
                    using (Button(@class: "btn btn-primary", onclick: "submitCsv(this)")) Write(Strings.Next);
                }
            }

            using (Textarea2(id: "export-textarea", style: "display: none"))
            {
                var type = ManagerServer.Model.Object.GetGuidByType(typeof(T));

                using (var w = new StringWriter())
                {
                    var csvConfiguration = new CsvHelper.Configuration.CsvConfiguration(System.Threading.Thread.CurrentThread.CurrentCulture)
                    {
                        Delimiter = "\t"
                    };

                    if (BatchCreate)
                    {
                        var headers = GetDictionary(new T()).Select(x => x.Key).ToArray();
                        using (var csv = new CsvWriter(w, csvConfiguration))
                        {
                            foreach (var e in headers) csv.WriteField(e);
                        }
                    }
                    else
                    {                        
                        var rows = ApplicationData.Businesses.Get(Business).OfType<T>().Where(x => keys.Contains(x.Key)).Select(x => new Tuple<Guid, Dictionary<string, string>>(x.Key, GetDictionary(x))).ToArray();
                        var headers = rows.SelectMany(x => x.Item2.Keys).Distinct().ToArray();

                        using (var csv = new CsvWriter(w, csvConfiguration))
                        {
                            foreach (var e in headers) csv.WriteField(e);
                            csv.WriteField("Key");
                            csv.NextRecord();
                            if (rows != null)
                            {
                                foreach (var e in rows)
                                {
                                    foreach (var e2 in headers)
                                    {
                                        if (e.Item2.TryGetValue(e2, out string value)) csv.WriteField(value);
                                        else csv.WriteField(string.Empty);
                                    }
                                    csv.WriteField(e.Item1.ToString());
                                    csv.NextRecord();
                                }
                            }
                        }
                    }
                    Write(w.ToString());
                }
            }

            using (Div(id: "preview", @class: "mt-4")) { }

            using (Script())
            {
                Write(@"function copyToClipboard() {
    writeToClipboard(document.getElementById('export-textarea').value);
    document.getElementById('export-button').setAttribute('disabled','disabled');
    document.getElementById('export-button').innerText = " + Strings.Copied.EncodeJsString() + @";
    setTimeout(function(){ document.getElementById('export-button').innerText = " + Strings.Copy_to_clipboard.EncodeJsString() + @"; document.getElementById('export-button').removeAttribute('disabled'); }, 3000);
            }");
            }

            using (Script())
            {
                Write(@"function submitCsv(e) {

    document.getElementById('preview').innerHTML = '';
    e.setAttribute('disabled', 'disabled');
    e.innerHTML = e.innerHTML + '<i id=""fa-spinner"" class=""fas fa-spinner-third fa-spin mx-2""></i>';

    let httpRequest = new XMLHttpRequest();
    httpRequest.onreadystatechange = function () {
        if (httpRequest.readyState === XMLHttpRequest.DONE) {
            if (httpRequest.status === 200) {
                document.getElementById('preview').innerHTML = httpRequest.responseText;
            }
            document.getElementById('fa-spinner').remove();
            e.removeAttribute('disabled');
        }
    }
	httpRequest.open('POST', window.location);
    var formData = new FormData();
    formData.append('BatchCreateOrUpdateContent', document.getElementById('Content').value);
	httpRequest.send(formData);
}");

            }
        }

        protected override async Task InnerPost()
        {
            if (Request.HasFormContentType)
            {
                SetCulture(Business);

                var form = await Request.ReadFormAsync();

                if (form.ContainsKey("BatchUpdateKey"))
                {
                    await Get();
                    return;
                }
                if (form["BatchCreateOrUpdateContent"].Count > 0)
                {
                    Response.ContentType = "text/html";

                    var content = form["BatchCreateOrUpdateContent"].ToString();

                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        using (var r = new StringReader(content))
                        {
                            var csvConfiguration = new CsvHelper.Configuration.CsvConfiguration(System.Threading.Thread.CurrentThread.CurrentCulture)
                            {
                                Delimiter = "\t",
                                MissingFieldFound = null,
                                BadDataFound = null,
                                IgnoreBlankLines = true
                            };

                            using (var csv = new CsvReader(r, csvConfiguration))
                            {
                                csv.Read();
                                csv.ReadHeader();

                                var headers = csv.HeaderRecord.Where(x => !string.IsNullOrWhiteSpace(x)).Where(x => x != "Key").Distinct().ToArray();
                                var fields = GetMembers(typeof(T)).ToDictionary(x => x.Name);

                                var columns = headers.Select(x => x.Split('.').First()).Distinct().Where(x => fields.ContainsKey(x)).ToArray();

                                using (Div(@class: "card"))
                                {
                                    using (Div(@class: "card-header"))
                                    {
                                        using (Div(@class: "card-title")) Write("Preview");
                                    }

                                    var objects = new Dictionary<Guid, T>();

                                    using (Table(@class: "card-table"))
                                    {
                                        using (THead())
                                        {
                                            using (Tr())
                                            {
                                                using (Th(@class: "w-px")) { }
                                                foreach (var e in columns)
                                                {
                                                    using (Th()) Write(ManagerServer.Globalization.Strings.GetPropertyValue(e));
                                                }
                                            }
                                        }
                                        using (TBody())
                                        {
                                            int index = 1;
                                            while (csv.Read())
                                            {
                                                var csvRow = new Dictionary<string, string>();
                                                foreach (var e in headers)
                                                {
                                                    var cell = csv[e] ?? string.Empty;
                                                    csvRow.Add(e, cell.Trim());
                                                }

                                                if (csvRow.Count == 0) continue;
                                                if (csvRow.All(x => string.IsNullOrWhiteSpace(x.Value))) continue;

                                                var o = new T();

                                                if (csv.TryGetField<Guid>("Key", out Guid key))
                                                {
                                                    var o2 = ApplicationData.Businesses.Get(Business).SingleOrDefault<T>(key);
                                                    if (o2 != null)
                                                    {
                                                        o = ProtoBuf.Serializer.DeepClone<T>(o2);
                                                    }
                                                    o.Key = key;
                                                }
                                                else
                                                {
                                                    o.Key = Guid.CreateVersion7();
                                                }

                                                if (!objects.ContainsKey(o.Key))
                                                {
                                                    WriteColumns(index, columns, o, csvRow);

                                                    objects.Add(o.Key, o);
                                                }

                                                index++;
                                            }
                                        }
                                    }
                                    using (Div(@class: "card-header"))
                                    {
                                        using (PostForm())
                                        {
                                            using (Div(style: "display: flex; align-items: center"))
                                            {
                                                I(@class: "fas fa-fw fa-turn-up fa-rotate-90", style: "font-size: 32px; color: #ccc");
                                                Write("&nbsp;&nbsp;&nbsp;");
                                                using (var ms = new System.IO.MemoryStream())
                                                {
                                                    ProtoBuf.Serializer.Serialize<Dictionary<Guid, T>>(ms, objects);
                                                    InputHidden(name: "BatchCreateOrUpdateObjects", value: Convert.ToBase64String(ms.ToArray()));
                                                }
                                                using (Button(@class: "btn btn-primary", style: "font-weight: bold")) Write(BatchCreate ? Strings.BatchCreate : Strings.BatchUpdate);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    return;
                }

                if (form["BatchCreateOrUpdateObjects"].Count > 0)
                {
                    var objects = form["BatchCreateOrUpdateObjects"].ToString();

                    var list = new List<T>();

                    using (var ms = new System.IO.MemoryStream(Convert.FromBase64String(objects)))
                    {
                        var dict = ProtoBuf.Serializer.Deserialize<Dictionary<Guid, T>>(ms);
                        foreach (var e in dict) e.Value.Key = e.Key;
                        list.AddRange(dict.Values);
                    }

                    var referenceField = typeof(T).GetFieldOrProperty("Reference");
                    var automaticReferenceField = typeof(T).GetFieldOrProperty("AutomaticReference");
                    if (automaticReferenceField != null && automaticReferenceField.GetMemberType() == typeof(bool) && referenceField != null && referenceField.GetMemberType() == typeof(string))
                    {
                        var typeAccessor = FastMember.TypeAccessor.Create(typeof(T));
                        var references = ApplicationData.Businesses.Get(Business).OfType<T>().Select(x => typeAccessor[x, "Reference"] as string).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                        long reference = 1;
                        foreach (var e in references)
                        {
                            if (string.IsNullOrWhiteSpace(e)) continue;
                            var s = string.Join("", e.ToCharArray().Where(x => char.IsDigit(x)));
                            if (string.IsNullOrWhiteSpace(s)) continue;
                            long i = 0;
                            if (long.TryParse(s, out i))
                            {
                                if (i >= reference) reference = i + 1;
                            }
                        }

                        foreach (var e in list)
                        {
                            if (typeAccessor[e, "AutomaticReference"] is bool value && value)
                            {
                                typeof(T).GetFieldOrProperty("Reference").SetMemberValue(e, reference.ToString());
                                typeof(T).GetFieldOrProperty("AutomaticReference").SetMemberValue(e, false);
                                reference++;
                            }
                        }
                    }

                    ApplicationData.Businesses.Process(Business, list.ToArray(), GetUserName());
                    var referrer = new Start() { Business = Business }.ToUrl();
                    if (Referrer != null) referrer = Referrer;
                    Response.Redirect(referrer);
                    return;
                }
            }

            await base.InnerPost();
        }

        private void WriteColumns(int? index, string[] columns, object o, Dictionary<string, string> csvRow)
        {
            using (Tr())
            {
                if (index.HasValue)
                {
                    using (Th(@class: "w-px")) Write(index.ToString());
                }

                var objectAccessor = FastMember.ObjectAccessor.Create(o);
                var fields = GetMembers(o.GetType()).ToDictionary(x => x.Name);

                foreach (var e in columns)
                {
                    var fieldType = fields[e].GetMemberType();

                    using (Td())
                    {
                        if (csvRow.ContainsKey(e) && fieldType == typeof(string))
                        {
                            if (WriteValue(fieldType, objectAccessor[e], csvRow[e], out object parsedValue)) objectAccessor[e] = parsedValue;
                        }
                        else if (csvRow.ContainsKey(e) && fieldType == typeof(DateTime))
                        {
                            if (WriteValue(fieldType, objectAccessor[e], csvRow[e], out object parsedValue)) objectAccessor[e] = parsedValue;
                        }
                        else if (csvRow.ContainsKey(e) && fieldType == typeof(DateTime?))
                        {
                            if (WriteValue(fieldType, objectAccessor[e], csvRow[e], out object parsedValue)) objectAccessor[e] = parsedValue;
                        }
                        else if (csvRow.ContainsKey(e) && fieldType == typeof(decimal))
                        {
                            if (WriteValue(fieldType, objectAccessor[e], csvRow[e], out object parsedValue)) objectAccessor[e] = parsedValue;
                        }
                        else if (csvRow.ContainsKey(e) && fieldType == typeof(decimal?))
                        {
                            if (WriteValue(fieldType, objectAccessor[e], csvRow[e], out object parsedValue)) objectAccessor[e] = parsedValue;
                        }
                        else if (csvRow.ContainsKey(e) && fieldType == typeof(int))
                        {
                            if (WriteValue(fieldType, objectAccessor[e], csvRow[e], out object parsedValue)) objectAccessor[e] = parsedValue;
                        }
                        else if (csvRow.ContainsKey(e) && fieldType == typeof(int?))
                        {
                            if (WriteValue(fieldType, objectAccessor[e], csvRow[e], out object parsedValue)) objectAccessor[e] = parsedValue;
                        }
                        else if (csvRow.ContainsKey(e) && fieldType == typeof(bool))
                        {
                            if (WriteValue(fieldType, objectAccessor[e], csvRow[e], out object parsedValue)) objectAccessor[e] = parsedValue;
                        }
                        else if (csvRow.ContainsKey(e) && fieldType.IsEnum)
                        {
                            if (WriteValue(fieldType, objectAccessor[e], csvRow[e], out object parsedValue)) objectAccessor[e] = parsedValue;
                        }
                        else if (csvRow.ContainsKey(e) && fieldType == typeof(Guid?))
                        {
                            var csvRowValue = csvRow[e];
                            if (!string.IsNullOrWhiteSpace(csvRowValue))
                            {
                                if (Guid.TryParse(csvRowValue, out Guid result))
                                {
                                    // Nothing needed to do here
                                }
                                else
                                {
                                    var autocompleteAttribute = fields[e].GetCustomAttribute<AutocompleteAttribute>();
                                    if (autocompleteAttribute != null)
                                    {
                                        var o2 = ApplicationData.Businesses.Get(Business).SingleOrDefaultByCode(csvRowValue, autocompleteAttribute.Value);
                                        if (o2 != null)
                                        {
                                            csvRowValue = o2.Key.ToString();
                                        }
                                    }
                                }
                            }

                            if (WriteValue(fieldType, objectAccessor[e], csvRowValue, out object parsedValue)) objectAccessor[e] = parsedValue;
                        }
                        else if (fieldType == typeof(Dictionary<Guid, string>))
                        {
                            var customFields = objectAccessor[e] as Dictionary<Guid, string>;
                            if (customFields == null)
                            {
                                customFields = new Dictionary<Guid, string>();
                                objectAccessor[e] = customFields;
                            }

                            using (Table())
                            {
                                var keys = csvRow.Keys.Where(x => x.StartsWith(e + ".")).Where(x => x.Split('.').Length == 2).Select(x => x.Split('.')[1]).Where(x => Guid.TryParse(x, out Guid result)).Select(x => Guid.Parse(x)).ToArray();
                                using (Tr())
                                {
                                    foreach (var e2 in keys)
                                    {
                                        using (Td(style: "font-weight: bold; vertical-align: top"))
                                        {
                                            var customField = ApplicationData.Businesses.Get(Business).SingleOrDefault<ManagerServer.Model.CustomField>(e2);
                                            if (customField != null) Write(customField.Name);
                                            else Write(e2.ToString("N"));
                                        }
                                    }
                                }
                                using (Tr())
                                {
                                    foreach (var e2 in keys)
                                    {
                                        using (Td())
                                        {
                                            customFields.TryGetValue(e2, out string currentValue);
                                            if (WriteValue(typeof(string), currentValue, csvRow[e + "." + e2.ToString("N")], out object parsedValue))
                                            {
                                                SetValue<string>(customFields, e2, (string)parsedValue);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (fieldType == typeof(ManagerServer.Model.CustomFields))
                        {
                            var customFields2 = objectAccessor[e] as ManagerServer.Model.CustomFields;
                            if (customFields2 == null)
                            {
                                customFields2 = new ManagerServer.Model.CustomFields();
                                objectAccessor[e] = customFields2;
                            }

                            using (Table())
                            {
                                var keys = csvRow.Keys.Where(x => x.StartsWith(e + ".")).Where(x => x.Split('.').Length == 3).ToArray();
                                using (Tr())
                                {
                                    foreach (var e2 in keys)
                                    {
                                        if (Guid.TryParse(e2.Split('.')[2], out Guid key))
                                        {
                                            using (Td(style: "font-weight: bold; vertical-align: top"))
                                            {
                                                var customField = ApplicationData.Businesses.Get(Business).SingleOrDefault<ManagerServer.Model.NamedObject>(key);
                                                if (customField != null) Write(customField.GetName());
                                                else Write(key.ToString("N"));
                                            }
                                        }
                                    }
                                }
                                using (Tr())
                                {
                                    foreach (var e2 in keys)
                                    {
                                        if (Guid.TryParse(e2.Split('.')[2], out Guid key))
                                        {
                                            var currentValue = customFields2.GetValue(key);

                                            using (Td())
                                            {
                                                var customFieldType = e2.Split('.')[1];
                                                if (customFieldType == nameof(ManagerServer.Model.CustomFields.Booleans))
                                                {
                                                    if (WriteValue(typeof(bool), currentValue, csvRow[e2], out object parsedValue))
                                                    {
                                                        if (customFields2.Booleans == null) customFields2.Booleans = new Dictionary<Guid, bool>();
                                                        SetValue(customFields2.Booleans, key, (bool)parsedValue);
                                                    }
                                                }
                                                if (customFieldType == nameof(ManagerServer.Model.CustomFields.StringArrays))
                                                {
                                                    if (WriteValue(typeof(string[]), currentValue, csvRow[e2], out object parsedValue))
                                                    {
                                                        if (customFields2.StringArrays == null) customFields2.StringArrays = new Dictionary<Guid, string[]>();
                                                        var parsedValueAsArray = ((string)parsedValue).Split(',').ToArray();
                                                        SetValue(customFields2.StringArrays, key, parsedValueAsArray);
                                                    }
                                                }
                                                if (customFieldType == nameof(ManagerServer.Model.CustomFields.Strings))
                                                {
                                                    if (WriteValue(typeof(string), currentValue, csvRow[e2], out object parsedValue))
                                                    {
                                                        if (customFields2.Strings == null) customFields2.Strings = new Dictionary<Guid, string>();
                                                        SetValue(customFields2.Strings, key, (string)parsedValue);
                                                    }
                                                }
                                                if (customFieldType == nameof(ManagerServer.Model.CustomFields.Dates))
                                                {
                                                    if (WriteValue(typeof(DateTime?), currentValue, csvRow[e2], out object parsedValue))
                                                    {
                                                        if (customFields2.Dates == null) customFields2.Dates = new Dictionary<Guid, DateTime?>();
                                                        SetValue(customFields2.Dates, key, (DateTime?)parsedValue);
                                                    }
                                                }
                                                if (customFieldType == nameof(ManagerServer.Model.CustomFields.Decimals))
                                                {
                                                    if (WriteValue(typeof(decimal?), currentValue, csvRow[e2], out object parsedValue))
                                                    {
                                                        if (customFields2.Decimals == null) customFields2.Decimals = new Dictionary<Guid, decimal?>();
                                                        SetValue(customFields2.Decimals, key, (decimal?)parsedValue);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (fieldType.IsArray)
                        {
                            var innerFields = GetMembers(fieldType.GetElementType()).ToDictionary(x => x.Name);
                            var innerColumns = csvRow.Keys.Where(x => x.Split('.')[0] == e).Where(x => x.Split('.').Length >= 3).Select(x => x.Split('.')[2]).Distinct().Where(x => innerFields.ContainsKey(x)).ToArray();

                            using (Table(@class: "card-table"))
                            {
                                using (Tr())
                                {
                                    foreach (var e2 in innerColumns)
                                    {
                                        using (Td(style: "font-weight: bold; vertical-align: top")) Write(ManagerServer.Globalization.Strings.GetPropertyValue(e2));
                                    }
                                }

                                var requiredLength = 1;
                                var lineNumbers = csvRow.Keys.Select(x => x.Split('.')).Where(x => x[0] == e).Where(x => x.Length >= 3).Select(x => x[1]).Where(x => !string.IsNullOrWhiteSpace(x)).Where(x => x.All(y => char.IsDigit(y))).Select(x => int.Parse(x));
                                if (lineNumbers.Any()) requiredLength = lineNumbers.Max();
                                var array = objectAccessor[e] as Array;
                                if (array == null)
                                {
                                    array = Array.CreateInstance(fieldType.GetElementType(), requiredLength);
                                    objectAccessor[e] = array;
                                }
                                else if (array.Length < requiredLength)
                                {
                                    var array2 = Array.CreateInstance(fieldType.GetElementType(), requiredLength);
                                    array.CopyTo(array2, 0);
                                    objectAccessor[e] = array2;
                                    array = array2;
                                }
                                for (int i = 0; i < array.Length; i++)
                                {
                                    var prefix = $"{e}.{i + 1}.";
                                    var innerCsvRow = csvRow.Where(x => x.Key.StartsWith(prefix)).ToDictionary(x => x.Key.Substring(prefix.Length), x => x.Value);

                                    var arrayElement = array.GetValue(i);

                                    if (arrayElement == null && innerCsvRow.All(x => string.IsNullOrWhiteSpace(x.Value))) continue;

                                    if (arrayElement == null)
                                    {
                                        arrayElement = Activator.CreateInstance(fieldType.GetElementType());
                                        array.SetValue(arrayElement, i);
                                    }

                                    WriteColumns(null, innerColumns, arrayElement, innerCsvRow);
                                }

                                // Remove null elements from array
                                var list = new List<object>();
                                for (int i = 0; i < array.Length; i++)
                                {
                                    var arrayElement = array.GetValue(i);
                                    if (arrayElement != null) list.Add(arrayElement);
                                }
                                var array3 = Array.CreateInstance(fieldType.GetElementType(), list.Count);
                                list.ToArray().CopyTo(array3, 0);
                                objectAccessor[e] = array3;
                            }
                        }
                        else
                        {
                            Write(ToString(objectAccessor[e]));
                        }
                    }
                }
            }
        }

        private void SetValue<T2>(Dictionary<Guid, T2> dict, Guid key, T2 value)
        {
            if (object.Equals(value, default(T2))) dict.Remove(key);
            else if (object.Equals(value, string.Empty)) dict.Remove(key);
            else if (object.Equals(value, Array.Empty<string>())) dict.Remove(key);
            else dict[key] = value;
        }

        private bool WriteValue(Type fieldType, object currentValue, string csvValue, out object parsedValue)
        {
            parsedValue = Parse(fieldType, csvValue);

            if (currentValue is string[] stringArray) currentValue = string.Join(',', stringArray);

            var equals = object.Equals(currentValue, parsedValue);
            if (!equals)
            {
                if (currentValue is Guid && parsedValue is Guid)
                {
                    var code1 = (ApplicationData.Businesses.Get(Business).SingleOrDefault((Guid)currentValue) as ICode)?.Code;
                    var code2 = (ApplicationData.Businesses.Get(Business).SingleOrDefault((Guid)parsedValue) as ICode)?.Code;
                    if (!string.IsNullOrWhiteSpace(code1) && !string.IsNullOrWhiteSpace(code2))
                    {
                        if (object.Equals(code1, code2))
                        {
                            equals = true;
                        }
                    }
                }
            }

            if (!equals)
            {
                using (Span(@class: "line-through text-rose-600")) WriteDisplayString(currentValue);
                Write("&nbsp;");
                using (Span(@class: "text-green-600")) WriteDisplayString(parsedValue);
                return true;
            }
            else
            {
                WriteDisplayString(currentValue);
                return false;
            }
        }

        private Dictionary<string, string> GetDictionary(object o)
        {
            var output = new Dictionary<string, string>();
            foreach (var field in GetMembers(o.GetType()))
            {
                var value = field.GetMemberValue(o);
                if (value is Array array)
                {
                    for (int i = 0; i < array.Length; i++)
                    {
                        var arrayElement = array.GetValue(i);
                        var innerDict = GetDictionary(arrayElement);
                        foreach (var e in innerDict) output.Add($"{field.Name}.{(i + 1)}.{e.Key}", e.Value);
                    }
                }
                else if (value is Dictionary<Guid, string> strings)
                {
                    foreach (var e in strings) output.Add($"{field.Name}.{e.Key.ToString("N")}", e.Value);
                }
                else if (value is Dictionary<Guid, bool> booleans)
                {
                    foreach (var e in booleans) output.Add($"{field.Name}.{e.Key.ToString("N")}", ToString(e.Value));
                }
                else if (value is Dictionary<Guid, decimal?> decimals)
                {
                    foreach (var e in decimals) output.Add($"{field.Name}.{e.Key.ToString("N")}", ToString(e.Value));
                }
                else if (value is Dictionary<Guid, DateTime?> dates)
                {
                    foreach (var e in dates) output.Add($"{field.Name}.{e.Key.ToString("N")}", ToString(e.Value));
                }
                else if (value is Dictionary<Guid, string[]> stringArrays)
                {
                    foreach (var e in stringArrays) output.Add($"{field.Name}.{e.Key.ToString("N")}", ToString(e.Value));
                }
                else if (value is ManagerServer.Model.CustomFields customFields)
                {
                    var innerDict = GetDictionary(customFields);
                    foreach (var e in innerDict) output.Add($"{field.Name}.{e.Key}", e.Value);
                }
                else if (value is Guid guid)
                {
                    var o2 = ApplicationData.Businesses.Get(Business).SingleOrDefault(guid) as ICode;
                    if (o2 != null && !string.IsNullOrWhiteSpace(o2.Code))
                    {
                        output.Add(field.Name, o2.Code);
                    }
                    else
                    {
                        output.Add(field.Name, guid.ToString());
                    }
                }
                else if (value != null)
                {
                    output.Add(field.Name, ToString(value));
                }
                else if (field.GetMemberType() == typeof(Guid?))
                {
                    output.Add(field.Name, string.Empty);
                }
                else if (field.GetMemberType() == typeof(string))
                {
                    output.Add(field.Name, string.Empty);
                }
                else if (field.GetMemberType() == typeof(decimal?))
                {
                    output.Add(field.Name, string.Empty);
                }
                else if (field.GetMemberType() == typeof(int?))
                {
                    output.Add(field.Name, string.Empty);
                }
                else if (field.GetMemberType() == typeof(DateTime?))
                {
                    output.Add(field.Name, string.Empty);
                }
                else if (field.GetMemberType().IsArray)
                {
                    var innerDict = GetDictionary(Activator.CreateInstance(field.GetMemberType().GetElementType()));
                    foreach (var e in innerDict) output.Add($"{field.Name}.1.{e.Key}", e.Value);
                }
            }
            return output;
        }

        private string ToString(object value)
        {
            if (value is bool) return value.ToString().ToUpperInvariant();
            else if (value is DateTime dateTime)
            {
                if (dateTime == default(DateTime)) return string.Empty;
                return dateTime.ToLocalShortDisplayString();
            }
            else if (value is decimal d)
            {
                if (d == default(decimal)) return string.Empty;
                return d.ToString();
            }
            else if (value is string[] stringArray) return string.Join(",", stringArray);
            else if (value != null) return value.ToString().Trim();
            else return string.Empty;
        }

        private object Parse(Type fieldType, string text)
        {
            if (fieldType == typeof(string))
            {
                return text;
            }
            else if (fieldType == typeof(string[]))
            {
                return text;
            }
            else if (fieldType.IsEnum)
            {
                if (Enum.TryParse(fieldType, text, out object result)) return result;
                return Enum.GetValues(fieldType).GetValue(0);
            }
            else if (fieldType == typeof(Guid?))
            {
                if (Guid.TryParse(text, out Guid result)) return result;
                else return default(Guid?);
            }
            else if (fieldType == typeof(DateTime))
            {
                if (DateTime.TryParse(text, out DateTime result)) return result;
                else return default(DateTime);
            }
            else if (fieldType == typeof(DateTime?))
            {
                if (DateTime.TryParse(text, out DateTime result)) return result;
                else return default(DateTime?);
            }
            else if (fieldType == typeof(bool))
            {
                if (bool.TryParse(text, out bool result)) return result;
                else return default(bool);
            }
            else if (fieldType == typeof(decimal))
            {
                if (decimal.TryParse(text, out decimal result)) return result;
                else return default(decimal);
            }
            else if (fieldType == typeof(decimal?))
            {
                if (decimal.TryParse(text, out decimal result)) return result;
                else return default(decimal?);
            }
            else if (fieldType == typeof(int))
            {
                if (int.TryParse(text, out int result)) return result;
                else return default(int);
            }
            else if (fieldType == typeof(int?))
            {
                if (int.TryParse(text, out int result)) return result;
                else return default(int?);
            }
            return null;
        }

        private void WriteDisplayString(object value)
        {
            if (value is Guid guid)
            {
                var code = (ApplicationData.Businesses.Get(Business).SingleOrDefault(guid) as ICode)?.Code;
                if (!string.IsNullOrWhiteSpace(code))
                {
                    Write(code);
                    return;
                }
            }
            if (value is decimal d && d != default(decimal))
            {
                Write(d.ToNumberString());
                return;
            }
            if (value is string[])
            {
                Write(string.Join(", ", (string[])value));
                return;
            }

            Write(ToString(value));
        }

        protected override void OnFooterEndSection(Context context)
        {
            var batchOperationItems = GetBatchOperations(context);

            var namespaceEntry = this.GetType().GetCustomAttribute<NamespaceEntryAttribute>();
            if (namespaceEntry != null)
            {
                var userPermissions = GetCurrentUserPermissions(Business);
                if (userPermissions.CanCreate(this.GetType().Namespace))
                {
                    var batchOperations = (NakedObjectsWithBatchCreateAndBatchUpdate<T>)this.MemberwiseClone();
                    batchOperations.BatchCreate = true;

                    batchOperationItems.Items.Add(new Tuple<string, BusinessTemplate>(Strings.BatchCreate, batchOperations));
                }

                var rows = context.Get<Array>();
                if (rows.Length > 0)
                {
                    if (userPermissions.CanUpdate(this.GetType().Namespace))
                    {
                        var batchOperations = (NakedObjectsWithBatchCreateAndBatchUpdate<T>)this.MemberwiseClone();
                        batchOperations.BatchUpdate = true;
                        batchOperationItems.Items.Add(new Tuple<string, BusinessTemplate>(Strings.BatchUpdate, batchOperations));
                    }
                }
            }

            base.OnFooterEndSection(context);
        }

        private static ConcurrentDictionary<Type, Lazy<MemberInfo[]>> memberInfoCache = new();
        private static MemberInfo[] GetMembers(Type t)
        {
            return memberInfoCache.GetOrAdd(t, new Lazy<MemberInfo[]>(() =>
            {
                return t.GetFieldsAndProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(x => x.GetCustomAttribute<ProtoBuf.ProtoMemberAttribute>() != null)
                    .Where(x => x.Name != "Obsolete_")
                    .ToArray();
            })).Value;
        }
    }
}