using System.Linq;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    internal abstract class NakedObjectsWithAbstractViewButtonColumn<T> : NakedObjectsWithAbstractEditButtonColumn<T>
    {
        [Center]
        [Default]
        [MinWidth]
        [ViewRedirect]
        [Icon("fa-eye")]
        [Priority(-100)]
        [HideColumnIfAllEmpty]
        public virtual BusinessTemplate[] GetView(T[] rows)
        {
            return rows.Select(x => default(BusinessTemplate)).ToArray();
        }
    }
}
