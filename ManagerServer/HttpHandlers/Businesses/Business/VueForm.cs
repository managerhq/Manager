using ManagerServer;
using ManagerServer.Api.Businesses.Business;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Model;
using ManagerServer.Orm;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    internal abstract class VueForm : Form
    {
        [InheritedProtoMember(251)] public string DeleteReferrer;
    }

    internal abstract class VueForm<T> : VueForm where T : ManagerServer.Model.Object, new()
    {
        [InheritedProtoMember(250)] public bool ConfirmDelete;        

        protected virtual void InnerGet3() { }
        
        protected virtual void InnerGet4() { }

        protected virtual bool CanHaveImage() => false;

        protected virtual void OnSource(T form, ManagerServer.Model.Object source) { }        

        protected sealed override void InnerGet2()
        {
            using (Script())
            {
                Write(@"function updateCustomField(type, target, source, key) {
    // Check if the target property is undefined or an empty string
    if (target && target[type] && (typeof target[type][key] === 'undefined' || target[type][key] === '' || target[type][key] === null)) {
        if (source && source[type]) {
            app.$set(target[type], key, source[type][key]);
        }
    }
}");
            }

            if (ConfirmDelete)
            {
                Dictionary<Type, int> references = new();
                if (typeof(T).IsSubclassOf(typeof(NamedObject)))
                {
                    var database = ApplicationData.Businesses.Get(Business);                    
                    references = database.GetGeneralLedgerTransactions()
                        .GetTransactionsByForeignKey(Key.Value)
                        .Select(x => database.SingleOrDefault(x))
                        .Where(x => x != null)
                        .GroupBy(x => x.GetType())
                        .ToDictionary(x => x.Key, x => x.Count());
                }

                if (references.Any())
                {
                    using (Div(@class: "card"))
                    {
                        using (Div(@class: "card-header"))
                        {
                            using (Div(@class: "flex justify-between"))
                            {
                                using (Div(@class: "card-title")) Write(GetTitle());
                                using (Div(@class: "text-xs opacity-25")) Write(Key.ToString());
                            }
                        }

                        using (Div(@class: "card-header"))
                        {
                            Write(Strings.TheFormCannotBeDeleted);
                        }
                    }

                    var nakedTables = typeof(Program).Assembly.GetTypes().Where(x => x.BaseType != null).Where(x => x.BaseType.IsGenericType).Where(x => x.BaseType.GetGenericTypeDefinition() == typeof(NakedObjectsWithAutomaticRows<>)).ToArray();

                    using (Div(@class: "flex flex-wrap gap-8 p-8"))
                    {
                        foreach (var e in references)
                        {
                            var nakedTableType = nakedTables.FirstOrDefault(x => x.BaseType.GetGenericArguments()[0] == e.Key);
                            if (nakedTableType != null)
                            {
                                var nakedTable = Activator.CreateInstance(nakedTableType) as BusinessTemplate;
                                nakedTable.HttpContext = HttpContext;
                                nakedTable.Business = Business;
                                nakedTable.Referrer = this.ToUrl();
                                nakedTableType.GetFieldOrProperty("Reference").SetMemberValue(nakedTable, Key.Value);

                                using (A(href: nakedTable.ToUrl(), @class: $"basis-72 flex items-center gap-4 p-4 hover:no-underline hover:bg-neutral-100 hover:rounded-xl"))
                                {
                                    using (Span(@class: "whitespace-nowrap border py-0.5 px-2 bg-white rounded-lg tabular-nums observer:blur-sm observer:hover:blur-none observer:hover:transition text-neutral-500 border-neutral-300")) Write(e.Value.ToString());
                                    I(@class: "text-neutral-400 fas fa-fw " + Icons.GetIcon(nakedTableType.Name), style: "font-size: 32px");
                                    using (Span()) Write(Strings.GetPropertyValue(nakedTableType.Name));
                                }
                            }
                            else
                            {
                                Write(e.Key.Name);
                                Br();
                            }
                        }
                    }

                    return;
                }
            }

            T o = null;
            if (Key.HasValue)
            {
                o = ApplicationData.Businesses.Get(Business).SingleOrDefault<T>(Key.Value);
                if (o == null)
                {
                    var o2 = ApplicationData.Businesses.Get(Business).Single<T>();
                    if (o2.Key == Key.Value) o = o2;
                }
            }
            if (Clone.HasValue)
            {
                o = ProtoBuf.Serializer.DeepClone<T>(ApplicationData.Businesses.Get(Business).SingleOrDefault<T>(Clone.Value));

                if (o != null)
                {
                    var referenceField = typeof(T).GetFieldOrProperty("Reference");
                    if (referenceField != null && referenceField.GetMemberType() == typeof(string)) referenceField.SetMemberValue(o, null);
                    var formDefault = ApplicationData.Businesses.Get(Business).Single<T>();
                    var automaticReferenceField = typeof(T).GetFieldOrProperty("AutomaticReference");
                    if (automaticReferenceField != null && automaticReferenceField.GetMemberType() == typeof(bool))
                    {
                        automaticReferenceField.SetMemberValue(o, automaticReferenceField.GetMemberValue(formDefault));
                    }
                }
            }
            if (Data2 != null)
            {
                o = ProtoBuf.Serializer.Deserialize<T>(new System.IO.MemoryStream(Data2));
            }
            if (Request.HasFormContentType)
            {
                var form = Request.ReadFormAsync().GetAwaiter().GetResult();
                var value = form["Form"].ToString();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    o = ProtoBuf.Serializer.Deserialize<T>(new System.IO.MemoryStream(Convert.FromBase64String(value)));
                }
            }
            if (o == null)
            {
                var formDefault = ApplicationData.Businesses.Get(Business).Single<T>();
                foreach (var e in typeof(T).GetFieldsAndProperties().Where(x => x.GetMemberType() == typeof(DateTime)))
                {
                    e.SetMemberValue(formDefault, default(DateTime));
                }
                o = ProtoBuf.Serializer.DeepClone<T>(formDefault);
            }

            ManagerServer.Model.Object source = null;
            if (Source.HasValue)
            {
                source = ApplicationData.Businesses.Get(Business).SingleOrDefault(Source.Value);
                if (source == null)
                {
                    source = ApplicationData.Businesses.Get(Business).GetReportTransformation2(Source.Value);
                }
            }
            OnSource(o, source);

            if (Source.HasValue || Clone.HasValue)
            {
                // Make sure only custom fields not excluded from copying or cloning are actually copied or cloned
                if (o is ICustomFields customFields)
                {
                    if (customFields.CustomFields != null)
                    {
                        var keys = ApplicationData.Businesses.Get(Business).GetCustomFields(o.GetType()).Where(x => !x.ExcludeFromCopyingOrCloning).Select(x => x.Key).ToArray();
                        customFields.CustomFields.StripValues(keys);
                    }
                }
            }

            using (Div(id: "v-model-form"))
            {
                using (Div(@class: "card"))
                {
                    using (Div(@class: "card-header"))
                    {
                        using (Div(@class: "flex justify-between"))
                        {
                            using (Div(@class: "flex items-center gap-3"))
                            {
                                using (Div(@class: "card-title"))
                                {
                                    var titleAttribute = this.GetType().GetCustomAttribute<TitleAttribute>();
                                    var title = titleAttribute?.Text?.FirstOrDefault() ?? typeof(T).Name;
                                    Write(ManagerServer.Globalization.Strings.GetPropertyValue(title));
                                }
                                var bounce = false;
                                if (this is TabsForm)
                                {
                                    if (!ApplicationData.Businesses.Get(Business).Exists<ManagerServer.Model.Tabs>())
                                    {
                                        bounce = true;
                                    }
                                }
                                WriteHelp(bounce);
                            }
                            using (Div(@class: "text-xs opacity-25")) Write(Key.ToString());
                        }
                    }

                    if (ConfirmDelete)
                    {
                        using (Div(@class: "card-header flex gap-2 items-center"))
                        {
                            using (Div(@class: "font-semibold")) Write(Strings.Are_you_sure);
                            using (Form(method: "POST", action: new FormDelete() { Business = Business, Key = Key.Value, Referrer = DeleteReferrer ?? Referrer }.ToUrl(), hxBoost: true, hxDisabledElt: "find button"))
                            {
                                using (Button(@class: "btn btn-danger"))
                                {
                                    if (Key == ManagerServer.Model.Object.GetGuidByType(typeof(T)))
                                    {
                                        Write(Strings.Reset);
                                    }
                                    else
                                    {
                                        Write(Strings.Delete);
                                    }
                                    I(@class: "htmx-indicator ms-2 fas fa-circle-notch fa-spin !hidden");
                                }
                            }
                            if (!string.IsNullOrWhiteSpace(Referrer))
                            {
                                using (DefaultLink(Referrer)) Write(Strings.Cancel);
                            }
                        }
                    }

                    using (Div(@class: "card-form"))
                    {
                        using (Style()) Write("[v-cloak] { display: none; }");
                        using (Div(v_cloak: true))
                        {
                            InnerGet3();
                            InnerGet4();

                            if (CanHaveImage())
                            {
                                using (Script())
                                {
                                    Write(@"function previewImage() {
  var preview = document.querySelector('#display-image');
  var file = document.querySelector('#image-input').files[0];
  var reader = new FileReader();
  reader.onloadend = function () { preview.src = reader.result; document.querySelector('#image-input').classList.add('hidden'); document.querySelector('#removeImageButton').classList.remove('hidden'); }
  if (file) reader.readAsDataURL(file);
}");
                                    Write(@"function removeImage() {
document.querySelector('#ImageDeleted').value = 'true';
document.querySelector('#display-image').src = '';
document.querySelector('#image-input').value = '';
document.querySelector('#image-input').classList.remove('hidden');
document.querySelector('#removeImageButton').classList.add('hidden');
}");
                                }

                                using (Fieldset())
                                {
                                    using (Legend()) Write(Strings.Image);

                                    var dataUrl = Key.HasValue ? ApplicationData.Businesses.GetImageDataUrl(Business, Key.Value) : null;
                                    using (Div(id: "removeImageButton", @class: "text-end p-2" + (string.IsNullOrWhiteSpace(dataUrl) ? " hidden" : null))) using (Button(onclick: "removeImage()")) I(@class: "fa-solid fa-trash text-neutral-400 hover:text-rose-600");
                                    Img(id: "display-image", @class: "border rounded", src: dataUrl);

                                    InputFile(id: "image-input", onchange: "previewImage()", accept: "image/jpeg, image/png, image/jpg", @class: (!string.IsNullOrWhiteSpace(dataUrl) ? "hidden" : "form-file"));
                                    InputHidden(id: "ImageDeleted", value: "false");
                                }
                            }
                        }
                    }

                    if (!ConfirmDelete)
                    {
                        using (Div(@class: "card-header flex gap-2 items-center"))
                        {
                            var userPermissions = GetCurrentUserPermissions(Business);

                            if (!Key.HasValue)
                            {
                                if (userPermissions.CanCreate(this.GetType().Namespace))
                                {
                                    using (Form(method: "POST", action: this.ToUrl(), hxBoost: true, hxDisabledElt: "find button", enctype: HttpFramework.Enctype.multipartformdata))
                                    {
                                        InputHidden(name: "febb4049-dcdb-4c7a-a395-4b71da72a85b", value: "{}");
                                        using (Button(@class: "btn btn-primary"))
                                        {
                                            Write(Strings.Create);
                                            I(@class: "htmx-indicator ms-2 fas fa-circle-notch fa-spin !hidden");
                                        }
                                    }

                                    if (Data2 == null)
                                    {
                                        if (!this.GetType().FullName.StartsWith("ManagerServer.HttpHandlers.Businesses.Business.Reports."))
                                        {
                                            using (Form(method: "POST", action: this.ToUrl(), hxBoost: true, hxDisabledElt: "find button", enctype: HttpFramework.Enctype.multipartformdata))
                                            {
                                                InputHidden(name: "febb4049-dcdb-4c7a-a395-4b71da72a85b", value: "{}");
                                                InputHidden(name: nameof(Strings.CreateAndAddAnother), value: "true");
                                                using (Button(@class: "btn btn-outline"))
                                                {
                                                    Write(Strings.CreateAndAddAnother);
                                                    I(@class: "htmx-indicator ms-2 fas fa-circle-notch fa-spin !hidden");
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    using (Button(@class: "btn btn-primary", disabled: true)) Write(Strings.Create);
                                    using (Span()) Write(@"Administrator has disabled ""Create"" button");
                                }
                            }
                            else
                            {
                                if (userPermissions.CanUpdate(this.GetType().Namespace))
                                {
                                    using (Form(method: "POST", action: this.ToUrl(), hxBoost: true, hxDisabledElt: "find button", enctype: HttpFramework.Enctype.multipartformdata))
                                    {
                                        InputHidden(name: "febb4049-dcdb-4c7a-a395-4b71da72a85b", value: "{}");
                                        using (Button(@class: "btn btn-success"))
                                        {
                                            Write(Strings.Update);
                                            I(@class: "htmx-indicator ms-2 fas fa-circle-notch fa-spin !hidden");
                                        }
                                    }
                                }
                                else
                                {
                                    using (Button(@class: "btn btn-success", disabled: true)) Write(Strings.Update);
                                }

                                var deleteButton = true;

                                if (userPermissions.CanDelete(this.GetType().Namespace))
                                {
                                    var lockDate = ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.LockDate>();
                                    if (lockDate.GetLockDate().HasValue)
                                    {
                                        if (o is ManagerServer.Model.Transaction transaction)
                                        {
                                            if (transaction.GetGeneralLedgerTransactions(ApplicationData.Businesses.Get(Business)).Any(x => lockDate.IsLocked(x.Date)))
                                            {
                                                deleteButton = false;
                                                using (Span()) Write("No transaction dated before lock date (" + lockDate.GetLockDate().Value.ToLocalShortDisplayString() + @") can be deleted.");
                                            }
                                        }
                                        if (o is ManagerServer.Model.ExchangeRate exchangeRate)
                                        {
                                            if (lockDate.IsLocked(exchangeRate.Date))
                                            {
                                                deleteButton = false;
                                                using (Span()) Write("No exchange rate dated before lock date (" + lockDate.GetLockDate().Value.ToLocalShortDisplayString() + @") can be deleted.");
                                            }
                                        }
                                        if (o is ManagerServer.Model.InventoryUnitCost inventoryUnitCost)
                                        {
                                            if (lockDate.IsLocked(inventoryUnitCost.Date))
                                            {
                                                deleteButton = false;
                                                using (Span()) Write("No exchange rate dated before lock date (" + lockDate.GetLockDate().Value.ToLocalShortDisplayString() + @") can be deleted.");
                                            }
                                        }
                                    }

                                    if (deleteButton)
                                    {
                                        var confirmDeleteHandler = (VueForm<T>)this.MemberwiseClone();
                                        confirmDeleteHandler.ConfirmDelete = true;
                                        using (A(href: confirmDeleteHandler.ToUrl(), @class: "btn btn-danger"))
                                        {
                                            if (Key == ManagerServer.Model.Object.GetGuidByType(typeof(T)))
                                            {
                                                Write(Strings.Reset);
                                            }
                                            else
                                            {
                                                Write(Strings.Delete);
                                            }
                                        }
                                    }
                                }

                                if (!userPermissions.CanUpdate(this.GetType().Namespace) && !userPermissions.CanDelete(this.GetType().Namespace))
                                {
                                    using (Span()) Write(@"Administrator has disabled ""Update"" and ""Delete"" buttons");
                                }
                                else if (!userPermissions.CanDelete(this.GetType().Namespace))
                                {
                                    using (Span()) Write(@"Administrator has disabled ""Delete"" button");
                                }
                            }
                        }
                    }
                }

#if DEBUG
                using (Details(@class: "card mt-8"))
                {
                    using (Summary(@class: "card-header flex cursor-pointer"))
                    {
                        using (Div(@class: "card-title")) Write("Data");
                    }
                    using (Div(@class: "card-form"))
                    {
                        using (Pre())
                        {
                            Write("{{ JSON.stringify($data, null, 2) }}");
                        }
                    }
                }
#endif
            }

            Script("resources/vue/vue.js?version=" + typeof(Template).Assembly.GetName().Version.ToString());
            Script("resources/sortable/sortable.js?version=" + typeof(Template).Assembly.GetName().Version.ToString()); // Dependency for VueDraggable
            Script("resources/vuedraggable/vuedraggable.js?version=" + typeof(Template).Assembly.GetName().Version.ToString()); // Required for reordering rows
            Script("resources/vueselect/vue-select.js?version=" + typeof(Template).Assembly.GetName().Version.ToString()); // Eventually remove
            Script("resources/datepicker/date-picker.js?version=" + typeof(Template).Assembly.GetName().Version.ToString()); // Date pickers
            Script("resources/jquery/jquery-1-8-2-min.js?version=" + typeof(Template).Assembly.GetName().Version.ToString()); // Required for Select2
            Script("resources/decimal/decimal.js?version=" + typeof(Template).Assembly.GetName().Version.ToString()); // Decimal arithmetic
            Script("resources/select2/select2.js?version=" + typeof(Template).Assembly.GetName().Version.ToString()); // Autocomplete dropboxes
            Script("resources/select2vue/select2vue.js?version=" + typeof(Template).Assembly.GetName().Version.ToString()); // Required for Select2 + Vue
            Script("resources/mathexpressionevaluator/math-expression-evaluator-min.js?version=" + typeof(Template).Assembly.GetName().Version.ToString()); // Required for in-field expressions (e.g. 5*5)

            var expand = new List<string>();
            foreach (var e in typeof(T).GetFieldsAndProperties())
            {
                if (e.Name.StartsWith("Obsolete_")) continue;
                if (e.GetMemberType() == typeof(Guid?)) expand.Add(e.Name);
                if (e.GetMemberType() == typeof(Guid[])) expand.Add(e.Name);
                if (e.GetMemberType().IsArray)
                {
                    foreach (var e2 in e.GetMemberType().GetElementType().GetFieldsAndProperties())
                    {
                        if (e2.GetMemberType() == typeof(Guid?)) expand.Add(e.Name+"."+e2.Name);
                        if (e2.GetMemberType() == typeof(Guid[])) expand.Add(e.Name + "." + e2.Name);

                        var substituteAttribute = e2.GetCustomAttribute<ManagerServer.Model.Attributes.SubstituteAttribute>();
                        if (substituteAttribute != null)
                        {
                            expand.Add(e.Name + "." + string.Join('.', substituteAttribute.Path));
                        }
                    }
                }
            }

            var businessDetails = ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.BusinessDetails>();
            var localizationObjects = new List<NamedObject>();
            localizationObjects.AddRange(ManagerServer.Localizations.Localizations.Get(businessDetails.Obsolete_Country).OfType<ManagerServer.Model.ReportTransformation2>());
            localizationObjects.AddRange(ManagerServer.Localizations.Localizations.Get(businessDetails.Obsolete_Country).OfType<ManagerServer.Model.TaxCodeReportingCategory>());
            localizationObjects.AddRange(ManagerServer.Localizations.Localizations.Get(businessDetails.Obsolete_Country).OfType<ManagerServer.Model.TaxAmountReportingCategory>());
            localizationObjects.AddRange(ManagerServer.Localizations.Localizations.Get(businessDetails.Obsolete_Country).OfType<ManagerServer.Model.TaxAmountReversedReportingCategory>());
            localizationObjects.AddRange(ManagerServer.Localizations.Localizations.Get(businessDetails.Obsolete_Country).OfType<ManagerServer.Model.PayslipEarningsItemReportingCategory>());
            localizationObjects.AddRange(ManagerServer.Localizations.Localizations.Get(businessDetails.Obsolete_Country).OfType<ManagerServer.Model.PayslipDeductionItemReportingCategory>());
            localizationObjects.AddRange(ManagerServer.Localizations.Localizations.Get(businessDetails.Obsolete_Country).OfType<ManagerServer.Model.PayslipContributionItemReportingCategory>());

            var jsonSettings = new Newtonsoft.Json.JsonSerializerSettings();
            jsonSettings.ContractResolver = new CustomContractResolver(skipKey: true);
            jsonSettings.Converters.Add(new GuidSerializer(ApplicationData.Businesses.Get(Business), localizationObjects.ToDictionary(x => x.Key), expand.ToArray()));
            jsonSettings.Converters.Add(new DateTimeConverter());
            jsonSettings.Converters.Add(new MemberInfoSerializer());
            jsonSettings.Formatting = Formatting.Indented;
            jsonSettings.StringEscapeHandling = StringEscapeHandling.EscapeHtml;

            var sb = new StringBuilder();
            using (var writer = new StringWriter(sb))
            {
                var serializer = JsonSerializer.Create(jsonSettings);
                serializer.Serialize(writer, o);
            }
            
            var methods = new List<string>();

            var currencyAttribute = typeof(T).GetCustomAttribute<ManagerServer.Model.Attributes.CurrencyAttribute>();
            if (currencyAttribute != null)
            {
                methods.Add("getForeignCurrencyKey: function() { return " + currencyAttribute.GetForeignCurrencyKeyExpression() + "; }");
                methods.Add("getCurrencyCode: function() { return "+ currencyAttribute.GetCodeExpression() + "; }");
                methods.Add("getCurrencyExchangeRate: function() { return " + currencyAttribute.GetExchangeRateExpression() + "; }");
                methods.Add("getCurrencyExchangeRateIsInverse: function() { return " + currencyAttribute.GetExchangeRateIsInverseExpression() + "; }");
                methods.Add("getCurrencyDecimalPlaces: function() { return " + currencyAttribute.GetDecimalPlacesExpression() + "; }");

                methods.Add(@"getAccountCurrency: function(lineItem) {
var foreignCurrency = null;
if (lineItem.Account != null && lineItem.Account.IsAccountsReceivable && this.getAccountsReceivableCustomer(lineItem) != null) foreignCurrency = this.getAccountsReceivableCustomer(lineItem).Currency;
else if (lineItem.Account != null && lineItem.Account.IsAccountsPayable && this.getAccountsPayableSupplier(lineItem) != null) foreignCurrency = this.getAccountsPayableSupplier(lineItem).Currency;
else if (lineItem.Account != null && lineItem.Account.IsEmployeeClearingAccount && lineItem.Employee != null) foreignCurrency = lineItem.Employee.Currency;
else if (lineItem.Account != null && lineItem.Account.IsCashAtBank && lineItem.BankOrCashAccount != null) foreignCurrency = lineItem.BankOrCashAccount.Currency;
else if (lineItem.Account != null && lineItem.Account.IsControlAccountForSpecialAccounts && lineItem.SpecialAccount != null) foreignCurrency = lineItem.SpecialAccount.Currency;
else return null;
return (foreignCurrency in foreignCurrencies ? foreignCurrency : null); }".Replace(Environment.NewLine, string.Empty));
            }
            methods.Add(@"getForeignCurrencyOrBaseCurrencyCode: function(key) {
if (key == null) return baseCurrency.code;
return foreignCurrencies[key].code;
}");

            foreach (var e in typeof(T).GetFieldsAndProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (e.Name.StartsWith("Obsolete_")) continue;
                if (e.GetMemberType().IsArray && e.GetMemberType().GetElementType().GetConstructor(Type.EmptyTypes) != null)
                {
                    var ifExpression2 = string.Join(" && ", e.GetCustomAttributes<ManagerServer.Model.Attributes.IfAttribute>().Select(x => x.GetIfExpression()));
                    if (string.IsNullOrWhiteSpace(ifExpression2)) ifExpression2 = "true";
                    methods.Add($"getIf{e.Name}: function() {{ return {ifExpression2}; }}");
                    methods.Add("addTo" + e.Name + ": function() { this." + e.Name + ".push(" + Newtonsoft.Json.JsonConvert.SerializeObject(Activator.CreateInstance(e.GetMemberType().GetElementType()), jsonSettings) + ") }");

                    foreach (var e2 in e.GetMemberType().GetElementType().GetFieldsAndProperties(BindingFlags.Instance | BindingFlags.Public))
                    {
                        var ifJoin = "&&";
                        if (e2.GetCustomAttributes<ManagerServer.Model.Attributes.OrAttribute>().Any()) ifJoin = "||";
                        var ifExpression = string.Join($" {ifJoin} ", e2.GetCustomAttributes<ManagerServer.Model.Attributes.IfAttribute>().Select(x => x.GetIfExpression()));
                        if (string.IsNullOrWhiteSpace(ifExpression)) ifExpression = "true";
                        methods.Add($"getIf{e2.Name}: function(lineItem) {{ return {ifExpression}; }}");
                        methods.Add($"getIfAny{e2.Name}: function() {{ return this.{e.Name}.filter(x => this.getIf{e2.Name}(x)).length > 0; }}");

                        if (e2.GetMemberType() == typeof(object))
                        {
                            var expression = e2.GetCustomAttribute<ManagerServer.Model.Attributes.ExpressionAttribute>()?.GetExpression() ?? "null";
                            methods.Add($"get{e2.Name}: function(lineItem) {{ return {expression} }}");
                        }
                        else if (e2.GetMemberType() == typeof(decimal))
                        {
                            var expression = $"(this.getIf{e2.Name}(lineItem) ? new Decimal(lineItem.{e2.Name}) : new Decimal(0))";
                            methods.Add($"get{e2.Name}: function(lineItem) {{ return {expression} }}");
                        }
                        else if (e2.GetMemberType() == typeof(decimal?))
                        {
                            methods.Add($"get{e2.Name}: function(lineItem) {{ return this.getIf{e2.Name}(lineItem) && lineItem.{e2.Name} != null ? new Decimal(lineItem.{e2.Name}) : null; }}");
                        }
                        else
                        {
                            var substitute = e2.GetCustomAttribute<ManagerServer.Model.Attributes.SubstituteAttribute>()?.GetExpression() ?? "null";

                            methods.Add($"get{e2.Name}Substitute: function(lineItem) {{ return {substitute}; }}");
                            methods.Add($"get{e2.Name}: function(lineItem) {{ return this.get{e2.Name}Substitute(lineItem) || (this.getIf{e2.Name}(lineItem) ? lineItem.{e2.Name} : null); }}");
                        }

                        methods.Add($"get{e2.Name}Array: function() {{ return this.{e.Name}.map(x => this.getIf{e2.Name}(x) ? this.get{e2.Name}(x) : null).filter(x => x != null); }}");
                    }
                }
                else if (e.GetMemberType() == typeof(object))
                {
                    var ifExpression2 = string.Join(" && ", e.GetCustomAttributes<ManagerServer.Model.Attributes.IfAttribute>().Select(x => x.GetIfExpression()));
                    if (string.IsNullOrWhiteSpace(ifExpression2)) ifExpression2 = "true";
                    methods.Add($"getIf{e.Name}: function() {{ return {ifExpression2}; }}");

                    var expression = "new Decimal(0)";
                    if (e.GetCustomAttribute<ManagerServer.Model.Attributes.ExpressionAttribute>() != null)
                    {
                        expression = e.GetCustomAttribute<ManagerServer.Model.Attributes.ExpressionAttribute>()?.GetExpression() ?? expression;
                    }
                    methods.Add($"get{e.Name}: function() {{ return {expression} }}");
                }
                else
                {
                    var ifJoin = "&&";
                    if (e.GetCustomAttributes<ManagerServer.Model.Attributes.OrAttribute>().Any()) ifJoin = "||";
                    var ifExpression = string.Join($" {ifJoin} ", e.GetCustomAttributes<ManagerServer.Model.Attributes.IfAttribute>().Select(x => x.GetIfExpression()));
                    if (string.IsNullOrWhiteSpace(ifExpression)) ifExpression = "true";
                    methods.Add($"getIf{e.Name}: function() {{ return {ifExpression}; }}");
                    methods.Add($"get{e.Name}: function() {{ return this.getIf{e.Name}() ? this.{e.Name} : null; }}");
                }
            }

            using (Script())
            {
                var tabs = this.GetTabs(false, Business);
                var baseCurrency = ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.BaseCurrency>();
                Write("const baseCurrency = "+Newtonsoft.Json.JsonConvert.SerializeObject(new { code = baseCurrency.GetCode(), decimalPlaces = baseCurrency.GetDecimalPlaces() }) + ";");
                Write("const foreignCurrencies = "+Newtonsoft.Json.JsonConvert.SerializeObject(ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.ForeignCurrency>().ToDictionary(x => x.Key, x => new { code = x.GetCode(), decimalPlaces = x.GetDecimalPlaces() }))+";");
                Write("const decimalSeparator = "+ System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator.EncodeJsString()+";");
                Write("const goodsReceipts = "+tabs.GoodsReceipts.Visible.ToString().ToLowerInvariant()+";");
                Write("const deliveryNotes = " + tabs.DeliveryNotes.Visible.ToString().ToLowerInvariant() + ";");
                Write("const billableTime = " + tabs.BillableTime.Visible.ToString().ToLowerInvariant() + ";");
                //Write("const inventoryAutomaticRevaluation = " + Manager.ApplicationData.Businesses.Get(FileID).Single<InventoryAutomaticRevaluation>().Enabled.ToString().ToLowerInvariant() + ";");

                var withholdingTax = ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.WithholdingTax>();
                Write($"const withholdingTaxReceivable = {withholdingTax.WithholdingTaxReceivable.ToString().ToLowerInvariant()};");
                Write($"const withholdingTaxPayable = {withholdingTax.WithholdingTaxPayable.ToString().ToLowerInvariant()};");

                Write("Vue.component('v-select', VueSelect.VueSelect);");
                Write(@"app = new Vue({ el: ""#v-model-form"", data: ");
                foreach (var e in sb.GetChunks()) Write(e.ToString());
                Write(@", methods: { " + string.Join($",{Environment.NewLine}", methods) + " }");
                Write(" })");
            }

            using (Script())
            {
                var baseCurrencyJson = @""""+typeof(BaseCurrency).Name+@""":"+Newtonsoft.Json.JsonConvert.SerializeObject(ApplicationData.Businesses.Get(Business).Single<BaseCurrency>());

                Write(@"
function postContentAndReturnDecimal(owner, url, setterFunction) {
    owner.setAttribute('disabled','');

    const icon = owner.querySelector('i');
    if (icon) icon.classList.add('fa-spin');

    return fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: '{ " + baseCurrencyJson+@", """ + typeof(T).Name+ @""": '+JSON.stringify(app.$data)+'}'
    })
    .then(response => {
        if (!response.ok) throw new Error('Network response was not ok');
        return response.json();
    })
    .then(data => {
        if (data.Error) throw new Error(data.Message);
        setterFunction(parseFloat(data.Value));
        owner.removeAttribute('disabled');
        if (icon) icon.classList.remove('fa-spin');
    })
    .catch(error => {
        alert(error);
        owner.removeAttribute('disabled');
        if (icon) icon.classList.remove('fa-spin');
    });
}
");
            }
        }

        public override async Task Put()
        {
            var userPermissions = GetCurrentUserPermissions(Business);

            if (Key.HasValue)
            {
                if (!userPermissions.CanUpdate(this.GetType().Namespace))
                {
                    Response.StatusCode = 400;
                    await Response.WriteAsync("You don't have a permission to update this object.");
                    return;
                }
            }
            else
            {
                if (!userPermissions.CanCreate(this.GetType().Namespace))
                {
                    Response.StatusCode = 400;
                    await Response.WriteAsync("You don't have a permission to create this object.");
                    return;
                }
            }

            var json = string.Empty;
            using (var s = new System.IO.StreamReader(Request.Body)) json = await s.ReadToEndAsync();

            var newObject = (ManagerServer.Model.Object)Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(T));
            newObject.Key = Key ?? Guid.CreateVersion7();

            ApplicationData.Businesses.Process(Business, newObject, GetUserName());
            Response.StatusCode = 200;
            await Response.WriteAsync("OK");
        }

        protected override async Task InnerPost()
        {
            var form = await Request.ReadFormAsync();
            if (form.ContainsKey("Form"))
            {
                await Get();
                return;
            }

            if (!ApplicationData.Businesses.Exists(Business))
            {
                Response.StatusCode = 400;
                await Response.WriteAsync("Business does not exist.");
                return;
            }

            Response.ContentType = "text/plain; charset=utf-8"; // Ajax expects XML response. This will prevent that.

            var userPermissions = GetCurrentUserPermissions(Business);

            if (Key.HasValue)
            {
                if (!userPermissions.CanUpdate(this.GetType().Namespace))
                {
                    Response.StatusCode = 400;
                    await Response.WriteAsync("You don't have a permission to update this object.");
                    return;
                }
            }
            else
            {
                if (!userPermissions.CanCreate(this.GetType().Namespace))
                {
                    Response.StatusCode = 400;
                    await Response.WriteAsync("You don't have a permission to create this object.");
                    return;
                }
            }
            
            var json = form["febb4049-dcdb-4c7a-a395-4b71da72a85b"]; // magic string
            if (string.IsNullOrWhiteSpace(json))
            {
                Response.StatusCode = 400;
                await Response.WriteAsync("NULL");
                return;
            }
            var jsonSettings = new JsonSerializerSettings();
            jsonSettings.Converters.Add(new GuidJsonConverter());
            jsonSettings.Converters.Add(new MemberInfoSerializer());
            jsonSettings.Converters.Add(new DateTimeConverter());
            jsonSettings.Converters.Add(new IntConverter());
            jsonSettings.Converters.Add(new DecimalConverter());
            jsonSettings.Converters.Add(new StringConverter());
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
            var o = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json, jsonSettings);
            o.Key = Key ?? Guid.CreateVersion7();

            var passwordField = typeof(T).GetFieldOrProperty("Password");
            if (passwordField != null && passwordField.GetMemberType() == typeof(string))
            {
                var newPassword = passwordField.GetMemberValue(o) as string;
                if (string.IsNullOrWhiteSpace(newPassword))
                {
                    var o2 = ApplicationData.Businesses.Get(Business).SingleOrDefault<T>(o.Key);
                    if (o2 != null)
                    {
                        var currentPassword = passwordField.GetMemberValue(o2);
                        passwordField.SetMemberValue(o, currentPassword);
                    }
                }
            }

            var lockDate = ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.LockDate>();
            if (lockDate.GetLockDate().HasValue)
            {
                if (o is Transaction newTransaction)
                {
                    var newTransactions = (newTransaction.CreateGeneralLedgerTransactions(ApplicationData.Businesses.Get(Business)) ?? []).Where(x => x.Date <= lockDate.GetLockDate().Value).Select(x => x.GetHashCode2()).ToList();

                    if (Key.HasValue)
                    {
                        var o2 = ApplicationData.Businesses.Get(Business).SingleOrDefault<T>(Key);
                        if (o2 is Transaction oldTransaction)
                        {
                            foreach (var e in oldTransaction.GetGeneralLedgerTransactions(ApplicationData.Businesses.Get(Business)).Where(x => x.Date <= lockDate.GetLockDate().Value).Select(x => x.GetHashCode2()))
                            {
                                if (newTransactions.Contains(e))
                                {
                                    newTransactions.Remove(e);
                                }
                                else
                                {
                                    newTransactions.Add(-1);
                                    break;
                                }
                            }
                        }
                    }

                    if (newTransactions.Any())
                    {
                        Response.StatusCode = 400;
                        await Response.WriteAsync("No transaction dated before lock date (" + lockDate.GetLockDate().Value.ToLocalShortDisplayString() + @") can be created or updated.");
                        return;
                    }
                }
                if (o is ExchangeRate newExchangeRate)
                {
                    var oldExchangeRate = ApplicationData.Businesses.Get(Business).SingleOrDefault<T>(Key) as ManagerServer.Model.ExchangeRate;
                    if (oldExchangeRate != null)
                    {
                        if (oldExchangeRate.Date <= lockDate.GetLockDate().Value)
                        {
                            Response.StatusCode = 400;
                            await Response.WriteAsync("No exchange rate dated before lock date (" + lockDate.GetLockDate().Value.ToLocalShortDisplayString() + @") can be created or updated.");
                            return;
                        }
                    }

                    if (newExchangeRate.Date <= lockDate.GetLockDate().Value)
                    {
                        Response.StatusCode = 400;
                        await Response.WriteAsync("No exchange rate dated before lock date (" + lockDate.GetLockDate().Value.ToLocalShortDisplayString() + @") can be created or updated.");
                        return;
                    }
                }
                if (o is InventoryUnitCost newInventoryUnitCost)
                {
                    var oldInventoryUnitCost = ApplicationData.Businesses.Get(Business).SingleOrDefault<T>(Key) as ManagerServer.Model.InventoryUnitCost;
                    if (oldInventoryUnitCost != null)
                    {
                        if (oldInventoryUnitCost.Date <= lockDate.GetLockDate().Value)
                        {
                            Response.StatusCode = 400;
                            await Response.WriteAsync("No inventory unit cost dated before lock date (" + lockDate.GetLockDate().Value.ToLocalShortDisplayString() + @") can be created or updated.");
                            return;
                        }
                    }

                    if (newInventoryUnitCost.Date <= lockDate.GetLockDate().Value)
                    {
                        Response.StatusCode = 400;
                        await Response.WriteAsync("No inventory unit cost dated before lock date (" + lockDate.GetLockDate().Value.ToLocalShortDisplayString() + @") can be created or updated.");
                        return;
                    }
                }
            }

            if (o.Key != ManagerServer.Model.Object.GetGuidByType(typeof(T))) // Form defaults ignored
            {
                if (o is IHasAutomaticReference o2 && o2.AutomaticReference) // Automatic reference field is checked
                {
                    var nextReference = o2.GetNextReference(ApplicationData.Businesses.Get(Business).OfType<T>().Cast<IHasAutomaticReference>());
                    o2.AutomaticReference = false;
                    o2.Reference = nextReference.ToString();
                }
                if (o is IForeignCurrencyTransaction foreignCurrencyTransaction && foreignCurrencyTransaction.Currency.HasValue && foreignCurrencyTransaction.ExchangeRate == 0m)
                {
                    var database = ApplicationData.Businesses.Get(Business);
                    var foreignCurrency = database.SingleOrDefault<ForeignCurrency>(foreignCurrencyTransaction.Currency.Value);
                    if (foreignCurrency == null)
                    {
                        var bankOrCashAccount = database.SingleOrDefault<BankOrCashAccount>(foreignCurrencyTransaction.Currency.Value);
                        var customer = database.SingleOrDefault<Customer>(foreignCurrencyTransaction.Currency.Value);
                        var supplier = database.SingleOrDefault<Supplier>(foreignCurrencyTransaction.Currency.Value);
                        var employee = database.SingleOrDefault<Employee>(foreignCurrencyTransaction.Currency.Value);
                        var specialAccount = database.SingleOrDefault<SpecialAccount>(foreignCurrencyTransaction.Currency.Value);
                        var foreignCurrencyKey = bankOrCashAccount?.Currency ?? customer?.Currency ?? supplier?.Currency ?? employee?.Currency ?? employee?.Currency ?? specialAccount?.Currency;
                        foreignCurrency = database.SingleOrDefault<ForeignCurrency>(foreignCurrencyKey);
                    }
                    if (foreignCurrency != null)
                    {
                        var latestExchangeRate = database.OfType<ExchangeRate>().Where(x => x.Currency == foreignCurrency.Key && x.Date <= foreignCurrencyTransaction.Date).OrderByDescending(x => x.Date).FirstOrDefault();
                        if (latestExchangeRate != null)
                        {
                            foreignCurrencyTransaction.ExchangeRate = latestExchangeRate.ExchangeRateValue;
                            foreignCurrencyTransaction.ExchangeRateIsInverse = latestExchangeRate.ExchangeRateIsInverse;
                        }
                        else
                        {
                            foreignCurrencyTransaction.ExchangeRate = 1m;
                        }
                    }
                }
            }

            try
            {
                ApplicationData.Businesses.Process(Business, o, GetUserName());
            }
            catch (SQLiteException ex)
            {
                Response.StatusCode = 400;
                await Response.WriteAsync(ex.Message);
                return;
            }

            if (form.Files["Image"] != null)
            {
                using (var ms = new System.IO.MemoryStream())
                {
                    await form.Files["Image"].CopyToAsync(ms);
                    ApplicationData.Businesses.InsertOrReplaceImage(Business, o.Key, ms.ToArray(), form.Files["Image"].ContentType);
                }
            }
            else if (form.ContainsKey("ImageDeleted") && form["ImageDeleted"] == "true")
            {
                ApplicationData.Businesses.DeleteImage(Business, o.Key);
            }

            if (form[nameof(Strings.CreateAndAddAnother)] == "true")
            {
                Response.Headers["HX-Redirect"] = this.ToUrl();
                return;
            }

            var defaultView = Assembly.GetHttpHandlerTypeByCamelCaseKey($"{typeof(T).Name}View");

            if (!Key.HasValue)
            {
                if (defaultView != null && defaultView.IsSubclassOf(typeof(BaseView3)))
                {
                    var baseView3 = Activator.CreateInstance(defaultView) as BaseView3;
                    if (baseView3 != null)
                    {
                        baseView3.HttpContext = HttpContext;
                        baseView3.Business = Business;
                        baseView3.Key = o.Key;
                        baseView3.Referrer = Referrer;
                        Response.Headers["HX-Redirect"] = baseView3.ToUrl();
                        return;
                    }
                }
            }            

            if (Referrer != null)
            {
                Response.Headers["HX-Redirect"] = Referrer;
            }
            else
            {
                Response.Headers["HX-Redirect"] = new Start() { Business = Business }.ToUrl();
            }
        }

        public class StringConverter : JsonConverter
        {
            public override bool CanRead => true;
            public override bool CanWrite => false;

            public override bool CanConvert(Type objectType) => objectType == typeof(string);

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                return (reader.Value as string)?.Trim();
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }

        public class GuidJsonConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                if (objectType == typeof(Guid)) return true;
                if (objectType == typeof(Guid?)) return true;
                return false;
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                var jToken = Newtonsoft.Json.Linq.JToken.Load(reader);
                if (jToken.Type == JTokenType.String) return new Guid(jToken.ToString()); // This line is required to handle Figure2 on ReportTransformation because Guids are not expanded.
                if (!jToken.HasValues) return null;
                var key = jToken[nameof(ManagerServer.Model.Object.Key)].ToObject<string>();
                return new Guid(key);
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }        

        public class GuidSerializer : JsonConverter
        {
            public ManagerServer.Database database;
            private Dictionary<Guid, ManagerServer.Model.NamedObject> localizationObjects;
            private string[] expand;

            public GuidSerializer(ManagerServer.Database database, Dictionary<Guid, ManagerServer.Model.NamedObject> localizationObjects, string[] expand)
            {
                this.database = database;
                this.localizationObjects = localizationObjects;
                this.expand = expand;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                var path = string.Join('.', writer.Path.Split('.').Select(x => string.Join(string.Empty, x.TakeWhile(x => x != '['))));

                if (!expand.Contains(path))
                {
                    writer.WriteValue(value);
                    return;
                }

                var serializer2 = new JsonSerializer();
                serializer2.Converters.Add(this);
                serializer2.ContractResolver = new CustomContractResolver(skipKey: false);

                var o = database.SingleOrDefault((Guid)value);
                if (localizationObjects.ContainsKey((Guid)value))
                {
                    serializer2.Serialize(writer, localizationObjects[(Guid)value]);
                }
                else if (o is NamedObject)
                {
                    serializer2.Serialize(writer, o);
                }
                else
                {
                    var type = ManagerServer.Model.Attributes.GuidAttribute.GetTypeByGuid((Guid)value);
                    if (type != null)
                    {
                        if (type.GetCustomAttribute<ManagerServer.Model.Attributes.SingletonAttribute>() != null)
                        {
                            var o2 = database.Single((Guid)value);
                            serializer2.Serialize(writer, o2);
                        }
                        else
                        {
                            serializer2.Serialize(writer, new { Key = (Guid)value, UniqueName = Strings.GetPropertyValue(type) });
                        }
                    }
                    else
                    {
                        serializer2.Serialize(writer, new { Key = (Guid)value, UniqueName = (Guid)value });
                    }
                }
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }

            public override bool CanConvert(Type objectType)
            {
                return objectType.Equals(typeof(Guid)) || objectType.Equals(typeof(Guid?));
            }
        }

        public class MemberInfoSerializer : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                var serializer2 = new JsonSerializer();
                serializer2.Serialize(writer, value);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                var jToken = Newtonsoft.Json.Linq.JToken.Load(reader);
                if (!jToken.HasValues) return null;
                var key = jToken[nameof(ManagerServer.Model.MemberInfo.Key)].ToObject<string>();
                if (string.IsNullOrWhiteSpace(key)) return null;
                return new ManagerServer.Model.MemberInfo() { Key = key };
            }

            public override bool CanConvert(Type objectType)
            {
                if (objectType == typeof(ManagerServer.Model.MemberInfo))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public class DateTimeConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                if (objectType == typeof(DateTime)) return true;
                if (objectType == typeof(DateTime?)) return true;
                return false;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                var date = (DateTime)value;
                if (date == DateTime.MinValue)
                {
                    writer.WriteRawValue("new Date(new Date().getTime() - new Date().getTimezoneOffset() * 60000).toISOString().slice(0, 10)");
                }
                else
                {
                    writer.WriteValue(((DateTime)value).ToString("yyyy-M-d"));
                }
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                var jToken = Newtonsoft.Json.Linq.JToken.Load(reader);
                if (jToken.Type == JTokenType.String)
                {
                    var value = jToken.Value<string>();
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        if (DateTime.TryParseExact(value, "yyyy-M-d", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime result))
                        {
                            return result;
                        }
                    }
                }
                return null;
            }
        }

        public class IntConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                if (objectType == typeof(int)) return true;
                if (objectType == typeof(int?)) return true;
                return false;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                var jToken = Newtonsoft.Json.Linq.JToken.Load(reader);
                if (jToken.Type == JTokenType.Float)
                {
                    return (int)jToken.Value<double>();
                }
                else if (jToken.Type == JTokenType.Integer)
                {
                    try
                    {
                        return jToken.Value<int>();
                    }
                    catch (OverflowException) // Value was either too large or too small for an Int32.
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        public class DecimalConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                if (objectType == typeof(decimal)) return true;
                if (objectType == typeof(decimal?)) return true;
                return false;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                var jToken = Newtonsoft.Json.Linq.JToken.Load(reader);
                if (jToken.Type == JTokenType.Float || jToken.Type == JTokenType.Integer)
                {
                    if (jToken is JValue jValue && jValue.Value is IConvertible convertible)
                    {
                        try
                        {
                            // Convert using IConvertible to ensure type safety
                            return convertible.ToDecimal(System.Globalization.CultureInfo.InvariantCulture);
                        }
                        catch (OverflowException)
                        {
                            return decimal.MaxValue;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        public class CustomContractResolver : DefaultContractResolver
        {
            private bool skipKey;

            public CustomContractResolver(bool skipKey)
            {
                this.skipKey = skipKey;
            }

            protected override JsonProperty CreateProperty(System.Reflection.MemberInfo member, MemberSerialization memberSerialization)
            {
                var property = base.CreateProperty(member, memberSerialization);
                if (skipKey && member.Name.Equals(nameof(ManagerServer.Model.Object.Key)) && (member is System.Reflection.FieldInfo || member is System.Reflection.PropertyInfo) && member.GetMemberType() == typeof(Guid))
                {
                    property.ShouldSerialize = _ => false;
                }
                else if (member.Name.StartsWith("Obsolete_"))
                {
                    property.ShouldSerialize = _ => false;
                }
                return property;
            }

            protected override IValueProvider CreateMemberValueProvider(System.Reflection.MemberInfo member)
            {
                IValueProvider provider = base.CreateMemberValueProvider(member);

                if (member is FieldInfo || member is PropertyInfo)
                {
                    var fieldType = member.GetMemberType();
                    if (fieldType.IsArray && fieldType != typeof(byte[]))
                    {
                        var initialSize = 1;
                        if (member.GetCustomAttribute<ManagerServer.Model.Attributes.InitialSizeAttribute>() != null) initialSize = member.GetCustomAttribute<ManagerServer.Model.Attributes.InitialSizeAttribute>().Size;
                        return new EmptyArrayValueProvider(provider, fieldType.GetElementType(), initialSize);
                    }
                    if (fieldType.GetInterfaces().Any(x => x == typeof(System.Collections.IDictionary)))
                    {
                        return new EmptyDictionaryValueProvider(provider);
                    }
                    if (fieldType == typeof(CustomFields))
                    {
                        return new EmptyCustomFieldsValueProvider(provider);
                    }
                    if (member.Name == "Password" && fieldType == typeof(string))
                    {
                        return new DoNotSerializePasswordProvider(provider);
                    }
                }

                return provider;
            }

            // We can't use JsonConverter for this because it doesn't trigger on NULL
            class EmptyArrayValueProvider : IValueProvider
            {
                private IValueProvider innerProvider;
                private Array defaultValue;
                private int initialSize;

                public EmptyArrayValueProvider(IValueProvider innerProvider, Type elementType, int initialSize = 1)
                {
                    this.initialSize = initialSize;
                    this.innerProvider = innerProvider;
                    if (elementType.IsClass && elementType != typeof(string))
                    {
                        defaultValue = Array.CreateInstance(elementType, initialSize);
                        for (int i = 0; i < initialSize; i++) defaultValue.SetValue(Activator.CreateInstance(elementType), i);
                    }
                    else
                    {
                        defaultValue = Array.CreateInstance(typeof(object), 0);
                    }
                }

                public void SetValue(object target, object value)
                {
                    innerProvider.SetValue(target, value ?? defaultValue);
                }

                public object GetValue(object target)
                {
                    return innerProvider.GetValue(target) ?? defaultValue;
                }
            }

            class EmptyDictionaryValueProvider : IValueProvider
            {
                private IValueProvider innerProvider;
                private object defaultValue;

                public EmptyDictionaryValueProvider(IValueProvider innerProvider)
                {
                    this.innerProvider = innerProvider;
                    defaultValue = Activator.CreateInstance(typeof(object));
                }

                public void SetValue(object target, object value)
                {
                    innerProvider.SetValue(target, value ?? defaultValue);
                }

                public object GetValue(object target)
                {
                    return innerProvider.GetValue(target) ?? defaultValue;
                }
            }

            class EmptyCustomFieldsValueProvider : IValueProvider
            {
                private IValueProvider innerProvider;
                private ManagerServer.Model.CustomFields defaultValue;

                public EmptyCustomFieldsValueProvider(IValueProvider innerProvider)
                {
                    this.innerProvider = innerProvider;
                    defaultValue = new CustomFields()
                    {
                        Dates = new Dictionary<Guid, DateTime?>(),
                        Decimals = new Dictionary<Guid, decimal?>(),
                        Strings = new Dictionary<Guid, string>()
                    };
                }

                public void SetValue(object target, object value)
                {
                    innerProvider.SetValue(target, value ?? defaultValue);
                }

                public object GetValue(object target)
                {
                    return innerProvider.GetValue(target) ?? defaultValue;
                }
            }

            class DoNotSerializePasswordProvider : IValueProvider
            {
                private IValueProvider innerProvider;

                public DoNotSerializePasswordProvider(IValueProvider innerProvider)
                {
                    this.innerProvider = innerProvider;
                }

                public void SetValue(object target, object value)
                {
                    innerProvider.SetValue(target, value);
                }

                public object GetValue(object target)
                {
                    return null;
                }
            }            
        }

        public void VSelect(string v_model = null, string search = null, string label = null, string selectable = null, string placeholder = null, string @class = null, string style = null, object[] options = null, string reduce = null, bool? multiple = null, bool? taggable = null)
        {
            string output = @"<v-select";
            if (multiple == true) output += @" multiple";
            if (taggable == true) output += @" taggable";
            if (v_model != null) output += @" v-model=""" + v_model + @"""";
            if (label != null) output += @" label=""" + label + @"""";
            if (style != null) output += @" style=""" + style + @"""";
            if (reduce != null) output += @" :reduce=""" + reduce + @"""";
            if (search != null) output += @" @search=""" + search + @"""";
            if (selectable != null) output += @" :selectable=""" + selectable + @"""";
            if (placeholder != null) output += @" placeholder=""" + placeholder + @"""";
            if (@class != null) output += @" class=""" + @class + @"""";
            if (options != null)
            {
                var sw = new System.IO.StringWriter();
                using (JsonTextWriter writer = new JsonTextWriter(sw))
                {
                    writer.QuoteChar = '\'';
                    writer.StringEscapeHandling = StringEscapeHandling.EscapeHtml;
                    var ser = new JsonSerializer();
                    ser.Serialize(writer, options);
                }

                output += @" :options=""" + sw.ToString() + @"""";
            }
            output += "></v-select>";
            Write(output);
        }      
    }
}