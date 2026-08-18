using ManagerServer;
using ManagerServer.Globalization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Attributes;
using ProtoBuf;

namespace ManagerServer.HttpHandlers.Businesses
{
    [ProtoContract]
    [Title(nameof(Strings.RemovedBusinesses))]
    [Guide("The `Removed Businesses` screen displays a list of all businesses that have been removed but not permanently deleted.")]
    [Guide("Removed businesses are not accessible for normal operations but can be restored if needed. They continue to occupy storage space until permanently deleted.")]
    [Guide("From this screen, you can either restore a removed business to make it active again, or permanently delete it to free up storage space.")]
    [Guide("To restore a business, click on its name in the list. To permanently delete a business and free up storage space, click the `X` button next to the business name.")]
    internal sealed class RemovedBusinesses : Template
    {
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
                            using (Div(@class: "text-xl font-bold")) Write(Strings.RemovedBusinesses);

                            Hr();

                            var removedBusinesses = await ApplicationData.GetRemovedBusinesses();

                            if (removedBusinesses.Length == 0)
                            {
                                using (Div(@class: "text-neutral-300 text-xl font-bold text-center p-4")) Write(Strings.Empty);
                            }
                            else
                            {
                                using (Table())
                                {
                                    foreach (var e in await ApplicationData.GetRemovedBusinesses())
                                    {
                                        using (Tr())
                                        {
                                            using (Td(@class: "py-2"))
                                            {
                                                using (A(href: new RemovedBusiness() { Name = e }.ToUrl())) Write(e);
                                            }
                                            /*
                                            using (Td(@class: "p-2 text-right"))
                                            {
                                                using (A(href: new PermanentlyDeleteBusiness() { Name = e }.ToUrl()))
                                                {
                                                    I(@class: "fa fa-xmark text-neutral-200 hover:text-neutral-400");
                                                }
                                            }
                                            */
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

        }
    }
}
