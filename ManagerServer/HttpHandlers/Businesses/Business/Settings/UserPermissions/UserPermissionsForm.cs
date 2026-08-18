using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Helpers;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.UserPermissions
{
    [ProtoContract]
    [Title(nameof(Strings.UserPermissions), nameof(Strings.Edit))]
    [Guide("`UserPermissions` control which areas of Manager a user can access and what actions they can perform.")]
    [Guide("Assign permissions to users who need access to this business file without giving them full administrative rights.")]
    [Guide("Configure user permissions using these settings:")]
    [Fields(typeof(ManagerServer.Model.UserPermissions))]
    internal sealed class UserPermissionsForm : VueForm<ManagerServer.Model.UserPermissions>
    {
        [ProtoMember(1)] public string Username;

        protected override void InnerGet3()
        {
            using (Div(@class: "form-group"))
            {
                using (Label()) Write(Strings.Username);
                InputText(v_model: nameof(ManagerServer.Model.UserPermissions.Username), @class: "form-control", style: "width: 300px");
            }

            using (Div(@class: "form-group"))
            {
                using (Label()) Write(Strings.AccessType);
                using (Div()) using (Select(@class: "form-select", style: "width: auto", v_model: nameof(ManagerServer.Model.UserPermissions.AccessType)))
                {
                    Option(value: ((int)ManagerServer.Model.Enums.UserPermissionsAccessType.CustomAccess).ToString(), text: Strings.CustomAccess);
                    Option(value: ((int)ManagerServer.Model.Enums.UserPermissionsAccessType.FullAccess).ToString(), text: Strings.FullAccess);
                }
            }            

            using (Div(v_if: $"{nameof(ManagerServer.Model.UserPermissions.AccessType)} == {((int)ManagerServer.Model.Enums.UserPermissionsAccessType.CustomAccess).ToString()}"))
            {
                var namespacePrefix = typeof(BusinessTemplate).Namespace + ".";
                var namespaces = GetType()
                    .Assembly
                    .GetTypes()
                    .Where(x => x.Namespace != null)
                    .Where(x => x.Namespace.StartsWith(namespacePrefix))                    
                    .Select(x => x.Namespace.Substring(namespacePrefix.Length))
                    .Distinct()
                    .Where(x => x != nameof(HttpHandlers.Businesses.Business.Emails))
                    .Where(x => x != nameof(HttpHandlers.Businesses.Business.Attachments))
                    .Where(x => x != nameof(HttpHandlers.Businesses.Business.Settings)+"."+nameof(HttpHandlers.Businesses.Business.Settings.UserPermissions))
                    .ToArray();

                RenderCheckboxes(namespaces, string.Empty);
            }
        }

        private bool RenderCheckboxes(string[] namespaces, string prefix)
        {
            var permittedActionOptions = new[]
            {
                new { label = Strings.View, value = (int)ManagerServer.Model.PermittedActions.View },
                new { label = string.Join(", ",new string[] { Strings.View, Strings.Create }), value = (int)ManagerServer.Model.PermittedActions.ViewCreate },
                new { label = string.Join(", ",new string[] { Strings.View, Strings.Create, Strings.Update }), value = (int)ManagerServer.Model.PermittedActions.ViewCreateUpdate },
                new { label = string.Join(", ",new string[] { Strings.View, Strings.Create, Strings.Update, Strings.Delete }), value = (int)ManagerServer.Model.PermittedActions.ViewCreateUpdateDelete },
            };

            var any = false;

            var level = prefix.Count(x => x == '.');
            foreach (var e in namespaces.Where(x => x.StartsWith(prefix) && x.Split('.').Length == level+1).OrderBy(x => Strings.GetPropertyValue(x.Split('.').Last())))
            {
                any = true;
                using (Div(@class: "flex items-start gap-2 my-1"))
                {
                    InputCheckbox(id: e, @class: "form-check-input", v_model: $"{nameof(ManagerServer.Model.UserPermissions.Namespaces)}['{e}']");
                    using (Div())
                    {
                        using (Label(@for: e)) Write(Strings.GetPropertyValue(e.Split('.').Last()));
                        using (Div(v_if: $"{nameof(ManagerServer.Model.UserPermissions.Namespaces)}['{e}']"))
                        {
                            if (e == nameof(HttpHandlers.Businesses.Business.BankAndCashAccounts))
                            {
                                var list = new List<Model.IBankOrCashAccount>();
                                list.AddRange(ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.BankOrCashAccount>().OrderBy(x => x.GetName()).ToArray());
                                VSelect(v_model: nameof(ManagerServer.Model.UserPermissions.BankAndCashAccounts), multiple: true, options: list.ToArray(), label: "Name", selectable: $"x => !{nameof(ManagerServer.Model.UserPermissions.BankAndCashAccounts)}.some(y => y.Key == x.Key)", style: "min-width: 200px");
                            }                            

                            var innerAny = RenderCheckboxes(namespaces, e+'.');
                            if (!innerAny)
                            {
                                using (Select(@class: "form-select", style: "width: auto", v_model: $"{nameof(ManagerServer.Model.UserPermissions.Namespaces2)}['{e}']"))
                                {
                                    foreach (var e2 in permittedActionOptions)
                                    {
                                        Option(value: e2.value.ToString(), text: e2.label);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return any;
        }
    }
}