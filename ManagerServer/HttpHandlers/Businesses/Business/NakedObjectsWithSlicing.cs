using System.Linq;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    internal abstract class NakedObjectsWithSlicing : NakedObjectsWithPagination
    {
        protected override void InnerGet4(Context context)
        {
            SkipAndTake(context);

            base.InnerGet4(context);
        }

        protected void SkipAndTake(Context context)
        {
            var rows = context.Get<Array>();

            if (rows == null) return;

            if (context.Get<Total>() == null) context.Set(new Total() { Value = rows.Length });

            var selection = ((object[])rows).Skip(Skip).Take(GetPageSize()).ToArray();

            var elementType = rows.GetType().GetElementType();
            rows = (object[])Array.CreateInstance(elementType, selection.Length);
            Array.Copy(selection, rows, rows.Length);
            context.Set(rows);
        }
    }
}