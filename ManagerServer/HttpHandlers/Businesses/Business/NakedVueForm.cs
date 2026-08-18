using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.IO;
using ManagerServer.Model.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;
using MemberInfo = System.Reflection.MemberInfo;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    internal abstract class NakedVueForm<T> : VueForm<T> where T : ManagerServer.Model.Object, new()
    {
        protected override sealed void InnerGet3()
        {
            var queue = new Queue<MemberInfo>();
            foreach (var e in typeof(T).GetFieldsAndProperties(BindingFlags.Instance | BindingFlags.Public).Where(x => x.DeclaringType == typeof(T)))
            {
                if (!e.CanWrite()) continue;
                queue.Enqueue(e);
            }

            ProcessQueueLoop(queue);
        }

        private void ProcessQueueLoop(Queue<MemberInfo> queue)
        {
            while (queue.Any())
            {
                var nextField = queue.Peek();
                if (!IsVisible(nextField))
                {
                    queue.Dequeue();
                    continue;
                }

                var ifExpression = $"getIf{nextField.Name}()";
                if (!nextField.GetCustomAttributes<IfAttribute>().Any()) ifExpression = null;

                if (nextField.GetCustomAttribute<FieldsetAttribute>() != null)
                {
                    using (Fieldset())
                    {
                        using (Legend()) Write(Strings.GetPropertyValue(nextField.GetCustomAttribute<FieldsetAttribute>().Legend));
                        using (Div(v_if: ifExpression)) ProcessQueue(queue);
                        ProcessQueueLoop(queue);
                    }
                }
                else
                {
                    using (Div(v_if: ifExpression)) ProcessQueue(queue);
                }
            }
        }

        private void ProcessQueue(Queue<MemberInfo> queue)
        {
            if (queue.Peek().GetCustomAttribute<NoWrapAttribute>() != null)
            {
                using (Div(@class: "flex gap-2"))
                {
                    using (Div())
                    {
                        InnerProcessQueue(queue);
                    }

                    while (queue.Any())
                    {
                        var nextField = queue.Peek();
                        var breakAfterCell = (nextField.GetCustomAttribute<NoWrapAttribute>() == null);

                        if (IsVisible(nextField))
                        {
                            var ifExpression = $"getIf{nextField.Name}()";
                            if (!nextField.GetCustomAttributes<IfAttribute>().Any()) ifExpression = null;

                            if (nextField.GetCustomAttribute<TypeaheadAttribute>() != null)
                            {
                                // Typeahead component cannot be properly destroyed until upgrade to Vue 3.0 which has 'beforeUnmount' method.
                                using (Div(v_show: ifExpression)) InnerProcessQueue(queue);
                            }
                            else
                            {
                                using (Div(v_if: ifExpression)) InnerProcessQueue(queue);
                            }
                        }
                        else
                        {
                            queue.Dequeue();
                        }
                        if (breakAfterCell) break;
                    }
                }                
            }
            else
            {
                using (Div(@class: "flex")) InnerProcessQueue(queue);
            }
        }

        private void InnerProcessQueue(Queue<MemberInfo> queue)
        {
            var field = queue.Dequeue();

            var label = ManagerServer.Globalization.Strings.GetPropertyValue(field.Name);
            if (field.GetCustomAttribute<LabelAttribute>() != null) label = field.GetCustomAttribute<LabelAttribute>().ToString();
            if (field.GetCustomAttribute<EmptyLabelAttribute>() != null) label = "&nbsp;";
            var visibleLabel = (field.GetCustomAttribute<NoLabelAttribute>() == null);

            if (field.DeclaringType.IsSubclassOf(typeof(ManagerServer.Model.Transaction)) && field.Name == "Reference" && typeof(T).GetFieldsAndProperties().Any(x => x.Name == "AutomaticReference"))
            {
                using (Div(@class: "form-group"))
                {
                    using (Label()) Write(label);
                    using (Div(@class: "input-group"))
                    {
                        using (Span(@class: "input-group-text")) InputCheckbox(v_model: "AutomaticReference", @class: "form-check-input");
                        InputText(v_model: field.Name, @class: "form-control", style: "width: 120px; text-align: center", v_if: "!AutomaticReference", placeholder: Strings.Optional);
                        InputText(@class: "form-control", @readonly: true, style: "width: 120px; text-align: center", v_if: "AutomaticReference", placeholder: Strings.Automatic);
                    }
                }
            }
            else if (field.GetMemberType() == typeof(string))
            {
                if (field.GetCustomAttribute<CodeAttribute>() != null)
                {
                    using (Div(@class: "form-group w-full"))
                    {
                        LiquidEditor(field);
                    }
                }
                else if (field.GetCustomAttribute<HtmlAttribute>() != null)
                {
                    using (Div(@class: "form-group w-full"))
                    {
                        HtmlEditor(field);
                    }
                }
                else if (field.GetCustomAttribute<JavascriptAttribute>() != null)
                {
                    using (Div(@class: "form-group w-full"))
                    {
                        JavascriptEditor(field);
                    }
                }
                else if (field.GetCustomAttribute<TimeFormatAttribute>() != null)
                {
                    using (Div(@class: "form-group"))
                    {
                        using (Label()) Write(label);
                        VSelectTimeFormat(field);
                    }
                }
                else if (field.GetCustomAttribute<DateFormatAttribute>() != null)
                {
                    using (Div(@class: "form-group"))
                    {
                        using (Label()) Write(label);
                        VSelectDateFormat(field);
                    }
                }
                else if (field.GetCustomAttribute<NumberFormatAttribute>() != null)
                {
                    using (Div(@class: "form-group"))
                    {
                        using (Label()) Write(label);
                        VSelectNumberFormat(field);
                    }
                }
                else
                {
                    var width = "300px";
                    if (field.GetCustomAttribute<ShortAttribute>() != null) width = "120px";
                    if (field.GetCustomAttribute<LongAttribute>() != null) width = "600px";

                    string v_if = null;
                    if (field.GetCustomAttribute<IfNotEmptyAttribute>() != null) v_if = $"{field.Name} !== null && {field.Name}.length > 0";

                    using (Div(@class: "form-group", style: $"width: {width}", v_if: v_if))
                    {
                        if (visibleLabel) using (Label()) Write(label);

                        if (field.Name == "Password")
                        {
                            VInputPassword(field);
                        }
                        else if (field.GetCustomAttribute<SecretAttribute>() != null)
                        {
                            VInputPassword(field);
                        }
                        else if (field.GetCustomAttribute<TextareaAttribute>() != null)
                        {
                            VTextarea(null, field);
                        }
                        else
                        {
                            VInputTextWithPrependAppend(null, field);
                        }
                    }
                }
            }
            else if (field.GetMemberType() == typeof(KeyValuePair<string, string>))
            {
                using (Div(@class: "form-group"))
                {
                    if (visibleLabel) using (Label()) Write(label);
                    VInputText(null, field);
                }
            }
            else if (field.GetMemberType() == typeof(int?))
            {
                using (Div(@class: "form-group"))
                {
                    if (visibleLabel) using (Label()) Write(label);
                    VInputNumber(field);
                }
            }
            else if (field.GetMemberType() == typeof(decimal))
            {
                using (Div(@class: "form-group"))
                {
                    if (visibleLabel) using (Label()) Write(label);
                    VInputDecimal(null, field, true);
                }
            }
            else if (field.GetMemberType() == typeof(bool))
            {
                if (field.GetCustomAttribute<IconAttribute>() != null)
                {
                    using (Div(@class: "form-group"))
                    {
                        using (Label()) Write("&nbsp;");
                        VInputBoolean(null, field);
                    }
                }
                else if (field.GetCustomAttribute<TabSwitchAttribute>() != null)
                {
                    using (Div())
                    {
                        using (Div(@class: "card mb-2"))
                        {
                            using (Label(@class: "card-header cursor-pointer flex items-center gap-2 mb-0"))
                            {
                                InputCheckbox(value: "true", @class: "form-check-input", v_model: field.Name, v_on_change: $"{field.Name} ? document.getElementById('tab{field.Name}').classList.remove('hidden') : document.getElementById('tab{field.Name}').classList.add('hidden')");
                                using (Span()) Write(ManagerServer.Globalization.Strings.GetPropertyValue(field.Name));
                                if (field.GetCustomAttribute<TabSwitchAttribute>().Popular)
                                {
                                    using (Span(@class: "badge text-bg-primary mx-2")) Write(Strings.Popular);
                                }
                            }
                        }
                        if (queue.TryPeek(out MemberInfo nextField) && nextField.GetCustomAttribute<IfTrueAttribute>()?.Path[0] == field.Name)
                        {
                            using (Div(@class: "ms-12"))
                            {
                                while (queue.TryPeek(out MemberInfo nextField2))
                                {
                                    if (nextField2.GetCustomAttribute<IfTrueAttribute>()?.Path[0] != field.Name) break;

                                    ProcessQueue(queue);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (field.Name == "Inactive" && !Key.HasValue) return; // Do not show inactive checkboxes if new object
                    if (field.Name == "CustomTheme" && !ApplicationData.Businesses.Get(Business).OfType<CustomTheme>().Any()) return; // Do not show if not themes exist
                    if (field.GetCustomAttribute<IfExistsAttribute>() != null && !Key.HasValue) return; // Do not show inactive checkboxes if new object
                    if (field.DeclaringType.IsSubclassOf(typeof(ManagerServer.Model.Transaction)) && field.Name == "AutomaticReference" && typeof(T).GetFieldsAndProperties().Any(x => x.Name == "Reference")) return; // Already used

                    var id = Guid.CreateVersion7().ToString();

                    using (Div(@class: "flex items-start gap-2 my-1"))
                    {
                        InputCheckbox(id: id, @class: "form-check-input", value: "true", v_model: field.Name);
                        using (Div(@class: "w-full"))
                        {
                            using (Label(@for: id))
                            {
                                if (field.GetCustomAttribute<LabelAttribute>() != null) Write(field.GetCustomAttribute<LabelAttribute>().ToString());
                                else using (Div()) Write(ManagerServer.Globalization.Strings.GetPropertyValue(field.Name));
                            }
                            if (queue.TryPeek(out MemberInfo nextField) && nextField.GetCustomAttribute<IfTrueAttribute>()?.Path[0] == field.Name)
                            {
                                using (Div(v_if: field.Name + " == true"))
                                {
                                    while (queue.TryPeek(out MemberInfo nextField2))
                                    {
                                        if (nextField2.GetCustomAttribute<IfTrueAttribute>()?.Path[0] != field.Name) break;

                                        ProcessQueue(queue);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (field.GetMemberType() == typeof(DateTime) || field.GetMemberType() == typeof(DateTime?))
            {
                using (Div(@class: "form-group"))
                {
                    if (visibleLabel) using (Label()) Write(label);
                    VInputDate(null, field);
                }
            }
            else if (field.GetMemberType().IsEnum)
            {
                using (Div(@class: "form-group"))
                {
                    if (visibleLabel) using (Label()) Write(label);
                    SelectWithPrependAppend(null, field, false);
                }
            }
            else if (field.GetMemberType() == typeof(Guid?) || field.GetMemberType() == typeof(Guid[]))
            {
                if (field.GetCustomAttribute<SelectAttribute>() != null)
                {
                    var selectAttribute = field.GetCustomAttribute<SelectAttribute>();

                    using (Div(@class: "form-group"))
                    {
                        if (visibleLabel) using (Label()) Write(label);
                        var options = new List<Tuple<Guid, string>>();

                        foreach (var e in typeof(ManagerServer.Model.Object).Assembly.GetTypes().Where(x => selectAttribute.Type.IsAssignableFrom(x) && x.IsClass && !x.IsAbstract))
                        {
                            var guidAttribute = e.GetCustomAttribute<GuidAttribute>();
                            options.Add(new Tuple<Guid, string>(guidAttribute.Value, ManagerServer.Globalization.Strings.GetPropertyValue(e.Name)));
                        }

                        VueSelect(field.Name, options.ToArray(), true, true, false);
                    }
                }
                else
                {
                    using (Div(@class: "form-group"))
                    {
                        if (visibleLabel) using (Label()) Write(label);
                        AutocompleteSelect(null, field);
                    }
                }
            }
            else if (field.GetMemberType() == typeof(ManagerServer.Model.MemberInfo))
            {
                var width = "300px";
                if (field.GetCustomAttribute<ShortAttribute>() != null) width = "150px";

                using (Div(@class: "form-group", style: $"width: {width}"))
                {
                    if (visibleLabel) using (Label()) Write(label);
                    ReflectionAutocompleteSelect(null, field);
                }
            }
            else if (field.GetMemberType() == typeof(Dictionary<Guid, string>) && field.Name == "CustomFields")
            {
                CustomFields(field.Name);
            }
            else if (field.GetMemberType() == typeof(ManagerServer.Model.CustomFields))
            {
                CustomFields3(field.Name);
            }
            else if (field.GetMemberType() == typeof(object))
            {
                using (Div(@class: "form-group"))
                {
                    string v_if = null;
                    if (field.GetCustomAttribute<IfExpressionNotZero>() != null) v_if = $"get{field.Name}() != 0";
                    using (Div(v_if: v_if))
                    {
                        if (visibleLabel) using (Label()) Write(label);
                        Expression(field, $"get{field.Name}()");
                    }
                }
            }
            else if (field.GetMemberType().IsArray)
            {
                VArray(field);
            }
        }

        private void VInputTextWithPrependAppend(string prefix, MemberInfo field)
        {
            var prepend = field.GetCustomAttribute<PrependAttribute>();
            var append = field.GetCustomAttribute<AppendAttribute>();
            var appendValue = field.GetCustomAttribute<AppendValueAttribute>();
            if (prepend != null || append != null || appendValue != null)
            {
                using (Div(@class: "input-group"))
                {
                    if (prepend != null) using (Span(@class: "input-group-text input-sm")) Write(prepend.ToString());
                    VInputText(prefix, field);
                    if (appendValue != null) using (Span(@class: "input-group-text input-sm")) Write("{{ " + appendValue.GetExpression() + " }}");
                    if (append != null) using (Span(@class: "input-group-text input-sm")) Write(append.ToString());
                }
            }
            else
            {
                VInputText(prefix, field);
            }
        }

        private void VInputPassword(MemberInfo field)
        {
            var prepend = field.GetCustomAttribute<PrependAttribute>();
            using (Div(@class: "input-group"))
            {
                if (prepend != null) using (Span(@class: "input-group-text input-sm")) Write(prepend.ToString());
                InputPassword(id: "password", @class: "form-control", style: "width: 300px", v_model: field.Name, placeholder: "********");
                using (Span(@class: "input-group-btn", v_if: $"({field.Name} || '').length > 0"))
                {
                    using (Button(id: "password-btn", @class: "btn btn-sm", style: "border-left: none", onclick: "javascript:togglePassword();"))
                    {
                        I(@class: "fas fa-eye");
                        //Write(Strings.ShowPassword);
                    }
                    using (Script()) Write(@"function togglePassword() { var x = document.getElementById('password'); var btn = document.getElementById('password-btn'); if (x.type === 'password') { x.type = 'text'; btn.classList.add('active'); } else { x.type = 'password'; btn.classList.remove('active'); } }");
                }
            }
        }

        private void VInputBoolean(string prefix, MemberInfo field)
        {
            using (Label(@class: "form-control cursor-pointer mb-0"))
            {
                I(@class: "opacity-50 fa " + field.GetCustomAttribute<IconAttribute>().Value);
                InputCheckbox(@class: "form-check-input hidden", value: "true", v_model: prefix + field.Name);
            }            
        }

        private void VInputText(string prefix, MemberInfo field)
        {
            var placeholder = field.GetCustomAttribute<PlaceholderAttribute>()?.ToString();
            var minWidth = 300;
            if (field.GetCustomAttribute<ShortAttribute>() != null) minWidth = 120;
            if (field.GetCustomAttribute<LongAttribute>() != null) minWidth = 600;
            if (field.GetCustomAttribute<TypeaheadAttribute>() != null)
            {
                InputText(name: "query", v_model: prefix + field.Name, @class: "form-control", style: "min-width: " + minWidth + "px", placeholder: placeholder, hxGet: new Typeahead() { Business = Business, Field = field.Name, Type = ManagerServer.Model.Object.GetGuidByType(field.DeclaringType) }.ToUrl(), hxTrigger: "keyup changed delay:300ms", hxTarget: $"#{prefix}{field.Name}", hxInclude: "this", list: $"{prefix}{field.Name}");
                Write($@"<datalist id=""{prefix}{field.Name}""></datalist>");
            }
            else
            {
                InputText(v_model: prefix + field.Name, @class: "form-control", style: "min-width: " + minWidth + "px", placeholder: placeholder);
            }
        }

        private void VInputNumber(MemberInfo field)
        {
            var placeholder = field.GetCustomAttribute<PlaceholderAttribute>()?.ToString();
            var control = $@"<input v-model.number=""{field.Name}"" class=""form-control"" type =""number"" style=""width: 80px"" placeholder=""{placeholder}"">";

            var prepend = field.GetCustomAttribute<PrependAttribute>();
            var append = field.GetCustomAttribute<AppendAttribute>();
            if (prepend != null || append != null)
            {
                using (Div(@class: "input-group"))
                {
                    if (prepend != null) using (Span(@class: "input-group-text input-sm")) Write(prepend.ToString());
                    Write(control);
                    if (append != null) using (Span(@class: "input-group-text input-sm")) Write(append.ToString());
                }
            }
            else
            {
                Write(control);
            }
        }

        private void Expression(MemberInfo field, string expression)
        {
            var prepend = field.GetCustomAttribute<PrependAttribute>();
            var append = field.GetCustomAttribute<AppendAttribute>();
            var appendBaseCurrency = field.GetCustomAttribute<AppendBaseCurrencyAttribute>();
            if (prepend != null || append != null || appendBaseCurrency != null)
            {
                using (Div(@class: "input-group"))
                {
                    if (prepend != null) using (Span(@class: "input-group-text input-sm")) Write(prepend.ToString());
                    using (Span(@class: "form-control font-semibold text-right", style: "min-width: 60px")) Write($"{{{{ {expression} }}}}");
                    if (append != null) using (Span(@class: "input-group-text input-sm")) Write(append.ToString());
                    if (appendBaseCurrency != null) using (Span(@class: "input-group-text input-sm")) Write("{{ " + appendBaseCurrency.GetExpression() + " }}");
                }
            }
            else
            {
                using (Span(@class: "form-control font-semibold text-right", style: "min-width: 60px")) Write($"{{{{ {expression} }}}}");
            }
        }

        private void VInputDecimal(string prefix, MemberInfo field, bool width)
        {
            /*
            var minWidth = "100px";
            if (field.GetCustomAttribute<ShortAttribute>() != null) minWidth = "60px";
            */

            var placeholder = "0";
            if (field.GetCustomAttribute<NoPlaceholderAttribute>() != null) placeholder = string.Empty;
            if (field.GetCustomAttribute<PlaceholderAttribute>() != null) placeholder = field.GetCustomAttribute<PlaceholderAttribute>().ToString();
            var control = $@"<input-decimal v-model=""{prefix}{field.Name}"" placeholder=""{placeholder}"" nullable=""{(field.GetMemberType() == typeof(decimal?)).ToString().ToLowerInvariant()}"" group-separator=""{System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberGroupSeparator}"" decimal-separator=""{System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator}""></input-decimal>";

            var prepend = field.GetCustomAttribute<PrependAttribute>();
            var append = field.GetCustomAttribute<AppendAttribute>();
            var appendCurrency = field.GetCustomAttribute<AppendCurrencyAttribute>();
            var appendCurrency2 = field.GetCustomAttribute<IfDifferentCurrencyAttribute>();
            var appendCurrency3 = field.GetCustomAttribute<AppendBaseCurrencyAttribute>();
            var appendValue = field.GetCustomAttribute<AppendValueAttribute>();
            var appendWebService = field.GetCustomAttribute<WebServiceAttribute>();
            if (prepend != null || append != null || appendCurrency != null || appendCurrency2 != null || appendCurrency3 != null || appendValue != null || appendWebService != null)
            {
                using (Div(@class: "input-group grow"))
                {
                    if (prepend != null) using (Span(@class: "input-group-text input-sm")) Write(prepend.ToString());
                    Write(control);
                    if (appendCurrency != null) using (Span(@class: "input-group-text input-sm")) Write("{{ " + appendCurrency.GetExpression() + " }}");
                    if (appendCurrency2 != null) using (Span(@class: "input-group-text input-sm")) Write("{{ getForeignCurrencyOrBaseCurrencyCode(getAccountCurrency(lineItem)) }}");
                    if (appendCurrency3 != null) using (Span(@class: "input-group-text input-sm")) Write("{{ " + appendCurrency3.GetExpression() + " }}");
                    if (appendValue != null) using (Span(@class: "input-group-text input-sm")) Write("{{ " + appendValue.GetExpression() + " }}");
                    if (append != null) using (Span(@class: "input-group-text input-sm")) Write(append.ToString());
                    if (appendWebService != null)
                    {
                        if (appendWebService.Type != null)
                        {
                            var webServiceKey = ManagerServer.Model.Object.GetGuidByType(appendWebService.Type);
                            var webService = ApplicationData.Businesses.Get(Business).Single(webServiceKey) as IWebService;
                            if (webService != null)
                            {
                                if (Uri.TryCreate(webService.GetUrl(), UriKind.Absolute, out Uri result))
                                {
                                    using (Button(style: "width: auto", @class: "input-group-text input-sm cursor-pointer group", onclick: $"postContentAndReturnDecimal(this, '" + result.ToString() + $"', value => app.{prefix}{field.Name} = value)"))
                                    {
                                        I(@class: "fa fa-rotate-right opacity-50");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Write(control);
            }
        }

        private void VArray(MemberInfo field)
        {
            var elementType = field.GetMemberType().GetElementType();
            var fields = elementType.GetFieldsAndProperties(BindingFlags.Instance | BindingFlags.Public).Where(x => IsVisible(x)).OrderByDescending(x => x.DeclaringType.IsAbstract).ToArray();
            var database = ApplicationData.Businesses.Get(Business);
            var customFields = database.OfType<ManagerServer.Model.CustomField>().Where(x => x.Contains(elementType) && !x.Inactive).OrderBy(x => x.Position).ToArray();

            var customFields2 = new List<ICustomField>();
            customFields2.AddRange(database.UnorderedOfType<ManagerServer.Model.TextCustomField>().Cast<ManagerServer.Model.ICustomField>().Where(x => x.Contains(elementType) && !x.Inactive));
            customFields2.AddRange(database.UnorderedOfType<ManagerServer.Model.CheckboxCustomField>().Cast<ManagerServer.Model.ICustomField>().Where(x => x.Contains(elementType) && !x.Inactive));
            customFields2.AddRange(database.UnorderedOfType<ManagerServer.Model.DateCustomField>().Cast<ManagerServer.Model.ICustomField>().Where(x => x.Contains(elementType) && !x.Inactive));
            customFields2.AddRange(database.UnorderedOfType<ManagerServer.Model.MultipleValueCustomField>().Cast<ManagerServer.Model.ICustomField>().Where(x => x.Contains(elementType) && !x.Inactive));
            customFields2.AddRange(database.UnorderedOfType<ManagerServer.Model.NumberCustomField>().Cast<ManagerServer.Model.ICustomField>().Where(x => x.Contains(elementType) && !x.Inactive));
            customFields2.AddRange(database.UnorderedOfType<ManagerServer.Model.ImageCustomField>().Cast<ManagerServer.Model.ICustomField>().Where(x => x.Contains(elementType) && !x.Inactive));
            customFields2 = customFields2.OrderBy(x => x.Position).ToList();

            var businessDetails = ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.BusinessDetails>();
            var countryObjects = ManagerServer.Localizations.Localizations.Get(businessDetails.Obsolete_Country);
            var countryCustomFields = countryObjects.OfType<ManagerServer.Model.CustomField>().Where(x => x.Contains(elementType) && !x.Inactive).OrderBy(x => x.Position ?? int.MaxValue).ThenBy(x => x.Name).ToArray();
            var countryCustomFields2 = countryObjects.OfType<ManagerServer.Model.ICustomField>().Where(x => x.Contains(elementType) && !x.Inactive).OrderBy(x => x.Position ?? int.MaxValue).ThenBy(x => x.Name).ToArray();

            using (Div(@class: "form-group flex flex-col gap-2"))
            {
                using (Div(@class: "hidden not-supports-[grid-template-rows:subgrid]:block border p-4"))
                {
                    using (P()) Write("Your browser doesn’t support CSS subgrid.");
                    using (P()) Write("To view this layout correctly, update your browser to the latest version of Firefox, Chrome, or Safari.");
                }

                var columns = 0;

                columns++; // resize
                for (int i = 0; i < fields.Length; i++)
                {
                    if (fields[i].GetCustomAttribute<NoLabelAttribute>() != null)
                    {
                        continue; // Skip this column because we have already handled it previously with colspan
                    }
                    if (fields[i].GetMemberType() == typeof(ManagerServer.Model.CustomFields))
                    {
                        foreach (var e in countryCustomFields2)
                        {
                            columns++;
                        }
                        foreach (var e in customFields2)
                        {
                            columns++;
                        }
                    }
                    else if (fields[i].GetMemberType() == typeof(Dictionary<Guid, string>))
                    {
                        foreach (var e in countryCustomFields)
                        {
                            columns++;
                        }
                        foreach (var e in customFields)
                        {
                            columns++;
                        }
                    }
                    else
                    {
                        columns++;
                    }
                }
                columns++; // duplicate
                columns++; // delete

                using (Div(@class: "grid gap-y-1", style: $"grid-template-columns: repeat({columns}, auto)"))
                {
                    if (field.GetCustomAttribute<EmptyLabelAttribute>() == null)
                    {
                        if (fields.Any(x => x.GetCustomAttribute<EmptyLabelAttribute>() == null))
                        {
                            using (Div(@class: "contents"))
                            {
                                using (Div()) { }
                                var alignLeft = true;
                                for (int i = 0; i < fields.Length; i++)
                                {
                                    if (fields[i].GetCustomAttribute<NoLabelAttribute>() != null)
                                    {
                                        continue; // Skip this column because we have already handled it previously with colspan
                                    }
                                    else if (fields[i].GetMemberType() == typeof(ManagerServer.Model.CustomFields))
                                    {
                                        foreach (var e in countryCustomFields2)
                                        {
                                            using (Div(style: "white-space: nowrap; text-align: center")) using (Label()) Write(e.Name);
                                        }
                                        foreach (var e in customFields2)
                                        {
                                            using (Div(style: "white-space: nowrap; text-align: center")) using (Label()) Write(e.Name);
                                        }
                                    }
                                    else if (fields[i].GetMemberType() == typeof(Dictionary<Guid, string>))
                                    {
                                        foreach (var e in countryCustomFields)
                                        {
                                            using (Div(style: "white-space: nowrap; text-align: center")) using (Label()) Write(e.Name);
                                        }
                                        foreach (var e in customFields)
                                        {
                                            using (Div(style: "white-space: nowrap; text-align: center")) using (Label()) Write(e.Name);
                                        }
                                    }
                                    else
                                    {
                                        if (fields[i].GetMemberType() == typeof(decimal) || fields[i].GetMemberType() == typeof(decimal?)) alignLeft = false;
                                        if (field.GetCustomAttribute<FirstColumnLabelAttribute>() != null && i == 0) alignLeft = true;
                                        var style = alignLeft ? "text-align: start" : "white-space: nowrap; text-align: center";
                                        if (fields[i].GetCustomAttribute<LineNumberAttribute>() != null) style = "white-space: nowrap; text-align: center";
                                        using (Div(style: style))
                                        {
                                            using (If($"getIfAny{fields[i].Name}()"))
                                            {
                                                using (Label())
                                                {
                                                    if (field.GetCustomAttribute<FirstColumnLabelAttribute>() != null && i == 0)
                                                    {
                                                        Write(ManagerServer.Globalization.Strings.GetPropertyValue(field.Name));
                                                    }
                                                    else if (fields[i].GetCustomAttribute<EmptyLabelAttribute>() != null)
                                                    {
                                                        // Empty label
                                                    }
                                                    else if (fields[i].GetCustomAttribute<LabelAttribute>() != null)
                                                    {
                                                        var labelAttribute = fields[i].GetCustomAttribute<LabelAttribute>();
                                                        Write(labelAttribute.ToString());
                                                    }
                                                    else
                                                    {
                                                        Write(ManagerServer.Globalization.Strings.GetPropertyValue(fields[i].Name));
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                using (Div()) { }
                                using (Div()) { }
                            }
                        }
                    }

                    Write($@"<draggable v-model=""{field.Name}"" handle="".handle"" class=""contents"">");
                    using (Div(v_for: $"(lineItem, index) in {field.Name}", @class: "grid grid-cols-subgrid", style: $"grid-column: span {columns}"))
                    {
                        using (Div(@class: "handle flex items-stretch empty:pe-0 pe-1"))
                        {
                            using (Div(v_if: $"{field.Name}.length > 1", @class: "form-control cursor-move"))
                            {
                                I(@class: "fas fa-arrows-v opacity-50");
                            }
                        }
                        for (int i = 0; i < fields.Length; i++)
                        {
                            var currentField = fields[i];
                            var nextField = (i + 1 < fields.Length ? fields[i + 1] : null);

                            if (currentField.GetMemberType() == typeof(Dictionary<Guid, string>))
                            {
                                foreach (var e2 in countryCustomFields)
                                {
                                    using (Div(@class: "flex items-stretch empty:pe-0 pe-1"))
                                    {
                                        InputText(v_model: $"lineItem.{currentField.Name}['{e2.Key}']", @class: "form-control", style: "min-width: 100px");
                                    }
                                }
                                foreach (var e2 in customFields)
                                {
                                    using (Div(@class: "flex items-stretch empty:pe-0 pe-1"))
                                    {
                                        InputText(v_model: $"lineItem.{currentField.Name}['{e2.Key}']", @class: "form-control", style: "min-width: 100px");
                                    }
                                }
                            }
                            else if (fields[i].GetMemberType() == typeof(ManagerServer.Model.CustomFields))
                            {
                                foreach (var e2 in countryCustomFields2)
                                {
                                    using (Div(@class: "flex items-stretch empty:pe-0 pe-1"))
                                    {
                                        if (e2 is ManagerServer.Model.DateCustomField)
                                        {
                                            VInputDate($"lineItem.{currentField.Name}.Dates['{e2.Key}']", true, null, e2.LockedForManualEditing);
                                        }
                                        else if (e2 is ManagerServer.Model.NumberCustomField)
                                        {
                                            var control = $@"<input-decimal v-model=""lineItem.{currentField.Name}.Decimals['{e2.Key}']"" nullable=""true"" group-separator=""{System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberGroupSeparator}"" decimal-separator=""{System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator}""></input-decimal>";
                                            Write(control);
                                        }
                                        else if (e2 is ManagerServer.Model.MultipleValueCustomField multipleValueCustomField)
                                        {
                                            var v_model = $"lineItem.{currentField.Name}.StringArrays['{e2.Key}']";
                                            var options = multipleValueCustomField.Options?.Select(x => x.Value).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToArray();
                                            VueSelect(v_model, options: options, true, false, e2.LockedForManualEditing);
                                        }
                                        else if (e2 is ManagerServer.Model.TextCustomField textCustomField)
                                        {
                                            var v_model = $"lineItem.{currentField.Name}.Strings['{e2.Key}']";

                                            if (textCustomField.Type == ManagerServer.Model.Enums.TextCustomFieldType.ParagraphText)
                                            {
                                                Textarea(@class: "form-control field-sizing-content resize", v_model: v_model, @readonly: e2.LockedForManualEditing);
                                            }
                                            else if (textCustomField.Type == ManagerServer.Model.Enums.TextCustomFieldType.DropdownList)
                                            {
                                                var options = (textCustomField.OptionsForDropdownList ?? string.Empty).Replace("\r\n", "\n").Replace('\r', '\n').Split('\n').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                                                VueSelect(v_model, options, false, false, e2.LockedForManualEditing);
                                            }
                                            else
                                            {
                                                InputText(v_model: v_model, @class: "form-control", style: "min-width: 100px", @readonly: e2.LockedForManualEditing);
                                            }
                                        }
                                    }
                                }
                                foreach (var e2 in customFields2)
                                {
                                    using (Div(@class: "flex items-stretch empty:pe-0 pe-1"))
                                    {
                                        if (e2 is ManagerServer.Model.DateCustomField)
                                        {
                                            VInputDate($"lineItem.{currentField.Name}.Dates['{e2.Key}']", true, null, e2.LockedForManualEditing);
                                        }
                                        else if (e2 is ManagerServer.Model.CheckboxCustomField)
                                        {
                                            using (Label(@class: "text-center form-control block", style: "margin-bottom: 0px"))
                                            {
                                                InputCheckbox(value: "true", @class: "form-check-input", v_model: $"lineItem.{currentField.Name}.Booleans['{e2.Key}']", style: "margin-top: 0px", disabled: e2.LockedForManualEditing);
                                            }
                                        }
                                        else if (e2 is ManagerServer.Model.NumberCustomField)
                                        {
                                            var control = $@"<input-decimal v-model=""lineItem.{currentField.Name}.Decimals['{e2.Key}']"" nullable=""true"" group-separator=""{System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberGroupSeparator}"" decimal-separator=""{System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator}""></input-decimal>";
                                            Write(control);
                                        }
                                        else if (e2 is ManagerServer.Model.MultipleValueCustomField multipleValueCustomField)
                                        {
                                            var v_model = $"lineItem.{currentField.Name}.StringArrays['{e2.Key}']";
                                            var options = multipleValueCustomField.Options?.Select(x => x.Value).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToArray();
                                            VueSelect(v_model, options: options, true, false, e2.LockedForManualEditing);
                                        }
                                        else if (e2 is ManagerServer.Model.ImageCustomField imageCustomField)
                                        {
                                            InputText(@class: "form-control", style: "min-width: 100px", value: "Image custom field", @readonly: true);
                                        }
                                        else if (e2 is ManagerServer.Model.TextCustomField textCustomField)
                                        {
                                            var v_model = $"lineItem.{currentField.Name}.Strings['{e2.Key}']";

                                            if (textCustomField.Type == ManagerServer.Model.Enums.TextCustomFieldType.ParagraphText)
                                            {
                                                Textarea(@class: "form-control field-sizing-content resize", v_model: v_model, @readonly: e2.LockedForManualEditing);
                                            }
                                            else if (textCustomField.Type == ManagerServer.Model.Enums.TextCustomFieldType.DropdownList)
                                            {
                                                var options = (textCustomField.OptionsForDropdownList ?? string.Empty).Replace("\r\n", "\n").Replace('\r', '\n').Split('\n').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                                                VueSelect(v_model, options, false, false, e2.LockedForManualEditing);
                                            }
                                            else
                                            {
                                                InputText(v_model: v_model, @class: "form-control", style: "min-width: 100px", @readonly: e2.LockedForManualEditing);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (currentField.GetCustomAttribute<NoLabelAttribute>() == null && nextField?.GetCustomAttribute<NoLabelAttribute>() == null) // No colspan on this column
                                {
                                    using (Div(@class: "flex items-stretch empty:pe-0 pe-1"))
                                    {
                                        using (If($"getIfAny{currentField.Name}()"))
                                        {
                                            using (If($"getIf{currentField.Name}(lineItem)"))
                                            {
                                                using (Div(@class: "flex items-stretch grow")) CellField(fields, currentField);
                                            }
                                            Write("<template v-else>");
                                            InputText(@class: "form-control", @readonly: true);
                                            Write("</template>");
                                        }
                                    }
                                }
                                else if (currentField.GetCustomAttribute<NoLabelAttribute>() == null)
                                {
                                    using (Div(@class: "flex items-stretch gap-1 empty:pe-0 pe-1"))
                                    {
                                        var innerFields = new List<MemberInfo>();
                                        innerFields.Add(currentField);
                                        for (int i2 = i + 1; i2 < fields.Length; i2++)
                                        {
                                            if (fields[i2].GetCustomAttribute<NoLabelAttribute>() == null) break;
                                            innerFields.Add(fields[i2]);
                                        }

                                        var remainingFields = innerFields.ToList();
                                        foreach (var e in innerFields)
                                        {
                                            remainingFields.Remove(e);

                                            var colspanExpression = "1";
                                            if (remainingFields.Any())
                                            {
                                                colspanExpression = "([ " + string.Join(',', remainingFields.Select(x => $"getIf{x.Name}(lineItem)")) + " ].filter(x => x).length) > 0 ? 1 : ([ " + string.Join(',', innerFields.Select(x => $"getIf{x.Name}(lineItem)")) + " ].filter(x => !x).length+1)";
                                            }
                                            else
                                            {
                                                colspanExpression = "([ " + string.Join(',', innerFields.Select(x => $"getIf{x.Name}(lineItem)")) + " ].filter(x => !x).length+1)";
                                            }

                                            using (If($"getIf{e.Name}(lineItem)"))
                                            {
                                                using (Div(@class: "flex items-stretch grow")) CellField(fields, e);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        using (Div(@class: "flex items-stretch"))
                        {
                            using (Button(@class: "form-control me-1 cursor-pointer", title: "Copy", v_on_click: $"{field.Name}.splice(index, 0, JSON.parse(JSON.stringify({field.Name}[index])))"))
                            {
                                I(@class: "fas fa-table-rows opacity-50");
                            }
                        }
                        using (Div(@class: "flex items-center"))
                        {
                            using (Div(v_if: field.Name + ".length > 1"))
                            {
                                using (Button(@class: "text-2xl font-bold cursor-pointer opacity-25 hover:opacity-50", v_on_click: field.Name + ".splice(index, 1)")) Write("&times;");
                            }
                        }
                    }
                    Write("</draggable>");

                    using (Div(@class: "contents"))
                    {
                        using (Div()) { }
                        for (int i = 0; i < fields.Length; i++)
                        {
                            if (fields[i].GetCustomAttribute<NoLabelAttribute>() != null)
                            {
                                continue; // Skip this column because we have already handled it previously with colspan
                            }
                            else if (fields[i].GetMemberType() == typeof(ManagerServer.Model.CustomFields))
                            {
                                foreach (var e2 in countryCustomFields2)
                                {
                                    using (Div(@class: "flex items-stretch empty:pe-0 pe-1")) { }
                                }
                                foreach (var e2 in customFields2)
                                {
                                    using (Div(@class: "flex items-stretch empty:pe-0 pe-1")) { }
                                }
                            }
                            else if (fields[i].GetMemberType() == typeof(Dictionary<Guid, string>))
                            {
                                foreach (var e2 in countryCustomFields)
                                {
                                    using (Div(@class: "flex items-stretch empty:pe-0 pe-1")) { }
                                }
                                foreach (var e2 in customFields)
                                {
                                    using (Div(@class: "flex items-stretch empty:pe-0 pe-1")) { }
                                }
                            }
                            else
                            {
                                using (Div(@class: "flex items-stretch empty:pe-0 pe-1"))
                                {
                                    using (If($"{field.Name}.length > 1 && getIfAny{fields[i].Name}()"))
                                    {
                                        if (fields[i].GetCustomAttribute<SumAttribute>() != null)
                                        {
                                            CellExpression(fields[i], $"{field.Name}.map(x => get{fields[i].Name}(x)).reduce(function (item1, item2) {{ return item1.plus(item2); }}, new Decimal(0)).toNumber().toLocaleString()", true);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                var label = field.GetCustomAttribute<ManagerServer.Model.Attributes.AddLineLabelAttribute>()?.GetTranslatedString() ?? ManagerServer.Globalization.Strings.AddLine;

                using (Div(@class: "flex"))
                {
                    using (Details(@class: "dropdown"))
                    {
                        using (Summary(@class: "btn btn-sm")) Write(label);
                        using (Div(@class: "dropdown-menu"))
                        {
                            using (Button(@class: "dropdown-item", onclick: "this.closest('details').open = false", v_on_click: $"addTo{field.Name}")) Write(label);
                            using (Button(@class: "dropdown-item", onclick: "this.closest('details').open = false", v_on_click: $"for (i = 0; i < 5; i++) addTo{field.Name}()")) Write(label + " (5&times;)");
                            using (Button(@class: "dropdown-item", onclick: "this.closest('details').open = false", v_on_click: $"for (i = 0; i < 10; i++) addTo{field.Name}()")) Write(label + " (10&times;)");
                            using (Button(@class: "dropdown-item", onclick: "this.closest('details').open = false", v_on_click: $"for (i = 0; i < 20; i++) addTo{field.Name}()")) Write(label + " (20&times;)");
                        }
                    }
                }
            }            
        }
        
        private void CellField(MemberInfo[] fields, MemberInfo field)
        {
            if (field.GetCustomAttribute<ManagerServer.Model.Attributes.SubstituteAttribute>() != null)
            {
                using (If($"get{field.Name}Substitute(lineItem) != null"))
                {                    
                    var cellExpression = $"(get{field.Name}Substitute(lineItem) || {{}}).UniqueName";
                    CellExpression(field, cellExpression, false);
                }
                Write("<template v-else>");
            }

            if (field.GetMemberType() == typeof(string))
            {
                if (field.GetCustomAttribute<TextareaAttribute>() != null)
                {
                    Textarea(@class: "form-control field-sizing-content resize", style: "min-inline-size: 20ch; min-block-size: 3lh", v_model: $"lineItem.{field.Name}");
                }
                else
                {
                    VInputTextWithPrependAppend("lineItem.", field);
                }
            }
            else if (field.GetMemberType() == typeof(DateTime))
            {
                VInputDate("lineItem.", field);
            }
            else if (field.GetMemberType() == typeof(bool))
            {
                VInputBoolean("lineItem.", field);
            }
            else if (field.GetMemberType() == typeof(Guid?) || field.GetMemberType() == typeof(Guid[]))
            {
                AutocompleteSelect("lineItem.", field);
            }
            else if (field.GetMemberType() == typeof(string[]))
            {
                var control = $@"<select2-tags v-model=""lineItem.{field.Name}""></select2-tags>";
                Write(control);
            }
            else if (field.GetMemberType() == typeof(decimal) || field.GetMemberType() == typeof(decimal?))
            {
                if (fields.Length == 1)
                {
                    using (Div(@class: "input-group"))
                    {
                        using (Span(@class: "input-group-text input-sm")) Write(ManagerServer.Globalization.Strings.GetPropertyValue(field.Name));
                        VInputDecimal("lineItem.", field, false);
                    }
                }
                else
                {
                    VInputDecimal("lineItem.", field, false);
                }
            }
            else if (field.GetMemberType().IsEnum)
            {
                SelectWithPrependAppend("lineItem.", field, true);
            }
            else if (field.GetMemberType() == typeof(object))
            {
                if (field.GetCustomAttribute<ExpressionAttribute>() is ExpressionAttribute expression && expression.IsDecimal)
                {
                    CellExpression(field, $"get{field.Name}(lineItem).toNumber().toLocaleString()", true);
                }
                else if (field.GetCustomAttribute<LineNumberAttribute>() != null)
                {
                    using (Span(@class: "form-control text-center font-semibold whitespace-nowrap"))
                    {
                        Write("{{ index+1 }}");
                    }
                }
                else
                {
                    CellExpression(field, $"get{field.Name}(lineItem)", true);
                }
            }
            else if (field.GetMemberType() == typeof(ManagerServer.Model.MemberInfo))
            {
                ReflectionAutocompleteSelect("lineItem.", field);
            }

            if (field.GetCustomAttribute<ManagerServer.Model.Attributes.SubstituteAttribute>() != null) Write("</template>");
        }

        private void VTextarea(string prefix, MemberInfo field)
        {
            var width = "min-block-size: 6lh; min-inline-size: 40ch";
            string placeholder = null;
            if (field.GetCustomAttribute<PlaceholderAttribute>() != null)
            {
                var placeholderLine = field.GetCustomAttribute<PlaceholderAttribute>()?.ToString();
                if (!string.IsNullOrWhiteSpace(placeholderLine))
                {
                    for (int i = 1; i < 5; i++) placeholder += placeholderLine + $" {i}\n";
                }
            }
            if (field.GetCustomAttribute<LongAttribute>() != null) width = "min-block-size: 6lh; min-inline-size: 60ch";
            Textarea(v_model: prefix+field.Name, @class: "form-control field-sizing-content resize", style: width, placeholder: placeholder);
        }

        private string AsHtmlAtribute(string s)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\"");
            if (!string.IsNullOrEmpty(s))
            {
                sb.Append(s.Replace("\"", " & quot;"));
            }
            sb.Append("\"");
            return sb.ToString();
        }

        private void CellExpression(MemberInfo field, string expression, bool isRightAligned)
        {
            var prepend = field.GetCustomAttribute<PrependAttribute>()?.ToString();
            var append = field.GetCustomAttribute<AppendAttribute>()?.ToString();
            var appendExpression = field.GetCustomAttribute<AppendCurrencyAttribute>()?.GetExpression() ?? field.GetCustomAttribute<AppendValueAttribute>()?.GetExpression();

            if (prepend == null & append == null && appendExpression == null)
            {
                using (Div(@class: "input-group grow"))
                {
                    using (Span(@class: "input-group-text input-sm grow" + (isRightAligned ? " text-right pl-4" : null))) Write("{{ " + expression + " }}");
                }
            }
            else
            {
                using (Div(@class: "input-group grow"))
                {
                    if (prepend != null) using (Span(@class: "input-group-text input-sm")) Write(prepend.ToString());
                    using (Span(@class: "input-group-text input-sm grow"+(isRightAligned ? " text-right pl-4" : null))) Write("{{ " + expression + " }}");
                    if (append != null) using (Span(@class: "input-group-text input-sm")) Write(append.ToString());
                    if (appendExpression != null) using (Span(@class: "input-group-text input-sm")) Write("{{ " + appendExpression + " }}");
                }
            }
        }

        private void LiquidEditor(MemberInfo field)
        {
            Script(src: "resources/ace/ace.js");
            Write(@"<liquid-editor v-model="""+field.Name+@"""></liquid-editor>");
        }

        private void HtmlEditor(MemberInfo field)
        {
            Script(src: "resources/ace/ace.js");
            Write(@"<html-editor v-model=""" + field.Name + @"""></html-editor>");
        }

        private void JavascriptEditor(MemberInfo field)
        {
            Script(src: "resources/ace/ace.js");
            Write(@"<javascript-editor v-model=""" + field.Name + @"""></javascript-editor>");
        }

        private void SelectWithPrependAppend(string prefix, MemberInfo field, bool fullWidth)
        {
            var prepend = field.GetCustomAttribute<PrependAttribute>();
            var append = field.GetCustomAttribute<AppendAttribute>();
            var appendValue = field.GetCustomAttribute<AppendValueAttribute>();
            if (prepend != null || append != null || appendValue != null)
            {
                using (Div(@class: "input-group"))
                {
                    if (prepend != null) using (Span(@class: "input-group-text input-sm")) Write(prepend.ToString());
                    Select(prefix, field, fullWidth);
                    if (appendValue != null) using (Span(@class: "input-group-text input-sm")) Write("{{ " + appendValue.GetExpression() + " }}");
                    if (append != null) using (Span(@class: "input-group-text input-sm")) Write(append.ToString());
                }
            }
            else
            {
                Select(prefix, field, fullWidth);
            }
        }

        private void Select(string prefix, MemberInfo field, bool fullWidth)
        {
            var style = string.Empty;
            if (fullWidth) style = "width: 100%";

            using (Select(v_model_number: prefix+field.Name, @class: "form-select", style: style))
            {
                foreach (var e in Enum.GetValues(field.GetMemberType()))
                {
                    var text = ManagerServer.Globalization.Strings.GetPropertyValue(e.ToString());
                    if (field.GetMemberType() == typeof(ManagerServer.Model.Enums.BalanceSheetLayout))
                    {
                        var equityName = ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.Equity>().GetName();

                        if ((ManagerServer.Model.Enums.BalanceSheetLayout)e == ManagerServer.Model.Enums.BalanceSheetLayout.AssetsEqualsEquityLiabilities)
                        {
                            text = $"{Strings.Assets} = {equityName} + {Strings.Liabilities}";
                        }
                        if ((ManagerServer.Model.Enums.BalanceSheetLayout)e == ManagerServer.Model.Enums.BalanceSheetLayout.AssetsEqualsLiabilitiesEquity)
                        {
                            text = $"{Strings.Assets} = {Strings.Liabilities} + {equityName}";
                        }
                        if ((ManagerServer.Model.Enums.BalanceSheetLayout)e == ManagerServer.Model.Enums.BalanceSheetLayout.AssetsLiabilitiesEqualsEquity)
                        {
                            text = $"{Strings.Assets} - {Strings.Liabilities} = {equityName}";
                        }
                    }
                    if (field.GetMemberType() == typeof(ManagerServer.Model.Enums.SmtpPort))
                    {
                        if ((ManagerServer.Model.Enums.SmtpPort)e == ManagerServer.Model.Enums.SmtpPort._25)
                        {
                            text = "25";
                        }
                        if ((ManagerServer.Model.Enums.SmtpPort)e == ManagerServer.Model.Enums.SmtpPort._465)
                        {
                            text = "465";
                        }
                        if ((ManagerServer.Model.Enums.SmtpPort)e == ManagerServer.Model.Enums.SmtpPort._587)
                        {
                            text = "587";
                        }
                    }

                    Option(value: ((int)e).ToString(), text: text);
                }
            }
        }

        private void ReflectionAutocompleteSelect(string prefix, MemberInfo field)
        {
            using (Div(@class: "controls", style: $"min-width: 200px"))
            {
                var autocompleteAttribute = field.GetCustomAttribute<ManagerServer.Model.Attributes.MemberInfoAutocompleteAttribute>();
                string autocompleteFilter = null;
                if (autocompleteAttribute != null)
                {
                    if (autocompleteAttribute.Filter is Type type)
                    {
                        autocompleteFilter = $"'{type.FullName}'";
                    }
                    if (autocompleteAttribute.Filter is string s)
                    {
                        autocompleteFilter = $"(get{autocompleteAttribute.Filter}(typeof lineItem === typeof undefined ? null : lineItem) || {{}})." + nameof(ManagerServer.Model.MemberInfo.DeclaringType);
                    }
                }

                var onChangeAttributeExpressions = new List<string>();
                onChangeAttributeExpressions.AddRange(field.GetCustomAttributes<ManagerServer.Model.Attributes.OnChangeSetNullAttribute>().Select(x => x.GetExpression()));
                var onChangeAttributeExpression = string.Join(';', onChangeAttributeExpressions);

                var control = $@"<select2 v-model=""{prefix}{field.Name}"" :autocomplete-on-change=""function(item) {{ {onChangeAttributeExpression} }}"" multiple=""false"" autocomplete-url=""{new ReflectionAutocomplete() { Business = Business }.ToUrl()}"" :autocomplete-filter=""{autocompleteFilter}"" no-matches-text={AsHtmlAtribute(Strings.NoMatchesFound)} searching-text={AsHtmlAtribute(Strings.Searching)} placeholder={AsHtmlAtribute(" ")}></select2>";

                var prepend = field.GetCustomAttribute<PrependAttribute>();
                var append = field.GetCustomAttribute<AppendAttribute>();
                if (prepend != null || append != null)
                {
                    using (Div(@class: "input-group"))
                    {
                        if (prepend != null) using (Span(@class: "input-group-text input-sm")) Write(prepend.ToString());
                        Write(control);
                        if (append != null) using (Span(@class: "input-group-text input-sm")) Write(append.ToString());
                    }
                }
                else
                {
                    Write(control);
                }
            }
        }

        private void AutocompleteSelect(string prefix, MemberInfo field)
        {
            //var minWidth = "300px";
            //if (field.GetCustomAttribute<ShortAttribute>() != null) minWidth = "120px";

            var types = new Guid[0];
            var autocompleteFilter = "''";
            var autocompleteAttribute = field.GetCustomAttribute<ManagerServer.Model.Attributes.AutocompleteAttribute>();
            var placeholder = GetAutocompletePlaceholder(autocompleteAttribute);
            if (field.GetMemberType() == typeof(Guid[])) placeholder = null;
            if (string.IsNullOrWhiteSpace(placeholder))
            {
                if (field.GetCustomAttribute<PlaceholderAttribute>() != null)
                {
                    placeholder = field.GetCustomAttribute<PlaceholderAttribute>().ToString();
                }
            }
            if (autocompleteAttribute != null)
            {
                if (autocompleteAttribute.Value == null)
                {
                }
                else if (autocompleteAttribute.Value.IsInterface)
                {
                    types = typeof(ManagerServer.Model.Object).Assembly.GetTypes().Where(x => !x.IsAbstract && x.GetInterfaces().Contains(autocompleteAttribute.Value)).Select(x => ManagerServer.Model.Object.GetGuidByType(x)).ToArray();
                }
                else if (autocompleteAttribute.Value.IsAbstract)
                {
                    types = typeof(ManagerServer.Model.Object).Assembly.GetTypes().Where(x => !x.IsAbstract && x.IsSubclassOf(autocompleteAttribute.Value)).Select(x => ManagerServer.Model.Object.GetGuidByType(x)).ToArray();
                }
                else
                {
                    types = new Guid[] { ManagerServer.Model.Attributes.GuidAttribute.GetGuidByType(autocompleteAttribute.Value) };
                }
                if (autocompleteAttribute.Filter is string s && !string.IsNullOrWhiteSpace(s))
                {
                    autocompleteFilter = $"(get{autocompleteAttribute.Filter}((typeof lineItem === typeof undefined) ? null : lineItem) || {{}}).Key";
                }
                if (autocompleteAttribute.Filter is Type t)
                {
                    autocompleteFilter = $"'" + ManagerServer.Model.Object.GetGuidByType(t).ToString() + "'";
                }
            }

            var onChangeAttributeExpressions = new List<string>();
            onChangeAttributeExpressions.AddRange(field.GetCustomAttributes<ManagerServer.Model.Attributes.OnChangeSetNullAttribute>().Select(x => x.GetExpression()));
            onChangeAttributeExpressions.AddRange(field.GetCustomAttributes<ManagerServer.Model.Attributes.OnChangeSetDefaultAttribute>().Select(x => x.GetExpression()));

            var customFields = ApplicationData.Businesses.Get(Business).GetCustomFields(field.DeclaringType);
            foreach (var e in customFields)
            {
                if (e is TextCustomField) onChangeAttributeExpressions.Add($"if (item != null) updateCustomField('Strings', (typeof lineItem === typeof undefined) ? CustomFields2 : lineItem.CustomFields2, item.CustomFields2, '{e.Key}')");
                if (e is NumberCustomField) onChangeAttributeExpressions.Add($"if (item != null) updateCustomField('Decimals', (typeof lineItem === typeof undefined) ? CustomFields2 : lineItem.CustomFields2, item.CustomFields2, '{e.Key}')");
                if (e is CheckboxCustomField) onChangeAttributeExpressions.Add($"if (item != null) updateCustomField('Booleans', (typeof lineItem === typeof undefined) ? CustomFields2 : lineItem.CustomFields2, item.CustomFields2, '{e.Key}')");
                if (e is DateCustomField) onChangeAttributeExpressions.Add($"if (item != null) updateCustomField('Dates', (typeof lineItem === typeof undefined) ? CustomFields2 : lineItem.CustomFields2, item.CustomFields2, '{e.Key}')");
                if (e is MultipleValueCustomField) onChangeAttributeExpressions.Add($"if (item != null) updateCustomField('StringArrays', (typeof lineItem === typeof undefined) ? CustomFields2 : lineItem.CustomFields2, item.CustomFields2, '{e.Key}')");
            }

            var onChangeAttributeExpression = string.Join(';', onChangeAttributeExpressions);

            var expand = new List<string>();
            expand.AddRange(field.GetCustomAttributes<OnChangeSetDefaultAttribute>().Select(x => "Default" + x.Field));

            var fields = typeof(T).GetFieldsAndProperties().ToList();
            fields.AddRange(fields.ToArray().Where(x => x.GetMemberType().IsArray).SelectMany(x => x.GetMemberType().GetElementType().GetFieldsAndProperties()));
            foreach (var e in fields.SelectMany(x => x.GetCustomAttributes<SubstituteAttribute>()).Where(x => x.Path[0] == field.Name))
            {
                var value = string.Join('.', e.Path.Skip(1));
                expand.Add(value);
            }
            if (!string.IsNullOrWhiteSpace(autocompleteAttribute?.Subtext)) expand.Add(autocompleteAttribute.Subtext);

            var multiple = "false";
            if (field.GetMemberType() == typeof(Guid[])) multiple = "true";

            var control = $@"<select2 v-model=""{prefix}{field.Name}"" :autocomplete-on-change=""function(item) {{ {onChangeAttributeExpression} }}"" multiple=""{multiple}"" autocomplete-subtext=""{autocompleteAttribute?.Subtext}"" autocomplete-url=""{new Autocomplete() { Business = Business, Types = types, Expand = expand.ToArray() }.ToUrl()}"" :autocomplete-filter=""{autocompleteFilter}"" no-matches-text={AsHtmlAtribute(Strings.NoMatchesFound)} searching-text={AsHtmlAtribute(Strings.Searching)} placeholder={AsHtmlAtribute(placeholder)}></select2>";

            var prepend = field.GetCustomAttribute<PrependAttribute>();
            var append = field.GetCustomAttribute<AppendAttribute>();
            if (prepend != null || append != null)
            {
                using (Div(@class: "input-group"))
                {
                    if (prepend != null) using (Span(@class: "input-group-text input-sm")) Write(prepend.ToString());
                    Write(control);
                    if (append != null) using (Span(@class: "input-group-text input-sm")) Write(append.ToString());
                }
            }
            else
            {
                Write(control);
            }
        }

        private string GetAutocompletePlaceholder(AutocompleteAttribute autocompleteAttribute)
        {
            if (autocompleteAttribute == null) return " ";

            if (autocompleteAttribute.Placeholder == typeof(ManagerServer.Model.Equity)) return ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.Equity>().GetName();
            if (autocompleteAttribute.Placeholder == typeof(ManagerServer.Model.ProfitAndLossStatementAccountFixedAssetLossOnDisposal)) return ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.ProfitAndLossStatementAccountFixedAssetLossOnDisposal>().GetName();
            if (autocompleteAttribute.Placeholder == typeof(ManagerServer.Model.ProfitAndLossStatementAccountFixedAssetDepreciation)) return ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.ProfitAndLossStatementAccountFixedAssetDepreciation>().GetName();
            if (autocompleteAttribute.Placeholder == typeof(ManagerServer.Model.ProfitAndLossStatementAccountIntangibleAssetsGainsLossOnDisposal)) return ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.ProfitAndLossStatementAccountIntangibleAssetsGainsLossOnDisposal>().GetName();
            if (autocompleteAttribute.Placeholder == typeof(ManagerServer.Model.ProfitAndLossStatementAccountIntangibleAssetsAmortization)) return ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.ProfitAndLossStatementAccountIntangibleAssetsAmortization>().GetName();
            if (autocompleteAttribute.Placeholder == typeof(ManagerServer.Model.ProfitAndLossStatementAccountInventorySales)) return ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.ProfitAndLossStatementAccountInventorySales>().GetName();
            if (autocompleteAttribute.Placeholder == typeof(ManagerServer.Model.ProfitAndLossStatementAccountInventoryPurchases)) return ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.ProfitAndLossStatementAccountInventoryPurchases>().GetName();
            if (autocompleteAttribute.Placeholder == typeof(ManagerServer.Model.BalanceSheetTaxPayableAccount)) return ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.BalanceSheetTaxPayableAccount>().GetName();
            if (autocompleteAttribute.Value == typeof(ManagerServer.Model.ProfitAndLossStatementGroup)) return Strings.Uncategorized;
            if (autocompleteAttribute.Value == typeof(ManagerServer.Model.INonInventoryItemAccount)) return Strings.Suspense;
            if (autocompleteAttribute.Value == typeof(ManagerServer.Model.TaxCode)) return Strings.NoTax;
            if (autocompleteAttribute.Value == typeof(ManagerServer.Model.Project)) return Strings.Optional;
            if (autocompleteAttribute.Value == typeof(ManagerServer.Model.Division)) return Strings.Optional;
            if (autocompleteAttribute.Value == typeof(ManagerServer.Model.CashFlowStatementFinancingActivityGroup)) return Strings.Optional;
            if (autocompleteAttribute.Value == typeof(ManagerServer.Model.CashFlowStatementInvestingActivityGroup)) return Strings.Optional;
            if (autocompleteAttribute.Value == typeof(ManagerServer.Model.CashFlowStatementOperatingActivityGroup)) return Strings.Optional;
            if (autocompleteAttribute.Value == typeof(ManagerServer.Model.BankOrCashAccount)) return Strings.Suspense;
            if (autocompleteAttribute.Value == typeof(ManagerServer.Model.CustomTheme)) return Strings.Optional;
            if (autocompleteAttribute.Value == typeof(ManagerServer.Model.IGeneralLedgerAccount)) return Strings.Empty;
            if (autocompleteAttribute.Value == typeof(ManagerServer.Model.ICustomGeneralLedgerAccount)) return Strings.Suspense;
            if (autocompleteAttribute.Value == typeof(ManagerServer.Model.IReceiptOrPaymentAccount)) return Strings.Suspense;
            if (autocompleteAttribute.Value == typeof(ManagerServer.Model.IJournalEntryAccount)) return Strings.Suspense;
            if (autocompleteAttribute.Value == typeof(ManagerServer.Model.BalanceSheetAbstractGroup)) return Strings.Uncategorized;
            if (autocompleteAttribute.Value == typeof(ManagerServer.Model.CustomInventoryLocation)) return ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.DefaultInventoryLocation>().GetName();
            if (autocompleteAttribute.Value == typeof(ManagerServer.Model.ControlAccountForBankAccounts)) return ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.BalanceSheetCashAtBankAccount>().GetName();
            if (autocompleteAttribute.Value == typeof(ManagerServer.Model.ControlAccountForCapitalAccounts)) return ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.BalanceSheetCapitalAccountsAccount>().GetName();
            if (autocompleteAttribute.Value == typeof(ManagerServer.Model.ControlAccountForInvestments)) return ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.BalanceSheetInvestmentsAccount>().GetName();
            if (autocompleteAttribute.Value == typeof(ManagerServer.Model.ControlAccountForCustomers)) return ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.BalanceSheetAccountsReceivableAccount>().GetName();
            if (autocompleteAttribute.Value == typeof(ManagerServer.Model.ControlAccountForEmployees)) return ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.BalanceSheetEmployeeClearingAccount>().GetName();
            if (autocompleteAttribute.Value == typeof(ManagerServer.Model.ControlAccountForFixedAssets)) return ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.BalanceSheetFixedAssetsAtCostAccount>().GetName();
            if (autocompleteAttribute.Value == typeof(ManagerServer.Model.ControlAccountForFixedAssetsAccumulatedDepreciation)) return ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.BalanceSheetFixedAssetsAccumulatedDepreciationAccount>().GetName();
            if (autocompleteAttribute.Value == typeof(ManagerServer.Model.ControlAccountForIntangibleAssets)) return ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.BalanceSheetIntangibleAssetsAtCostAccount>().GetName();
            if (autocompleteAttribute.Value == typeof(ManagerServer.Model.ControlAccountForIntangibleAssetsAccumulatedAmortization)) return ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.BalanceSheetIntangibleAssetsAccumulatedAmortizationAccount>().GetName();
            if (autocompleteAttribute.Value == typeof(ManagerServer.Model.ControlAccountForInventoryItems)) return ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.BalanceSheetInventoryOnHandAccount>().GetName();
            if (autocompleteAttribute.Value == typeof(ManagerServer.Model.ControlAccountForSpecialAccounts)) return ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.BalanceSheetSpecialAccountsAccount>().GetName();
            if (autocompleteAttribute.Value == typeof(ManagerServer.Model.ControlAccountForSuppliers)) return ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.BalanceSheetAccountsPayableAccount>().GetName();
            if (autocompleteAttribute.Value == typeof(ManagerServer.Model.ForeignCurrency)) return ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.BaseCurrency>().GetDisplayName();
            return " ";
        }

        private void CustomFields(string fieldName)
        {
            var businessDetails = ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.BusinessDetails>();
            var countryObjects = ManagerServer.Localizations.Localizations.Get(businessDetails.Obsolete_Country);
            var countryCustomFields = countryObjects.OfType<ManagerServer.Model.CustomField>().Where(x => x.Contains(typeof(T)) && !x.Inactive).OrderBy(x => x.Position ?? int.MaxValue).ThenBy(x => x.Name).ToArray();
            if (countryCustomFields.Any())
            {
                CustomFields2(countryCustomFields, businessDetails.Obsolete_Country);
            }

            var customFields = ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.CustomField>().Where(x => x.Contains(typeof(T)) && !x.Inactive).OrderBy(x => x.Position ?? int.MaxValue).ThenBy(x => x.Name).ToArray();

            if (customFields.Any())
            {
                CustomFields2(customFields, Strings.CustomFields);
            }
        }

        private void CustomFields2(ManagerServer.Model.CustomField[] customFields, string name)
        {
            using (Fieldset())
            {
                using (Legend()) Write(Locales.GetNativeName(name));
                foreach (var e in customFields)
                {
                    using (Div(@class: "form-group"))
                    {
                        using (Label()) Write(e.Name);
                        if (e.Type == ManagerServer.Model.Enums.CustomFieldStyle.SingleLineText)
                        {
                            var width = "100px";
                            if (e.Size == ManagerServer.Model.Enums.CustomFieldSize.Medium) width = "300px";
                            if (e.Size == ManagerServer.Model.Enums.CustomFieldSize.Large) width = "500px";
                            InputText(v_model: "CustomFields['" + e.Key.ToString() + "']", @class: "form-control", style: "width: " + width);
                        }
                        if (e.Type == ManagerServer.Model.Enums.CustomFieldStyle.ParagraphText)
                        {
                            var width = "300px";
                            if (e.Size == ManagerServer.Model.Enums.CustomFieldSize.Medium) width = "500px";
                            if (e.Size == ManagerServer.Model.Enums.CustomFieldSize.Large) width = "700px";
                            this.Textarea(v_model: "CustomFields['" + e.Key.ToString() + "']", rows: 4, @class: "form-control field-sizing-content resize", style: "width: " + width);
                        }
                        if (e.Type == ManagerServer.Model.Enums.CustomFieldStyle.DropdownList)
                        {
                            var options = (e.OptionsForDropdownList ?? string.Empty).Replace("\r\n", "\n").Replace('\r', '\n').Split('\n').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                            using (Div()) VueSelect($"CustomFields['{e.Key}']", options, false, true, false);
                        }
                        if (e.Type == ManagerServer.Model.Enums.CustomFieldStyle.Image)
                        {
                            InputText(v_model: "CustomFields['" + e.Key.ToString() + "']", @class: "form-control", style: "width: 300px");
                        }
                        if (e.Type == ManagerServer.Model.Enums.CustomFieldStyle.Date)
                        {
                            VInputDate("CustomFields['" + e.Key.ToString() + "']", true, null, false);
                        }
                        if (e.Type == ManagerServer.Model.Enums.CustomFieldStyle.Number)
                        {
                            InputText(v_model: "CustomFields['" + e.Key.ToString() + "']", @class: "form-control", style: "width: 100px; text-align: right");
                        }
                        if (!string.IsNullOrWhiteSpace(e.Description))
                        {
                            var description = e.Description;
                            var links = e.Description.Split(' ').Where(x => x.ToLowerInvariant().StartsWith("http://") || x.ToLowerInvariant().StartsWith("https://")).ToArray();
                            foreach (var link in links)
                            {
                                description = description.Replace(link, @" &mdash; <a href=""" + link + @""" target=""_blank"">" + Strings.LearnMore + "</a>");
                            }
                            using (P(@class: "help-block")) Write(description);
                        }
                    }
                }
            }
        }

        private void CustomFields3(string fieldName)
        {
            var database = ApplicationData.Businesses.Get(Business);
            var businessDetails = database.Single<ManagerServer.Model.BusinessDetails>();
            var countryObjects = ManagerServer.Localizations.Localizations.Get(businessDetails.Obsolete_Country);
            var countryCustomFields = countryObjects.OfType<ManagerServer.Model.ICustomField>().Where(x => x.Contains(typeof(T)) && !x.Inactive).OrderBy(x => x.Position ?? int.MaxValue).ThenBy(x => x.Name).ToArray();
            if (countryCustomFields.Any())
            {
                CustomFields4(countryCustomFields, businessDetails.Obsolete_Country, fieldName);
            }

            var customFields = new List<ICustomField>();
            customFields.AddRange(database.OfType<TextCustomField>().Cast<ICustomField>().Where(x => x.Contains(typeof(T)) && !x.Inactive));
            customFields.AddRange(database.OfType<DateCustomField>().Cast<ICustomField>().Where(x => x.Contains(typeof(T)) && !x.Inactive));
            customFields.AddRange(database.OfType<NumberCustomField>().Cast<ICustomField>().Where(x => x.Contains(typeof(T)) && !x.Inactive));
            customFields.AddRange(database.OfType<CheckboxCustomField>().Cast<ICustomField>().Where(x => x.Contains(typeof(T)) && !x.Inactive));
            customFields.AddRange(database.OfType<MultipleValueCustomField>().Cast<ICustomField>().Where(x => x.Contains(typeof(T)) && !x.Inactive));
            customFields.AddRange(database.OfType<ImageCustomField>().Cast<ICustomField>().Where(x => x.Contains(typeof(T)) && !x.Inactive));

            if (customFields.Any())
            {
                CustomFields4(customFields.OrderBy(x => x.Position ?? int.MaxValue).ThenBy(x => x.Name).ToArray(), Strings.CustomFields, fieldName);
            }
        }

        private void CustomFields4(ManagerServer.Model.ICustomField[] customFields, string name, string fieldName)
        {
            using (Fieldset())
            {
                using (Legend()) Write(Locales.GetNativeName(name));
                foreach (var e in customFields)
                {
                    using (Div(@class: "form-group"))
                    {
                        if (e is ManagerServer.Model.DateCustomField)
                        {
                            using (Label()) Write(e.Name);
                            VInputDate($"{fieldName}.Dates['{e.Key}']", true, null, e.LockedForManualEditing);
                        }
                        else if (e is ManagerServer.Model.NumberCustomField)
                        {
                            using (Label()) Write(e.Name);
                            var control = $@"<input-decimal v-model=""{fieldName}.Decimals['{e.Key}']"" nullable=""true"" :readonly=""{e.LockedForManualEditing.ToString().ToLowerInvariant()}"" group-separator=""{System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberGroupSeparator}"" decimal-separator=""{System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator}""></input-decimal>";
                            using (Div(style: "width: 100px")) Write(control);
                        }
                        else if (e is ManagerServer.Model.CheckboxCustomField)
                        {
                            using (Div(@class: "flex items-start gap-2 my-1"))
                            {
                                InputCheckbox(id: e.Key.ToString("N"), @class: "form-check-input", value: "true", v_model: $"{fieldName}.Booleans['{e.Key}']", disabled: e.LockedForManualEditing);
                                using (Div(@class: "w-full")) using (Label(@for: e.Key.ToString("N"))) Write(e.Name);
                            }
                        }
                        else if (e is ManagerServer.Model.MultipleValueCustomField multipleValueCustomField)
                        {
                            using (Label()) Write(e.Name);
                            var v_model = $"{fieldName}.StringArrays['{e.Key}']";
                            var options = multipleValueCustomField.Options?.Select(x => x.Value).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToArray();
                            using (Div()) VueSelect(v_model, options: options, true, true, e.LockedForManualEditing);
                        }
                        else if (e is ManagerServer.Model.ImageCustomField imageCustomField)
                        {
                            using (Label()) Write(e.Name);
                            var v_model = $"{fieldName}.Images['{e.Key}']";
                            using (Div(@class: "flex items-center"))
                            {
                                using (Div())
                                {
                                    if (!e.LockedForManualEditing)
                                    {
                                        InputFile(@class: "form-control", vShow: $"!{v_model}", name: v_model, accept: "image/*", hxPost: new Image() { Business = Business }.ToUrl(), hxTrigger: "change", hxEncoding: "multipart/form-data", hxTarget: "this", hxSwap: "afterend", hxDisabledElt: "this");
                                        InputHidden(id: v_model, v_model: v_model);
                                    }
                                    Write(@$"<img class=""form-control"" style=""width: {imageCustomField.GetWidth()}px; height: {imageCustomField.GetHeight()}px"" v-if=""{v_model}"" :src=""`{new Image() { Business = Business }.ToUrl()}&key=${{{v_model}}}`"" />");
                                }
                                if (!e.LockedForManualEditing)
                                {
                                    using (Div())
                                    {
                                        using (Button(onclick: $"app.{v_model} = null", @class: "p-4"))
                                        {
                                            I(@class: "fa-solid fa-trash text-neutral-400 hover:text-rose-600");
                                        }
                                    }
                                }
                            }
                        }
                        else if (e is ManagerServer.Model.TextCustomField textCustomField)
                        {
                            using (Label()) Write(e.Name);

                            var v_model = $"{fieldName}.Strings['{e.Key}']";

                            if (textCustomField.Type == ManagerServer.Model.Enums.TextCustomFieldType.ParagraphText)
                            {
                                var width = "300px";
                                if (textCustomField.Size == ManagerServer.Model.Enums.CustomFieldSize.Medium) width = "500px";
                                if (textCustomField.Size == ManagerServer.Model.Enums.CustomFieldSize.Large) width = "700px";
                                this.Textarea(v_model: v_model, @class: "form-control field-sizing-content resize", style: "width: " + width + "; height: 100px", @readonly: e.LockedForManualEditing);
                            }
                            else if (textCustomField.Type == ManagerServer.Model.Enums.TextCustomFieldType.DropdownList)
                            {
                                var options = (textCustomField.OptionsForDropdownList ?? string.Empty).Replace("\r\n", "\n").Replace('\r', '\n').Split('\n').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                                var options2 = Newtonsoft.Json.JsonConvert.SerializeObject(options).Replace(@"""", "'");
                                using (Div()) VueSelect(v_model, options: options, false, true, e.LockedForManualEditing);
                            }
                            else
                            {
                                var width = "100px";
                                if (textCustomField.Size == ManagerServer.Model.Enums.CustomFieldSize.Medium) width = "300px";
                                if (textCustomField.Size == ManagerServer.Model.Enums.CustomFieldSize.Large) width = "500px";
                                InputText(v_model: v_model, @class: "form-control", style: "width: " + width, @readonly: e.LockedForManualEditing);
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(e.Description))
                        {
                            var description = e.Description;
                            var links = e.Description.Split(' ').Where(x => x.ToLowerInvariant().StartsWith("http://") || x.ToLowerInvariant().StartsWith("https://")).ToArray();
                            foreach (var link in links)
                            {
                                description = description.Replace(link, @" &mdash; <a href=""" + link + @""" target=""_blank"">" + Strings.LearnMore + "</a>");
                            }
                            using (P(@class: "help-block")) Write(description);
                        }
                    }
                }
            }
        }

        private void VInputDate(string fieldName, bool clearable, string placeholder, bool disabled)
        {
            var datePattern = "";
            var groups = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern.ToLowerInvariant().Where(x => x == 'd' || x == 'm' || x == 'y').GroupBy(x => x);
            if (groups.Count() != 3)
            {
                datePattern = "yyyy-mm-dd";
            }
            else
            {
                var list = new List<string>();
                foreach (var e in groups)
                {
                    if (e.Key == 'y') list.Add("YYYY");
                    else if (e.Key == 'm') list.Add("M");
                    else if (e.Key == 'd') list.Add("D");
                }

                var dateSeparator = " ";
                var shortDatePattern = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;
                if (shortDatePattern.Contains('/')) dateSeparator = "/";
                else if (shortDatePattern.Contains('-')) dateSeparator = "-";
                else if (shortDatePattern.Contains('.')) dateSeparator = ".";

                datePattern = string.Join(dateSeparator, list.ToArray());
            }

            var weekStart = 1;
            if (ApplicationData.Businesses.Get(Business).Exists<ManagerServer.Model.DateAndNumberFormat>())
            {
                var regionFormats = ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.DateAndNumberFormat>();
                weekStart = (int)regionFormats.FirstDayOfWeek;
            }

            Write(@"<date-picker v-model=""" + fieldName + @""" :shortcuts=""[{ text: 'today', onClick: () => new window.Date() }]"" :lang=""{ formatLocale: { firstDayOfWeek: " + (int)System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.FirstDayOfWeek + @" } }"" partial-update=""true"" type=""date"" value-type=""YYYY-M-D"" format=""" + datePattern + @""" :clearable=""" + clearable.ToString().ToLowerInvariant() + @""" placeholder=""" + placeholder + @""" :disabled="""+disabled.ToString().ToLowerInvariant()+@""">");
            Write(@"<template v-slot:footer=""{ emit }"">");
            using (Button(@class: "btn btn-block btn-sm", onclick: "this.parentElement.parentElement.parentElement.getElementsByClassName('mx-btn-shortcut')[0].click()")) Write(Strings.Today);
            // This won't hide the button on click so we use shortcut click trigger - we hide shortcut box using CSS
            // Write(@"<button class=""btn btn-block btn-sm"" @=""emit(new window.Date())"">" + Strings.Today + "</button>");
            Write("</template>");
            Write("</date-picker>");
        }

        private void VInputDate(string prefix, MemberInfo field)
        {
            var clearable = false;
            if (field.GetMemberType() == typeof(DateTime?)) clearable = true;

            var placeholder = field.GetCustomAttribute<PlaceholderAttribute>()?.ToString();

            var prepend = field.GetCustomAttribute<PrependAttribute>();
            var append = field.GetCustomAttribute<AppendAttribute>();
            if (prepend != null || append != null)
            {
                using (Div(@class: "input-group"))
                {
                    if (prepend != null) using (Span(@class: "input-group-text input-sm")) Write(prepend.ToString());
                    VInputDate(prefix+field.Name, clearable, placeholder, false);
                    if (append != null) using (Span(@class: "input-group-text input-sm")) Write(append.ToString());
                }
            }
            else
            {
                VInputDate(prefix + field.Name, clearable, placeholder, false);
            }
        }

        private void VueSelect(string vModel, Tuple<Guid, string>[] options, bool multiple, bool autoWidth, bool disabled)
        {
            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb))
            using (var writer = new Newtonsoft.Json.JsonTextWriter(sw))
            {
                writer.StringEscapeHandling = Newtonsoft.Json.StringEscapeHandling.EscapeHtml;
                writer.QuoteChar = '\'';

                new Newtonsoft.Json.JsonSerializer().Serialize(writer, (options ?? new Tuple<Guid, string>[0]).Select(x => new { Key = x.Item1.ToString(), UniqueName = x.Item2 }).ToArray());
            }

            VueSelect(vModel, sb.ToString(), multiple, autoWidth, disabled, "UniqueName");
        }

        private void VueSelect(string vModel, string[] options, bool multiple, bool autoWidth, bool disabled)
        {
            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb))
            using (var writer = new Newtonsoft.Json.JsonTextWriter(sw))
            {
                writer.StringEscapeHandling = Newtonsoft.Json.StringEscapeHandling.EscapeHtml;
                writer.QuoteChar = '\'';

                new Newtonsoft.Json.JsonSerializer().Serialize(writer, (options ?? new string[0]).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToArray());
            }

            VueSelect(vModel, sb.ToString(), multiple, autoWidth, disabled, null);
        }

        private void VueSelect(string vModel, string options, bool multiple, bool autoWidth, bool disabled, string label)
        {
            var dir = ManagerServer.Globalization.Languages.IsRightToLeft() ? "rtl" : "ltr";
            Write($@"<v-select {(multiple ? "multiple" : string.Empty)} v-model=""{vModel}"" label=""{label}"" :options=""{options}"" transition=""none"" :disabled=""{disabled.ToString().ToLowerInvariant()}"" dir=""{dir}"" {(autoWidth ? @"style=""display: block; width: auto; min-width: 150px""" : null)}>");
            Write(@"<div slot=""no-options"">");
            Write(Strings.NoMatchesFound);
            Write("</div>");
            Write("</v-select>");
        }

        private void VSelectTimeFormat(MemberInfo field)
        {
            using (Div()) using (Select(style: "width: 150px", @class: "form-select", v_model: field.Name))
            {
                var timeFormats = new List<string>();
                timeFormats.Add("h:mm:ss tt");
                timeFormats.Add("hh:mm:ss tt");
                timeFormats.Add("H:mm:ss");
                timeFormats.Add("HH:mm:ss");

                var currentTimeFormat = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.LongTimePattern;

                if (timeFormats.All(x => x != currentTimeFormat))
                {
                    var date = new DateTime(DateTime.Today.Year, 4, 5);
                    var example = date.ToString(currentTimeFormat);
                    Option(value: currentTimeFormat, text: example);
                }
                foreach (var e in timeFormats.OrderBy(x => x))
                {
                    var date = new DateTime(DateTime.Today.Year, 4, 5, 9, 40, 7);
                    var example = date.ToString(e);
                    Option(value: e, text: example);
                }
            }
        }

        private void VSelectCountry(MemberInfo field)
        {
            using (Div()) using (Select(style: "width: 300px", @class: "form-select", v_model: field.Name))
            {
                Option();
                var currentLanguage = Strings.CurrentLanguage.Value;
                if (!string.IsNullOrWhiteSpace(currentLanguage)) currentLanguage = currentLanguage.Split('-').First();
                foreach (var e in ManagerServer.Localizations.Localizations.Json.GroupBy(x => Locales.GetLanguage(x.Key)).OrderByDescending(x => x.Key == currentLanguage).ThenBy(x => Languages.GetLanguageNativeName(x.Key)))
                {
                    using (OptGroup(label: Languages.GetLanguageNativeName(e.Key)))
                    {
                        foreach (var e2 in e.OrderBy(x => Locales.GetNativeName(x.Key)))
                        {
                            Option(value: e2.Key, text: Locales.GetNativeName(e2.Key));
                        }
                    }
                }                
            }
        }

        private void VSelectDateFormat(MemberInfo field)
        {
            using (Div()) using (Select(style: "width: 150px", @class: "form-select", v_model: field.Name))
            {
                var dateFormats = new List<string>();
                dateFormats.Add("dd.MM.yy");
                dateFormats.Add("dd.MM.yy 'ý.'");
                dateFormats.Add("dd.MM.yyyy");
                dateFormats.Add("dd.MM.yyyy.");
                dateFormats.Add("dd/MM/yy");
                dateFormats.Add("dd/MM/yyyy");
                dateFormats.Add("dd-MM-yy");
                dateFormats.Add("dd-MM-yyyy");
                dateFormats.Add("MM/dd/yyyy");
                dateFormats.Add("yyyy.MM.dd");
                dateFormats.Add("yyyy/MM/dd");
                dateFormats.Add("yyyy-MM-dd");

                var currentDateFormat = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;

                if (dateFormats.All(x => x != currentDateFormat))
                {
                    var date = new DateTime(DateTime.Today.Year, 12, 31);
                    var example = date.ToString(currentDateFormat);
                    Option(value: currentDateFormat, text: example);
                }
                foreach (var e in dateFormats.OrderBy(x => x))
                {
                    var date = new DateTime(DateTime.Today.Year, 12, 31);
                    var example = date.ToString(e);
                    Option(value: e, text: example);
                }
            }
        }

        private void VSelectNumberFormat(MemberInfo field)
        {
            using (Div()) using (Select(style: "width: 150px", @class: "form-select", v_model: field.Name))
            {
                var current = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat;
                var formats = Newtonsoft.Json.JsonConvert.DeserializeObject<ManagerServer.Model.DateAndNumberFormat.NumberFormatParts[]>(System.IO.File.ReadAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Formats.json")));
                if (Languages.IsRightToLeft())
                {
                    formats = formats.Where(x => x.GroupSeparator != " ").ToArray();
                }

                if (!formats.Any(x => x.DecimalSeparator == current.NumberDecimalSeparator && x.GroupSeparator == current.NumberGroupSeparator && x.GroupSizes.SequenceEqual(current.NumberGroupSizes)))
                {
                    var example = (123456789.00m).ToString("N2", current);
                    Option(value: string.Empty, text: example, selected: true);
                }
                foreach (var e in formats.OrderBy(x => x.GroupSizes.Length).ThenBy(x => x.GroupSeparator).ThenBy(x => x.DecimalSeparator))
                {
                    using (var ms = new System.IO.MemoryStream())
                    {
                        ProtoBuf.Serializer.Serialize<ManagerServer.Model.DateAndNumberFormat.NumberFormatParts>(ms, e);

                        var numberFormat = new System.Globalization.NumberFormatInfo();
                        numberFormat.NumberDecimalSeparator = e.DecimalSeparator;
                        numberFormat.NumberGroupSeparator = e.GroupSeparator;
                        numberFormat.NumberGroupSizes = e.GroupSizes;
                        var example = (123456789.00m).ToString("N2", numberFormat);
                        Option(value: Convert.ToBase64String(ms.ToArray()), text: example);
                    }
                }
            }
        }

        private bool IsVisible(MemberInfo field)
        {
            //if (field.GetCustomAttribute<DoNotHideAttribute>() != null) return true;

            if (field.Name.StartsWith("Obsolete_"))
            {
                return false;
            }
            if (field.GetCustomAttribute<IfNotEnglishAttribute>() != null)
            {
                var isEnglish = ManagerServer.Globalization.Languages.GetLanguage() == "en";
                if (isEnglish) return false;
            }
            if (field.GetCustomAttribute<HiddenAttribute>() != null)
            {
                return false;
            }
            if (field.GetCustomAttribute<IfContainsAttribute>() != null)
            {
                var ifContainsAttribute = field.GetCustomAttribute<IfContainsAttribute>();
                var contains = ifContainsAttribute.Contains(ApplicationData.Businesses.Get(Business));
                if (!contains) return false;
            }
            if (field.GetMemberType() == typeof(Guid?))
            {
                var autocompleteAttribute = field.GetCustomAttribute<ManagerServer.Model.Attributes.AutocompleteAttribute>();
                if (autocompleteAttribute != null)
                {
                    if (autocompleteAttribute.Value == typeof(ManagerServer.Model.ISaleItem)) return ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.InventoryItem>().Any() || ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.NonInventoryItem>().Any() || ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.InventoryKit>().Any();
                    if (autocompleteAttribute.Value == typeof(ManagerServer.Model.IPurchaseItem)) return ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.InventoryItem>().Any() || ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.NonInventoryItem>().Any();
                    if (autocompleteAttribute.Value == typeof(ManagerServer.Model.SalesOrder)) return ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.SalesOrder>().Any();
                    if (autocompleteAttribute.Value == typeof(ManagerServer.Model.SalesQuote)) return ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.SalesQuote>().Any();
                    if (autocompleteAttribute.Value == typeof(ManagerServer.Model.TaxCode)) return ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.TaxCode>().Any();
                    if (autocompleteAttribute.Value == typeof(ManagerServer.Model.Division)) return ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.Division>().Any();
                    if (autocompleteAttribute.Value == typeof(ManagerServer.Model.Project)) return ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.Project>().Any();
                    if (autocompleteAttribute.Value == typeof(ManagerServer.Model.CustomInventoryLocation)) return ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.CustomInventoryLocation>().Any();                    
                    if (autocompleteAttribute.Value == typeof(ManagerServer.Model.CashFlowStatementFinancingActivityGroup)) return ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.CashFlowStatementFinancingActivityGroup>().Any();
                    if (autocompleteAttribute.Value == typeof(ManagerServer.Model.CashFlowStatementInvestingActivityGroup)) return ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.CashFlowStatementInvestingActivityGroup>().Any();
                    if (autocompleteAttribute.Value == typeof(ManagerServer.Model.CashFlowStatementOperatingActivityGroup)) return ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.CashFlowStatementOperatingActivityGroup>().Any();
                    if (autocompleteAttribute.Value == typeof(ManagerServer.Model.ForeignCurrency)) return ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.ForeignCurrency>().Any();
                    if (autocompleteAttribute.Value == typeof(ManagerServer.Model.ControlAccountForBankAccounts)) return ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.ControlAccountForBankAccounts>().Any();
                    if (autocompleteAttribute.Value == typeof(ManagerServer.Model.ControlAccountForInvestments)) return ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.ControlAccountForInvestments>().Any();
                    if (autocompleteAttribute.Value == typeof(ManagerServer.Model.ControlAccountForCapitalAccounts)) return ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.ControlAccountForCapitalAccounts>().Any();
                    if (autocompleteAttribute.Value == typeof(ManagerServer.Model.ControlAccountForCustomers)) return ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.ControlAccountForCustomers>().Any();
                    if (autocompleteAttribute.Value == typeof(ManagerServer.Model.ControlAccountForEmployees)) return ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.ControlAccountForEmployees>().Any();
                    if (autocompleteAttribute.Value == typeof(ManagerServer.Model.ControlAccountForFixedAssets)) return ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.ControlAccountForFixedAssets>().Any();
                    if (autocompleteAttribute.Value == typeof(ManagerServer.Model.ControlAccountForFixedAssetsAccumulatedDepreciation)) return ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.ControlAccountForFixedAssetsAccumulatedDepreciation>().Any();
                    if (autocompleteAttribute.Value == typeof(ManagerServer.Model.ControlAccountForIntangibleAssets)) return ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.ControlAccountForIntangibleAssets>().Any();
                    if (autocompleteAttribute.Value == typeof(ManagerServer.Model.ControlAccountForIntangibleAssetsAccumulatedAmortization)) return ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.ControlAccountForIntangibleAssetsAccumulatedAmortization>().Any();
                    if (autocompleteAttribute.Value == typeof(ManagerServer.Model.ControlAccountForInventoryItems)) return ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.ControlAccountForInventoryItems>().Any();
                    if (autocompleteAttribute.Value == typeof(ManagerServer.Model.ControlAccountForSpecialAccounts)) return ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.ControlAccountForSpecialAccounts>().Any();
                    if (autocompleteAttribute.Value == typeof(ManagerServer.Model.ControlAccountForSuppliers)) return ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.ControlAccountForSuppliers>().Any();
                    if (autocompleteAttribute.Value == typeof(ManagerServer.Model.CustomTheme))
                    {
                        return ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.CustomTheme>().Any();
                    }                   
                }
            }
            return true;
        }
    }
}