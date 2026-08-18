using ManagerServer.Model;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    internal abstract class NakedObjectsWithCustomFields<T> : NakedObjectsWithEditColumns<T>
    {
        protected virtual Type GetCustomFieldsType()
        {
            return typeof(T);
        }

        public Column[] GetCustomFieldColumns()
        {
            var customColumns = new List<Column>();

            if (typeof(T).IsAssignableTo(typeof(ManagerServer.Model.ICustomFields)))
            {
                var key = GetCustomFieldsType().GetCustomAttribute<ManagerServer.Model.Attributes.GuidAttribute>()?.Value;

                if (key.HasValue)
                {
                    var textCustomFields = ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.TextCustomField>().Where(x => x.Placement != null && x.Placement.Contains(key.Value)).ToArray();
                    var dateCustomFields = ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.DateCustomField>().Where(x => x.Placement != null && x.Placement.Contains(key.Value)).ToArray();
                    var multipleValueCustomFields = ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.MultipleValueCustomField>().Where(x => x.Placement != null && x.Placement.Contains(key.Value)).ToArray();
                    var checkboxCustomFields = ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.CheckboxCustomField>().Where(x => x.Placement != null && x.Placement.Contains(key.Value)).ToArray();
                    var numberCustomFields = ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.NumberCustomField>().Where(x => x.Placement != null && x.Placement.Contains(key.Value)).ToArray();
                    var classicCustomFields = ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.CustomField>().Where(x => x.Placement != null && x.Placement.Contains(key.Value)).ToArray();                    

                    foreach (var e in textCustomFields)
                    {
                        var attributes = new List<Attribute>();
                        attributes.Add(new GuidAttribute(e.Key.ToString()));
                        attributes.Add(new NameAttribute(e.Name));
                        if (e.Type == ManagerServer.Model.Enums.TextCustomFieldType.QrCode)
                        {
                            customColumns.Add(new TextCustomFieldAsQrCodeColumn() { Attributes = attributes.ToArray(), Key = e.Key, Visible = false, Label = e.Name, Priority = int.MaxValue, MergeTag = "%%" + e.Name + "%%" });
                        }
                        else
                        {
                            customColumns.Add(new TextCustomFieldColumn() { Attributes = attributes.ToArray(), Key = e.Key, Visible = false, Label = e.Name, Priority = int.MaxValue, MergeTag = "%%" + e.Name + "%%" });
                        }
                    }
                    foreach (var e in dateCustomFields)
                    {
                        var attributes = new List<Attribute>();
                        attributes.Add(new GuidAttribute(e.Key.ToString()));
                        attributes.Add(new NameAttribute(e.Name));
                        attributes.Add(new CenterAttribute());
                        customColumns.Add(new DateCustomFieldColumn() { Attributes = attributes.ToArray(), Key = e.Key, Visible = false, Label = e.Name, Priority = int.MaxValue, MergeTag = "%%" + e.Name + "%%" });
                    }
                    foreach (var e in multipleValueCustomFields)
                    {
                        var attributes = new List<Attribute>();
                        attributes.Add(new GuidAttribute(e.Key.ToString()));
                        attributes.Add(new NameAttribute(e.Name));
                        customColumns.Add(new StringArrayCustomFieldColumn() { Attributes = attributes.ToArray(), Key = e.Key, Visible = false, Label = e.Name, Priority = int.MaxValue, MergeTag = "%%" + e.Name + "%%" });
                    }
                    foreach (var e in checkboxCustomFields)
                    {
                        var attributes = new List<Attribute>();
                        attributes.Add(new GuidAttribute(e.Key.ToString()));
                        attributes.Add(new NameAttribute(e.Name));
                        attributes.Add(new CenterAttribute());
                        customColumns.Add(new BooleanCustomFieldColumn() { Attributes = attributes.ToArray(), Key = e.Key, Visible = false, Label = e.Name, Priority = int.MaxValue, MergeTag = "%%" + e.Name + "%%" });
                    }
                    foreach (var e in numberCustomFields)
                    {
                        var attributes = new List<Attribute>();
                        attributes.Add(new GuidAttribute(e.Key.ToString()));
                        attributes.Add(new NameAttribute(e.Name));
                        if (!e.HideTotalAmount) attributes.Add(new SumAttribute());
                        attributes.Add(new RightAttribute());
                        customColumns.Add(new DecimalCustomFieldColumn(e) { Attributes = attributes.ToArray(), Key = e.Key, Visible = false, Label = e.Name, Priority = int.MaxValue, MergeTag = "%%" + e.Name + "%%" });
                    }
                    foreach (var e in classicCustomFields)
                    {
                        var attributes = new List<Attribute>();
                        attributes.Add(new GuidAttribute(e.Key.ToString()));
                        attributes.Add(new NameAttribute(e.Name));
                        customColumns.Add(new ClassicCustomFieldColumn() { Attributes = attributes.ToArray(), Key = e.Key, Visible = false, Label = e.Name, Priority = int.MaxValue, MergeTag = "%%" + e.Name + "%%" });
                    }
                }
            }

            return customColumns.ToArray();
        }

        protected override void InnerGet4(Context context)
        {
            var customFieldColumns = GetCustomFieldColumns().ToList();
            if (customFieldColumns.Count > 0)
            {
                customFieldColumns.AddRange(context.Get<Column[]>());
                context.Set(customFieldColumns.OrderBy(x => x.Priority).ToArray());
            }            

            base.InnerGet4(context);
        }

        protected sealed class TextCustomFieldColumn : Column<string>
        {
            public override void EnsureCells(Array rows)
            {
                var rows2 = (T[])rows;
                var values = rows2.Cast<ICustomFields>().Select(x => x.CustomFields != null && x.CustomFields.Strings != null ? (x.CustomFields.Strings.TryGetValue(Key.Value, out string value) ? value : string.Empty) : string.Empty).ToArray();
                AddValues(rows, values);
            }
        }

        protected sealed class TextCustomFieldAsQrCodeColumn : Column<QrCode>
        {
            public override void EnsureCells(Array rows)
            {
                var rows2 = (T[])rows;
                var values = rows2.Cast<ICustomFields>().Select(x => x.CustomFields != null && x.CustomFields.Strings != null ? (x.CustomFields.Strings.TryGetValue(Key.Value, out string value) ? value : string.Empty) : string.Empty).ToArray();
                AddValues(rows, values.Select(x => new QrCode() { Value = x }).ToArray());
            }
        }

        protected sealed class DateCustomFieldColumn : Column<DateTime?>
        {
            public override void EnsureCells(Array rows)
            {
                var rows2 = (T[])rows;
                var values = rows2.Cast<ICustomFields>().Select(x => x.CustomFields != null && x.CustomFields.Dates != null ? (x.CustomFields.Dates.TryGetValue(Key.Value, out DateTime? value) ? value : null) : null).ToArray();
                AddValues(rows, values);
            }
        }

        protected sealed class DecimalCustomFieldColumn : Column<decimal?>
        {
            private NumberCustomField numberCustomField;

            public DecimalCustomFieldColumn(NumberCustomField numberCustomField)
            {
                this.numberCustomField = numberCustomField;
            }

            public override void EnsureCells(Array rows)
            {
                var rows2 = (T[])rows;
                var values = rows2.Cast<ICustomFields>().Select(x => x.CustomFields != null && x.CustomFields.Decimals != null ? (x.CustomFields.Decimals.TryGetValue(Key.Value, out decimal? value) ? numberCustomField.FormatDecimal(value) : null) : null).ToArray();
                AddValues(rows, values);
            }
        }

        protected sealed class BooleanCustomFieldColumn : Column<bool>
        {
            public override void EnsureCells(Array rows)
            {
                var rows2 = (T[])rows;
                var values = rows2.Cast<ICustomFields>().Select(x => x.CustomFields != null && x.CustomFields.Booleans != null ? (x.CustomFields.Booleans.TryGetValue(Key.Value, out bool value) ? value : false) : false).ToArray();
                AddValues(rows, values);
            }
        }

        protected sealed class StringArrayCustomFieldColumn : Column<string[]>
        {
            public override void EnsureCells(Array rows)
            {
                var rows2 = (T[])rows;
                var values = rows2.Cast<ICustomFields>().Select(x => x.CustomFields != null && x.CustomFields.StringArrays != null ? (x.CustomFields.StringArrays.TryGetValue(Key.Value, out string[] value) ? value : Array.Empty<string>()) : Array.Empty<string>()).ToArray();
                AddValues(rows, values);
            }
        }

        protected sealed class ClassicCustomFieldColumn : Column<string>
        {
            public override void EnsureCells(Array rows)
            {
                var rows2 = (T[])rows;
                var values = rows2.Cast<ICustomFields>().Select(x => x.ClassicCustomFields != null ? (x.ClassicCustomFields.TryGetValue(Key.Value, out string value) ? value : string.Empty) : string.Empty).ToArray();
                AddValues(rows, values);
            }
        }
    }
}
