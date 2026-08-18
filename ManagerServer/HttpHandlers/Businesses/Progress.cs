using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.HttpHandlers.Businesses
{
    [ProtoContract]
    public sealed class Progress : HttpHandler
    {
        [ProtoMember(1)] public string Business;
        [ProtoMember(2)] public int Delay;

        public override Task Get()
        {
            if (string.IsNullOrWhiteSpace(Business))
            {
                Response.Headers["HX-Refresh"] = "true";
                return Task.CompletedTask;
            }

            Delay += 50;
            if (Delay > 1000) Delay = 1000;

            var progress = ApplicationData.Businesses.GetProgress(Business);

            if (progress != null)
            {
                using (Span(@class: "font-semibold", hxGet: this.ToUrl(), hxTrigger: $"load delay:{Delay}ms"))
                {
                    Write(progress);
                }
            }
            else
            {
                using (Span(@class: "font-semibold", hxGet: new Progress().ToUrl(), hxTrigger: $"load"))
                {
                    Write(progress);
                }
            }

            return Task.CompletedTask;
        }
    }
}
