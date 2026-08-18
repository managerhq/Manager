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
    [Title(nameof(Strings.Business), nameof(Strings.Delete))]
    [Guide("This business has been removed from your active businesses list. Removed businesses are not permanently deleted and can be restored at any time.")]
    [Guide("When a business is removed, all its data remains intact but the business becomes inaccessible until restored. This provides a safe way to temporarily hide businesses you no longer need while preserving the option to recover them later.")]
    [Header("Available Actions")]
    [Guide("You have two options for managing removed businesses:")]
    [Guide("• `Restore Business` - Returns the business to your active businesses list, making it fully accessible again with all its data intact.")]
    [Guide("• `Cancel` - Returns to the removed businesses list without making any changes.")]
    internal sealed class RemovedBusiness : Template
    {
        [ProtoMember(1)] public string Name;
        [ProtoMember(2)] public string Error;

        protected override async Task InnerGet()
        {
            this.EnsureCurrentUserNotRestricted();
            using (Div(@class: "p-8 mx-auto max-w-prose"))
            {
                using (Div(@class: "card"))
                {
                    using (Div(@class: "card-body p-8"))
                    {
                        using (Div(@class: "flex flex-col space-y-4"))
                        {
                            using (Div(@class: "text-xl font-bold")) Write(Strings.RestoreBusiness);

                            Hr();

                            using (Div()) Write(Name);

                            Hr();

                            using (Div(@class: "flex gap-4 items-center"))
                            {
                                FormSuccessButton(nameof(Strings.RestoreBusiness));
                                using (A(href: new RemovedBusinesses().ToUrl(), @class: "btn")) Write(Strings.Cancel);
                            }

                            if (!string.IsNullOrWhiteSpace(Error))
                            {
                                using (Div(@class: "text-red-600 font-bold")) Write(Error);
                            }
                        }
                    }
                }
            }

        }

        protected override async Task InnerPost()
        {
            this.EnsureCurrentUserNotRestricted();

            var business = (await ApplicationData.GetRemovedBusinesses()).FirstOrDefault(x => x == Name);

            if (business == null)
            {
                Response.Redirect(new RemovedBusiness() { Name = Name, Error = "Business not found" }.ToUrl());
                return;
            }

            if (!await ApplicationData.RestoreBusiness(business))
            {
                Response.Redirect(new RemovedBusiness() { Name = Name, Error = "Business with the same name already found among list of businesses" }.ToUrl());
                return;
            }

            Response.Redirect(new Businesses().ToUrl());
        }
    }
}
