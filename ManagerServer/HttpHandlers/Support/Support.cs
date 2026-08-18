using System;
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
        public static string Path;

        static Support()
        {
            if (System.IO.File.Exists(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Support.html"))) Path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Support.html");
        }

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
                            using (Div(@class: "text-xl font-bold")) Write(Strings.Support);
                            Hr();

                            using (Div())
                            {
                                try
                                {
                                    var html = System.IO.File.ReadAllText(Path);
                                    html = html.Replace("<!--EmailAddress-->", Strings.EmailAddress);
                                    html = html.Replace("<!--Website-->", Strings.Website);
                                    html = html.Replace("<!--Guides-->", Strings.Guides);
                                    html = html.Replace("<!--Advisors-->", Strings.Advisors);
                                    html = html.Replace("<!--Accountants-->", Strings.Accountants);
                                    html = html.Replace("<!--Forum-->", Strings.Forum);
                                    Write(html);
                                }
                                catch (Exception ex)
                                {
                                    Write(ex.Message);
                                }
                            }
                        }
                    }                    
                }
            }

            return Task.CompletedTask;
        }
    }
}