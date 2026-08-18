using System.Linq;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    internal abstract class NakedObjectsWithAbstractEditButtonColumn<T> : NakedObjectsWithAbstractBatchOperation<T>
    {
        [Center]
        [Default]
        [MinWidth]
        [Icon("fa-edit")]
        [Priority(-200)]
        [HideColumnIfAllEmpty]
        public virtual BusinessTemplate[] GetEdit(T[] rows)
        {
            return rows.Select(x => default(BusinessTemplate)).ToArray();
        }
    }
}
