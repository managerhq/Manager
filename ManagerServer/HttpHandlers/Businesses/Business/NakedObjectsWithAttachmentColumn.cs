using System.Collections.Generic;
using System.Linq;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    internal abstract class NakedObjectsWithAttachmentColumn<T> : NakedObjectsWithCustomFields<T> where T : ManagerServer.Model.Object, new()
    {
        [Center]
        [Default]
        [MinWidth]
        [DoNotCopyToClipboard]
        [HideColumnIfAllEmpty]
        [Icon("fa-paperclip")]
        [Priority(-300)]
        [Guid("e86f12dd-2bfc-4eef-a7b0-71e4e9caeda9")]
        public bool[] GetAttachment(T[] rows)
        {
            var attachments = new HashSet<Guid>(ApplicationData.Businesses.Get(Business).OfType<ManagerServer.Model.Attachment>().Where(x => x.Object.HasValue).Select(x => x.Object.Value).Distinct());
            return rows.Select(x => attachments.Contains(x.Key)).ToArray();
        }

        protected override void OnColumnCell(Column column, object row)
        {
            if (IsAttachmentColumn(column))
            {
                var hasAttachment = (bool)column.GetValue(row);
                if (hasAttachment)
                {
                    I(@class: "fas fa-paperclip text-neutral-400", style: "font-size: 16px");
                }
                return;
            }
            base.OnColumnCell(column, row);
        }

        protected static bool IsAttachmentColumn(Column column)
        {
            if (column is Column<bool>)
            {
                if (column.Key == new Guid("e86f12dd-2bfc-4eef-a7b0-71e4e9caeda9"))
                {
                    return true;
                }
            }
            return false;
        }
    }
}