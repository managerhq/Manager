using ManagerServer.Helpers;
using System.Linq;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    internal abstract class NakedObjectsWithAutomaticRows<T> : NakedObjectsWithFilterByReference<T> where T : ManagerServer.Model.Object, new()
    {
        internal override bool IsEmpty(TabsExtensions.Item[] tabs)
        {
            return !ApplicationData.Businesses.Get(Business).OfType<T>().Any();
        }

        protected virtual T[] OnGetRows(T[] rows)
        {
            return rows;
        }

        protected override void InnerGet4(Context context)
        {
            var rows = ApplicationData.Businesses.Get(Business).OfType<T>();
            rows = OnGetRows(rows);
            context.Set<Array>(rows);
            context.Set(new Total() { Value = rows.Length });

            base.InnerGet4(context);
        }
    }
}