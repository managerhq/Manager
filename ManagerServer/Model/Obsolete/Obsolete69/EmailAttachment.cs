using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Orm;

namespace ManagerServer.Model.Obsolete.Obsolete69
{
    [Table("EmailAttachments")]
    public class EmailAttachment
    {
        [PrimaryKey] public Guid Key { get; set; }
        [Indexed] public Guid Email { get; set; }
        public string Filename { get; set; }
        public string ContentType { get; set; }
        public Guid Blob { get; set; }
    }
}
