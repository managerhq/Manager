using System.Linq;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    internal abstract class NakedObjectsWithObscureMode : NakedObjectsWithColumnTotals
    {
        protected override void OnColumnCell(Column column, object row)
        {
            if (column.Attributes.OfType<SumAttribute>().Any())
            {
                using (Div(@class: "observer:blur-sm observer:hover:blur-none observer:hover:transition"))
                {
                    base.OnColumnCell(column, row);
                    return;
                }
            }
            base.OnColumnCell(column, row);
        }

        protected override void OnColumnFooterCell(Column column, Array rows)
        {
            if (column.Attributes.OfType<SumAttribute>().Any())
            {
                using (Div(@class: "observer:blur-sm observer:hover:blur-none observer:hover:transition"))
                {
                    base.OnColumnFooterCell(column, rows);
                    return;
                }
            }
            base.OnColumnFooterCell(column, rows);
        }
    }
}