using System.Collections;
using System.Linq;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    internal abstract class NakedObjectsWithViewRedirects : NakedObjectsWithRunningTotals
    {
        [InheritedProtoMember(222)] public int? Redirect;

        protected override void InnerGet4(Context context)
        {
            var columns = context.Get<Column[]>();
            var viewColumn = columns.FirstOrDefault(x => x.Attributes.OfType<ViewRedirectAttribute>().Any());            

            if (viewColumn != null)
            {
                if (Redirect.HasValue)
                {
                    var rows = context.Get<Array>();
                    if (rows.Length > Redirect.Value)
                    {
                        var row = rows.GetValue(Redirect.Value);
                        if (row != null)
                        {
                            viewColumn.EnsureCells(new ArrayList(new[] { row }).ToArray(rows.GetType().GetElementType()));

                            var baseView3 = viewColumn.GetValue(row) as BaseView3;
                            if (baseView3 != null)
                            {
                                baseView3.MaxPosition = rows.Length - 1;
                                baseView3.Position = Redirect.Value;
                                this.Redirect = null;
                                baseView3.Referrer = this.ToUrl();
                                using (Script()) Write("window.location.href = '" + baseView3.ToUrl() + @"';");
                                return;
                            }
                        }
                    }

                    this.Redirect = null;
                    using (Script()) Write("window.location.href = '" + this.ToUrl() + @"';");
                    return;
                }
            }

            base.InnerGet4(context);
        }

        protected override void OnAfterHeader(Context context)
        {
            var columns = context.Get<Column[]>();
            var viewColumn = columns.FirstOrDefault(x => x.Attributes.OfType<ViewRedirectAttribute>().Any());

            if (viewColumn != null)
            {
                var rows = context.Get<Array>();
                viewColumn.EnsureCells(rows);

                var total = context.Get<Total>();
                if (total == null)
                {
                    total = new Total() { Value = rows.Length };
                    context.Set(total);
                }

                for (int i = 0; i < rows.Length; i++)
                {
                    var value = viewColumn.GetValue(rows.GetValue(i));
                    if (value is BaseView3 baseView3)
                    {
                        baseView3.MaxPosition = total.Value - 1;
                        baseView3.Position = Skip + i;
                    }
                }
            }

            base.OnAfterHeader(context);
        }

        protected class ViewRedirectAttribute : Attribute { }
    }
}