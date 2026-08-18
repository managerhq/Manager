using System.Threading.Tasks;
using ManagerServer.Globalization;
using ManagerServer.HttpHandlers.Businesses.Business;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses
{
    [ProtoContract]
    [Title(nameof(Strings.Error))]
    [Guide("This error occurs when there is insufficient memory available to load or process the business file you are trying to access.")]
    [Guide("This typically happens when your system is running low on available RAM due to multiple applications running simultaneously or when working with very large business files.")]
    [Header("Immediate Solutions")]
    [Guide("Close any unnecessary applications or browser tabs to free up memory and try loading the business file again.")]
    [Guide("If another business file is currently open in Manager, it may be automatically unloaded to make room for the file you're trying to access.")]
    [Header("Long-term Solutions")]
    [Guide("Consider upgrading your system's memory (RAM) if you frequently encounter this error, especially when working with large business files or multiple businesses simultaneously.")]
    [Guide("For optimal performance, ensure your system meets the recommended hardware requirements for running Manager with your typical workload.")]
    internal sealed class NotEnoughMemory : Template
    {
        [ProtoMember(1)] public string Business;

        protected override Task InnerGet()
        {
            if (!Response.HasStarted)
            {
                // This is to notify Manager Cloud proxy
                Response.Headers["x-manager-exception"] = typeof(OutOfMemoryException).Name;
            }

            GC.Collect();

            using (Div(@class: "p-8 mx-auto max-w-prose"))
            {
                using (Div(@class: "card"))
                {
                    using (Div(@class: "card-body p-8"))
                    {
                        using (Div(@class: "flex flex-col space-y-4"))
                        {
                            using (Div(@class: "flex flex-col space-y-4"))
                            {
                                using (Div(@class: "text-xl font-bold")) Write(Business);

                                Hr();

                                using (Div()) Write("The business file could not be loaded due to memory limitations.");

                                if (ApplicationData.Businesses.GetStaleBusiness() != null)
                                {
                                    using (Div(@class: "mt-4")) Write("Another business will be unloaded to free up memory and allow this one to load.");

                                    using (Div(@class: "flex gap-2 items-center"))
                                    {
                                        FormPrimaryButton(nameof(Strings.Next));
                                        using (DefaultLink(new Businesses().ToUrl())) Write(Strings.Cancel);
                                    }
                                }
                                else
                                {
                                    using (Div(@class: "flex gap-2 items-center"))
                                    {
                                        using (DefaultLink(new Businesses().ToUrl())) Write(Strings.GoBack);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return Task.CompletedTask;
        }

        protected override Task InnerPost()
        {
            ApplicationData.Businesses.Unload(Business);
            Response.Redirect(new Start() { Business = Business }.ToUrl());

            return Task.CompletedTask;
        }
    }
}
