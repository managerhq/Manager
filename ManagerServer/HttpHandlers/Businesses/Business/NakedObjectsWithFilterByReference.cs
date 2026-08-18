using System.Collections;
using System.Linq;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    internal abstract class NakedObjectsWithFilterByReference<T> : NakedObjectsWithImageColumn<T> where T : ManagerServer.Model.Object, new()
    {
        [InheritedProtoMember(300)] public Guid? Reference;

        protected override void InnerGet4(Context context)
        {
            if (Reference.HasValue)
            {
                var rows = (T[])context.Get<Array>();

                var rows2 = new ArrayList();

                var database = ApplicationData.Businesses.Get(Business);

                var references = database.GetGeneralLedgerTransactions().GetTransactionsByForeignKey(Reference.Value);
                foreach (var e in rows.OfType<ManagerServer.Model.Transaction>())
                {
                    if (references.Contains(e.Key)) rows2.Add(e);
                }

                context.Set<Array>(rows2.ToArray(typeof(T)));
                context.Set(new Total() { Value = rows2.Count });
            }

            base.InnerGet4(context);
        }
    }
}