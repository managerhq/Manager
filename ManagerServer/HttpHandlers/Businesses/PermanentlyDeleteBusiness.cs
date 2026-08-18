/*
using ManagerServer;
using ManagerServer.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Attributes;
using ProtoBuf;

namespace ManagerServer.HttpHandlers.Businesses
{
    [ProtoContract]
    [Title(nameof(Strings.PermanentlyDelete))]
    [Guide("This action allows you to permanently delete a business that has been previously removed from your list of active businesses.")]
    [Guide("**Warning**: This action is irreversible. Once a business is permanently deleted, all its data will be lost forever and cannot be recovered.")]
    [Guide("You can only permanently delete a business after it has been in the `Removed Businesses` list for at least 30 days. This waiting period helps prevent accidental permanent deletion.")]
    [Guide("To permanently delete a business, click the `Permanently Delete` button. You will need to confirm this action as it cannot be undone.")]
    internal sealed class PermanentlyDeleteBusiness : Template
    {
        [ProtoMember(1)] public string Name;
        [ProtoMember(2)] public string Error;

        protected override Task InnerGet()
        {
            using (Div(@class: "p-8 mx-auto max-w-prose"))
            {
                using (Div(@class: "card"))
                {
                    using (Div(@class: "card-body p-8"))
                    {
                        using (Div(@class: "flex flex-col space-y-4"))
                        {
                            using (Div(@class: "text-xl font-bold")) Write(Strings.PermanentlyDelete);

                            Hr();

                            using (Div()) Write(Name);

                            using (Div(@class: "flex gap-4 items-center"))
                            {
                                FormDangerButton(nameof(Strings.PermanentlyDelete));
                                using (DefaultLink(new RemovedBusinesses().ToUrl())) Write(Strings.Cancel);
                            }

                            if (!string.IsNullOrWhiteSpace(Error))
                            {
                                using (Div(@class: "text-red-600 font-semibold")) Write(Error);
                            }
                        }
                    }
                }
            }

            return Task.CompletedTask;
        }

        protected override async Task InnerPost()
        {
            var business = (await ApplicationData.Businesses.GetRemovedBusinesses()).FirstOrDefault(x => x == Name);

            if (business == null)
            {
                Response.Redirect(new PermanentlyDeleteBusiness() { Name = Name, Error = "Business not found" }.ToUrl());
                return;
            }

            if (business.LastWriteTimeUtc.AddDays(30) > DateTime.UtcNow)
            {
                Response.Redirect(new PermanentlyDeleteBusiness() { Name = Name, Error = Strings.BusinessCanBePermanentlyDeletedAfter30DaysOnly }.ToUrl());
                return;
            }

            System.IO.File.Delete(business.FullName);

            Response.Redirect(new RemovedBusinesses().ToUrl());
        }
    }
}
*/