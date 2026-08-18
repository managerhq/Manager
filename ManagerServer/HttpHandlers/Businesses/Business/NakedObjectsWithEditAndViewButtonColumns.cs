using System.Linq;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    internal abstract class NakedObjectsWithEditAndViewButtonColumns<T> : NakedObjectsWithTimestampColumn<T> where T : ManagerServer.Model.Object, new()
    {
        private static Type formType = typeof(Program).Assembly.GetTypes().SingleOrDefault(x => x.IsSubclassOf(typeof(NakedVueForm<T>)));

        public override BusinessTemplate[] GetEdit(T[] rows)
        {
            if (formType != null)
            {
                var referrer = this.ToUrl();
                return rows.Select(x =>
                {
                    var editHandler = (NakedVueForm<T>)Activator.CreateInstance(formType);
                    editHandler.HttpContext = HttpContext;
                    editHandler.Key = x.Key;
                    editHandler.Business = Business;
                    editHandler.Referrer = referrer;
                    return editHandler;
                }).ToArray();
            }
            return base.GetEdit(rows);
        }

        private static Type defaultView = Assembly.GetHttpHandlerTypeByCamelCaseKey($"{typeof(T).Name}View");

        public override BusinessTemplate[] GetView(T[] rows)
        {
            var referrer = this.ToUrl();
            if (defaultView != null && defaultView.IsSubclassOf(typeof(BaseView3)))
            {
                return rows.Select(x =>
                {
                    var viewHandler = (BaseView3)Activator.CreateInstance(defaultView);
                    viewHandler.HttpContext = HttpContext;
                    viewHandler.Key = x.Key;
                    viewHandler.Business = Business;
                    viewHandler.Referrer = referrer;
                    return viewHandler;
                }).ToArray();
            }
            else
            {
                return base.GetView(rows);
            }
        }
    }
}