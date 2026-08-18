using System.Collections.Generic;
using System.Linq;
using ManagerServer;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    internal abstract class NakedObjectsWithTimestampColumn<T> : NakedObjectsWithAttachmentColumn<T> where T : ManagerServer.Model.Object, new()
    {
        [Center]
        [MinWidth]
        [WhitespaceNoWrap]
        [Priority(9999)]
        [Guid("1b4d595d-ff7c-4f67-bfdb-7cf392326a4e")]
        public DateTime[] GetTimestamp(T[] rows)
        {
            return rows.Select(x => new DateTime(x.Timestamp, DateTimeKind.Utc)).ToArray();
        }
    }
}