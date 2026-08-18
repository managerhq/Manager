using ManagerServer.Orm;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Model.Obsolete.Obsolete89
{
    [Table("Blobs2", WithoutRowId = true)]
    public class Blob2
    {
        [PrimaryKey] public byte[] Hash { get; set; }
        public byte[] Content { get; set; }
    }
}
