using System;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ProtoBuf;

namespace ManagerServer.HttpHandlers.Businesses
{
    [ProtoContract]
    [Title(nameof(Strings.NewerVersionRequired))]
    [Guide("When you attempt to open a Manager.io business file in an older version, the program might refuse to open it because of version incompatibility.")]
    [Guide("Newer versions of Manager.io can always open businesses created in older versions of the program, but not the other way around.")]
    [Guide("This means if you are transferring business data between computers or editions, make sure you are transferring to an equal or newer version only.")]
    [Guide("For example, importing a Manager.io business to `Cloud Edition` always works because `Cloud Edition` is always running on the latest version automatically.")]
    [Guide("However, if you are importing a business from `Cloud Edition` to `Desktop Edition`, you might need to upgrade to the latest version of `Desktop Edition` first by downloading it from https://www.manager.io/download")]
    [Guide("Similarly, if you are importing a business to `Server Edition`, you might need to upgrade to the latest version of `Server Edition` by downloading it from https://www.manager.io/server-edition")]
    internal sealed class NewerVersionRequired : Template
    {
        [ProtoMember(1)] public string Business;

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
                            using (Div(@class: "text-xl font-bold")) Write(Business);

                            Hr();

                            using (Div()) Write(Strings.Upgrade_necessary);

                            using (Div()) using (DefaultLink(new Businesses().ToUrl())) Write(Strings.Back);
                        }
                    }
                }
            }
            return Task.CompletedTask;
        }
    }
}
