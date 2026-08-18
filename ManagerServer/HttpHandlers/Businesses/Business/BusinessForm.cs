using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ManagerServer;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    [ProtoContract]
    [Title(nameof(Strings.Business))]
    [Guide("The business form enables administrators to rename a business file within the system.")]
    [Guide("This form provides a secure way to change how a business is identified throughout the application.")]
    [Header("Access Restrictions")]
    [Guide("Only users with administrative privileges can access and use this form to rename businesses.")]
    [Guide("Restricted users can view the form but cannot make changes. They will see a read-only version with a message explaining that only administrators can rename business files.")]
    [Header("Renaming Process")]
    [Guide("When you rename a business, the system automatically updates the business file name on disk and adjusts all associated user permissions.")]
    [Guide("The rename operation includes validation to ensure the new name contains only valid characters and does not conflict with existing business files.")]
    [Guide("If an error occurs during the rename process, such as a file access issue, an error message will be displayed and the original name will be preserved.")]
    internal sealed class BusinessForm : BusinessTemplate
    {
        [ProtoMember(1)] public string Error;

        protected override void InnerGet2()
        {
            using (Div(@class: "card"))
            {
                using (Div(@class: "card-header"))
                {
                    using (Div(@class: "card-title")) Write(Strings.BusinessName);
                }

                var currentUser = this.GetCurrentUser();
                var @readonly = (currentUser != null && currentUser.Type == ManagerServer.Model.UserType.Restricted);

                using (Div(@class: "card-form"))
                {
                    using (Div(@class: "form-group"))
                    {
                        using (Label()) Write(Strings.BusinessName);
                        InputText(@class: "form-control", style: "width: 400px", name: nameof(FormData.Name), value: Business, @readonly: @readonly, form: nameof(Strings.Rename));
                        if (@readonly) using (P(@class: "help-block")) Write(Strings.OnlyAdminsCanRenameBusinessName);
                        if (!string.IsNullOrWhiteSpace(Error)) using (P(@class: "pt-2 text-danger")) Write(Error);
                    }
                }

                if (!@readonly)
                {
                    using (Div(@class: "card-header"))
                    {
                        FormPrimaryButton(nameof(Strings.Rename));
                    }
                }
            }
        }

        public sealed class FormData
        {
            public string Name;
        }

        protected override async Task InnerPost()
        {
            this.EnsureCurrentUserNotRestricted();
            var form = await Request.ReadFormAsync();

            var name = form[nameof(FormData.Name)].ToString() ?? string.Empty;
            name = string.Join(string.Empty, name.Where(x => !System.IO.Path.GetInvalidFileNameChars().Contains(x)));

            await Task.Delay(1000);

            if (string.IsNullOrWhiteSpace(name))
            {
                Response.Redirect(new BusinessForm() { Business = Business }.ToUrl());
                return;
            }

            if (!ApplicationData.Businesses.Exists(Business))
            {
                Response.Redirect(new BusinessForm() { Business = Business }.ToUrl());
                return;
            }

            var sourceName = Business;

            if (await ApplicationData.Businesses.FileExists(name))
            {
                Response.Redirect(new BusinessForm() { Business = Business }.ToUrl());
                return;
            }

            if (!ApplicationData.Businesses.IsValidName(name))
            {
                Response.Redirect(new BusinessForm() { Business = Business }.ToUrl());
                return;
            }

            try
            {
                ApplicationData.Businesses.Rename(sourceName, name);
            }
            catch (IOException ex)
            {
                Response.Redirect(new BusinessForm() { Business = Business, Error = ex.Message }.ToUrl());
                return;
            }

            var users = await ApplicationData.Users.GetAllAsync();
            foreach (var e in users.Where(x => x.Businesses != null && x.Businesses.Contains(sourceName)))
            {
                var businesses = new List<string>(e.Businesses);
                businesses.Remove(sourceName);
                businesses.Add(name);
                e.Businesses = businesses.ToArray();
                await ApplicationData.Users.Save(e);
            }

            ApplicationData.Businesses.Refresh();
            Response.Redirect(new Start() { Business = name }.ToUrl());
        }
    }
}
