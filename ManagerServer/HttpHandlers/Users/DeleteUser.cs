using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HttpFramework;
using ManagerServer.Helpers;
using ManagerServer.Globalization;
using System.Threading.Tasks;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Users
{
    [ProtoContract]
    [Title(nameof(Strings.User), nameof(Strings.Delete))]
    [Guide("To delete a user, go to the `Users` tab, click on the user you wish to delete, then click the `Delete` button.")]
    [Guide("Only users with the `Administrator` role can delete other users.")]
    [Guide("An administrator can delete other administrators but cannot delete their own user account.")]
    internal sealed class DeleteUser : Template
    {
        [ProtoMember(1)] public string Username;

        protected override async Task InnerGet()
        {
            var currentUser = this.GetCurrentUser();
            if (currentUser == null || currentUser.Type != ManagerServer.Model.UserType.Administrator)
            {
                Response.Redirect(new Users().ToUrl());
                return;
            }

            var user = await ApplicationData.Users.GetByUsernameAsync(Username);
            if (user == null)
            {
                Response.Redirect(new Users().ToUrl());
                return;
            }
            if (user.Businesses == null) user.Businesses = new string[0];

            using (Div(@class: "p-8 mx-auto max-w-prose"))
            {
                using (Div(@class: "card"))
                {
                    using (Div(@class: "card-body p-8"))
                    {
                        using (Div(@class: "flex flex-col space-y-4"))
                        {
                            using (Div(@class: "text-xl font-bold")) Write(Strings.User);

                            Hr();

                            using (Div(@class: "font-semibold")) Write(Strings.Are_you_sure);

                            Hr();

                            using (Div(@class: "flex gap-4 items-center"))
                            {
                                FormDangerButton(nameof(Strings.Delete));
                                using (A(href: new Users().ToUrl(), @class: "btn")) Write(Strings.Cancel);
                            }
                        }
                    }
                }
            }

            return;
        }

        protected override async Task InnerPost()
        {
            if (this.GetCurrentUser().Type != ManagerServer.Model.UserType.Administrator)
            {
                Response.Redirect(new Users().ToUrl());
                return;
            }

            await ApplicationData.Users.Delete(Username);
            Response.Redirect(new Users().ToUrl());
        }
    }
}