using System.Linq;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    internal abstract class NakedObjectsWithFutureDateWarning : NakedObjects
    {
        public sealed class WarnIfFutureDateAttribute : Attribute { }

        protected override void OnColumnCell(Column column, object row)
        {
            if (column is Column<DateTime> || column is Column<DateTime?>)
            {
                if (column.Attributes.OfType<WarnIfFutureDateAttribute>().Any())
                {
                    var date = column.GetValue(row);
                    if (date is DateTime date2)
                    {
                        if (date2 > DateTime.Now)
                        {
                            using (Span(@class: "text-red-500"))
                            {
                                I(@class: "fas fa-clock");
                                Write(" ");
                                base.OnColumnCell(column, row);
                            }
                            return;
                        }
                    }
                }
            }

            base.OnColumnCell(column, row);
        }
    }
}