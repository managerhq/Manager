using System.Linq;
using ManagerServer.Globalization;
using System.Threading.Tasks;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Support
{
    [ProtoContract]
    [Title(nameof(Strings.Support))]
    [Guide("Manager.io support is available through multiple channels to help you get the most out of the software.")]
    [Header("Official Documentation")]
    [Guide("Read the official guides for step-by-step instructions on using Manager.io features. Visit https://www.manager.io/guides")]
    [Header("Professional Support")]
    [Guide("Connect with certified accountants who use Manager.io with their clients. Find experienced professionals in the accountants directory at https://www.manager.io/accountants")]
    [Header("Community Support")]
    [Guide("Join the community forum to connect with other Manager.io users. Share experiences, ask questions, and learn from the community at https://forum.manager.io")]
    internal sealed class Support : Template
    {
        protected override Task InnerGet()
        {
            // The navbar hides this tab for a whitelabel build, but the route stays reachable, so
            // the page has to withhold our own links as well.
            if (Whitelabel.IsEnabled) return Task.CompletedTask;

            using (Div(@class: "p-8 mx-auto max-w-prose"))
            {
                using (Div(@class: "card"))
                {
                    using (Div(@class: "card-body p-8"))
                    {
                        using (Div(@class: "flex flex-col space-y-4"))
                        {
                            using (Div(@class: "text-xl font-bold")) Write(Strings.Support);
                            Hr();

                            using (Div())
                            {
                                SupportLink(Strings.Website, "https://www.manager.io");
                                SupportLink(Strings.Guides, "https://www.manager.io/guides");
                                SupportLink(Strings.Advisors, "https://www.manager.cloud/advisors");
                                SupportLink(Strings.Accountants, "https://www.manager.cloud/accountants");
                                SupportLink(Strings.Forum, "https://forum.manager.io");
                            }
                        }
                    }                    
                }
            }

            return Task.CompletedTask;
        }

        // Was Assets/Support.html, a file that existed only so these links could be swapped for a
        // build that is not ours. The --whitelabel flag covers that now. `mb-5` is the 20px margin
        // the file carried as an inline style.
        private void SupportLink(string label, string url)
        {
            using (Div(@class: "font-bold")) Write(label);
            using (Div(@class: "mb-5")) using (A(href: url, target: "_blank")) Write(url);
        }
    }
}
