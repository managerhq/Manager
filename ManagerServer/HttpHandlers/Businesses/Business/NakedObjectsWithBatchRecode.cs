using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Attributes;
using System.Reflection;
using ManagerServer.Model.Attributes;
using HttpFramework;
using MemberInfo = System.Reflection.MemberInfo;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    internal abstract class NakedObjectsWithBatchRecode<T> : NakedObjectsWithBatchDelete<T> where T : ManagerServer.Model.Object, new()
    {
        [InheritedProtoMember(290)] public bool BatchRecode;
        [InheritedProtoMember(291)] public string Field;

        protected override void InnerGet4(Context context)
        {
            if (BatchRecode)
            {
                var cancelHandler = (NakedObjectsWithBatchRecode<T>)this.MemberwiseClone();
                cancelHandler.BatchRecode = false;
                cancelHandler.Field = null;

                context.Set(new BatchOperation()
                {
                    Name = Strings.BatchRecode,
                    Cancel = cancelHandler
                });
            }
            base.InnerGet4(context);
        }

        private string GetLabel(MemberInfo field)
        {
            var labelAttributes = field.GetCustomAttribute<LabelAttribute>()?.Value;
            if (labelAttributes != null)
            {
                return string.Join(" - ", labelAttributes.Select(x => ManagerServer.Globalization.Strings.GetPropertyValue(x)));
            }
            else
            {
                return ManagerServer.Globalization.Strings.GetPropertyValue(field.Name);
            }
        }

        public override Tuple<string, byte[]>[] GetBatchOperation(T[] rows)
        {
            if (BatchRecode) return Serialize(nameof(NakedObjectsWithBatchRecode<T>), rows.Select(x => x.Key).ToArray());
            return base.GetBatchOperation(rows);
        }

        protected override void OnBeforeBatchOperationButton()
        {
            if (BatchRecode)
            {
                if (string.IsNullOrWhiteSpace(Field))
                {
                    var options = new List<Tuple<string, string>>();

                    var supportedTypes = new Type[] { typeof(bool) };
                    foreach (var e in typeof(T).GetFieldsAndProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
                    {
                        if (e.Name.StartsWith("Obsolete_")) continue;
                        if (!supportedTypes.Contains(e.GetMemberType())) continue;

                        var httpHandler = (NakedObjectsWithBatchRecode<T>)this.MemberwiseClone();
                        httpHandler.Field = e.Name;

                        options.Add(new Tuple<string, string>(GetLabel(e), httpHandler.ToUrl()));
                    }

                    using (Select(@class: "form-select", onchange: "window.location = this.value"))
                    {
                        Option();
                        foreach (var e in options.OrderBy(x => x.Item1))
                        {
                            Option(text: e.Item1, value: e.Item2);
                        }
                    }
                }
                else
                {
                    var fieldInfo = typeof(T).GetFieldOrProperty(Field);

                    if (fieldInfo != null)
                    {
                        using (Div(@class: "border border-neutral-300 rounded bg-neutral-200 flex items-center items-stretch"))
                        {
                            using (Div(@class: "p-3 text-neutral-600")) Write(GetLabel(fieldInfo));

                            if (fieldInfo.GetMemberType() == typeof(bool))
                            {
                                using (Select(name: "Value", @class: "form-select"))
                                {
                                    Option(value: bool.TrueString, text: Strings.IsChecked);
                                    Option(value: bool.FalseString, text: Strings.IsNotChecked);
                                }
                            }
                        }
                    }
                }

                I(@class: "fas fa-fw fa-right-long text-base opacity-25");
            }
        }

        protected override async Task InnerPost()
        {
            var keys = await Deserialize<Guid>(nameof(NakedObjectsWithBatchRecode<T>));
            if (keys != null && keys.Length > 0)
            {
                var userPermissions = this.GetCurrentUserPermissions(Business);
                if (userPermissions.CanUpdate(this.GetType().Namespace))
                {
                    if (!string.IsNullOrWhiteSpace(Field))
                    {
                        var fieldInfo = typeof(T).GetFieldOrProperty(Field);

                        var list = new List<ManagerServer.Model.Object>();

                        var value = Request.Form["value"].ToString();
                        var newValue = Convert.ChangeType(value, fieldInfo.GetMemberType());

                        var database = ApplicationData.Businesses.Get(Business);

                        foreach (var key in keys)
                        {
                            var o = database.SingleOrDefault<T>(key);
                            if (o == null) continue;

                            var currentValue = fieldInfo.GetMemberValue(o);
                            if (currentValue != newValue)
                            {
                                fieldInfo.SetMemberValue(o, newValue);
                                list.Add(o);
                            }
                        }

                        ApplicationData.Businesses.Process(Business, list.ToArray(), GetUserName());
                    }
                }
            }
            await base.InnerPost();
        }

        protected override void OnFooterEndSection(Context context)
        {
            var userPermissions = this.GetCurrentUserPermissions(Business);
            if (userPermissions.CanUpdate(this.GetType().Namespace))
            {
                var batchOperations = GetBatchOperations(context);

                var batchRecodeHandler = (NakedObjectsWithBatchRecode<T>)this.MemberwiseClone();
                batchRecodeHandler.BatchRecode = true;
                batchRecodeHandler.Field = null;

                batchOperations.Items.Add(new Tuple<string, BusinessTemplate>(Strings.BatchRecode, batchRecodeHandler));
            }

            base.OnFooterEndSection(context);
        }
    }
}
