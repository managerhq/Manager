using ManagerServer.Globalization;
using ManagerServer.Helpers;
using ManagerServer.Model;
using System.Collections.Generic;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business.History
{
    [ProtoContract]
    internal sealed class GetChangeView : ViewEndpoint<View>, IView
    {
        public override View AuthorizedHandle()
        {
            if (!UserPermissions.FullAccess) return null;

            ManagerServer.Model.Object objectBefore;
            ManagerServer.Model.Object objectAfter;
            using (var c = GetApplicationData().Businesses.SQLiteConnection(Business))
            {
                var change = c.Find<ManagerServer.ApplicationData.Change>(Key);
                if (change == null) return null;

                objectBefore = (change.ContentTypeBefore != Guid.Empty) ? Serialization.Deserialize(change.ContentTypeBefore, change.ContentBefore) : null;
                objectAfter = (change.ContentTypeAfter != Guid.Empty) ? Serialization.Deserialize(change.ContentTypeAfter, change.ContentAfter) : null;
            }

            Languages.SetLanguage(Language);

            var view = new View { BusinessName = Business, Title = string.Empty };

            if (objectBefore == null && objectAfter == null)
            {
                return view;
            }

            if (objectBefore == null && objectAfter != null)
            {
                view.Title = Strings.GetPropertyValue(objectAfter.GetType().Name);
                view.Status = new View.StatusInfo { Text = Strings.Create, Tone = Tone.Positive };
                AppendObjectFields(view.Fields, objectBefore, objectAfter);
            }
            else if (objectBefore != null && objectAfter == null)
            {
                view.Title = Strings.GetPropertyValue(objectBefore.GetType().Name);
                view.Status = new View.StatusInfo { Text = Strings.Delete, Tone = Tone.Negative };
                AppendObjectFields(view.Fields, objectBefore, objectAfter);
            }
            else if (objectBefore.GetType() == objectAfter.GetType())
            {
                view.Title = Strings.GetPropertyValue(objectAfter.GetType().Name);
                AppendObjectFields(view.Fields, objectBefore, objectAfter);
            }
            else
            {
                view.Title = Strings.GetPropertyValue(objectAfter.GetType().Name);
                AppendObjectFields(view.Fields, objectBefore, null);
                AppendObjectFields(view.Fields, null, objectAfter);
            }

            view.Direction = Languages.IsRightToLeft() ? Direction.Rtl : Direction.Ltr;
            view.Language = Languages.GetLanguage();
            return view;
        }

        private void AppendObjectFields(List<View.FieldInfo> target, object objectBefore, object objectAfter)
        {
            var type = objectBefore?.GetType() ?? objectAfter?.GetType();
            if (type == null) return;

            foreach (var m in type.GetFieldsAndProperties())
            {
#if !DEBUG
                if (m.Name.StartsWith("Obsolete_")) continue;
#endif
                if (m.Name == nameof(Model.Object.Key)) continue;
                if (m.Name == nameof(Model.Object.Timestamp)) continue;

                var label = Strings.GetPropertyValue(m.Name);

                if (m.GetMemberType().IsArray)
                {
                    var field = BuildArrayField(label, m, objectBefore, objectAfter);
                    if (field != null) target.Add(field);
                }
                else
                {
                    var field = BuildScalarField(label, m, objectBefore, objectAfter);
                    if (field != null) target.Add(field);
                }
            }
        }

        private View.FieldInfo BuildScalarField(string label, System.Reflection.MemberInfo m, object objectBefore, object objectAfter)
        {
            var beforeText = objectBefore != null ? GetText(m.GetMemberValue(objectBefore)) : string.Empty;
            var afterText = objectAfter != null ? GetText(m.GetMemberValue(objectAfter)) : string.Empty;

            if (string.IsNullOrWhiteSpace(beforeText) && string.IsNullOrWhiteSpace(afterText)) return null;

            if (m.Name == nameof(EmailSettings.Password))
            {
                if (beforeText == afterText)
                {
                    beforeText = "********";
                    afterText = "********";
                }
                else
                {
                    beforeText = "*******";
                    afterText = "********";
                }
            }

            return BuildLeafField(label, beforeText, afterText, objectBefore != null, objectAfter != null);
        }

        private View.FieldInfo BuildArrayField(string label, System.Reflection.MemberInfo m, object objectBefore, object objectAfter)
        {
            Array arrayBefore = objectBefore != null ? m.GetMemberValue(objectBefore) as Array : null;
            Array arrayAfter = objectAfter != null ? m.GetMemberValue(objectAfter) as Array : null;

            var count = 0;
            if (arrayBefore != null) count = arrayBefore.Length;
            if (arrayAfter != null && arrayAfter.Length > count) count = arrayAfter.Length;
            if (count == 0) return null;

            var elementType = m.GetMemberType().GetElementType();
            var lineMembers = elementType.GetFieldsAndProperties().Where(x => !x.Name.StartsWith("Obsolete_")).ToArray();

            var rows = new List<View.FieldInfo>();
            for (int i = 0; i < count; i++)
            {
                var lineBefore = (arrayBefore != null && arrayBefore.Length > i) ? arrayBefore.GetValue(i) : null;
                var lineAfter = (arrayAfter != null && arrayAfter.Length > i) ? arrayAfter.GetValue(i) : null;

                var cells = new List<View.FieldInfo>();
                foreach (var lm in lineMembers)
                {
                    var cellBefore = lineBefore != null ? GetText(lm.GetMemberValue(lineBefore)) : null;
                    var cellAfter = lineAfter != null ? GetText(lm.GetMemberValue(lineAfter)) : null;

                    if (string.IsNullOrWhiteSpace(cellBefore) && string.IsNullOrWhiteSpace(cellAfter)) continue;

                    var cell = BuildLeafField(Strings.GetPropertyValue(lm.Name), cellBefore ?? string.Empty, cellAfter ?? string.Empty, lineBefore != null, lineAfter != null);
                    if (cell != null) cells.Add(cell);
                }

                if (cells.Count == 0) continue;

                rows.Add(new View.FieldInfo
                {
                    Label = (i + 1).ToString(),
                    Fields = cells,
                });
            }

            if (rows.Count == 0) return null;

            return new View.FieldInfo
            {
                Key = m.Name,
                Label = label,
                Fields = rows,
            };
        }

        private static View.FieldInfo BuildLeafField(string label, string beforeText, string afterText, bool hasBefore, bool hasAfter)
        {
            if (!hasBefore)
            {
                return new View.FieldInfo { Label = label, Text = afterText };
            }
            if (!hasAfter)
            {
                return new View.FieldInfo { Label = label, Text = beforeText };
            }
            if (beforeText == afterText)
            {
                return new View.FieldInfo { Label = label, Text = beforeText };
            }

            var children = new List<View.FieldInfo>();
            if (!string.IsNullOrWhiteSpace(beforeText))
            {
                children.Add(new View.FieldInfo { Label = "−", Text = beforeText });
            }
            if (!string.IsNullOrWhiteSpace(afterText))
            {
                children.Add(new View.FieldInfo { Label = "+", Text = afterText, Emphasis = true });
            }

            return new View.FieldInfo
            {
                Label = label,
                Fields = children,
            };
        }

        private string GetText(object o)
        {
            if (o == null) return string.Empty;
            if (o is string s) return s;
            if (o is bool b) return b ? Strings.IsChecked : string.Empty;
            if (o is DateTime dt) return dt == default ? string.Empty : dt.ToLocalShortDisplayString();
            if (o is decimal dec) return dec == default ? string.Empty : dec.ToNumberString();
            if (o is int i) return i == default ? string.Empty : i.ToString("N0", System.Threading.Thread.CurrentThread.CurrentCulture);
            if (o is Enum) return ((int)o) == default ? string.Empty : o.ToString();
            if (o is Guid g) return GetApplicationData().Businesses.Get(Business).SingleOrDefault<NamedObject>(g)?.GetName() ?? string.Empty;
            return string.Empty;
        }

        public View GetView() => AuthenticatedHandle();
    }
}
