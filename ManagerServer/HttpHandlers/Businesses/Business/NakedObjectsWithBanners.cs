using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    internal abstract class NakedObjectsWithBanners : NakedObjectsWithBatchOperations
    {
        protected override void InnerGet4(Context context)
        {
            if (this.GetType().Name == this.GetType().Namespace.Split('.').Last())
            {
                foreach (var e in this.GetType().Assembly.GetTypes().Where(x => this.GetType().Namespace == x.Namespace && x.IsSubclassOf(typeof(NakedObjectsWithBanners))))
                {
                    var contextTable = (NakedObjectsWithBanners)Activator.CreateInstance(e);
                    contextTable.Business = Business;
                    contextTable.HttpContext = HttpContext;
                    var count = contextTable.GetContextCount();
                    if (count > 0)
                    {
                        contextTable.Referrer = this.ToUrl();
                        using (A(href: contextTable.ToUrl(), @class: "border border-neutral-300 text-neutral-500 p-4 bg-yellow-100 rounded-lg block hover:no-underline flex gap-4 mb-4 font-semibold"))
                        {
                            I(@class: $"fas fa-square-plus", style: "font-size: 16px");
                            using (Span()) Write(contextTable.GetTitle());
                        }
                    }
                }
            }

            base.InnerGet4(context);
        }

        public virtual int GetContextCount()
        {
            return 0;
        }
    }
}
