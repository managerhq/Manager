using System;
using ProtoBuf;
using ManagerServer.Model.Attributes;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("3c2d2934-27a8-4df3-9afd-890ebeb6fb4e")]
    public sealed class CustomColumns : Object
    {
        [Guide("List of columns and their visibility settings.")]
        [Guide("This configuration controls which columns appear in transaction lists and reports.")]
        [Guide("Users can customize their view by enabling or disabling specific columns.")]
        [Guide("Column preferences are saved per user and persist across sessions.")]
        [ProtoMember(2)] public CustomColumn[] Columns { get; set; }

        [ProtoContract]
        public sealed class CustomColumn
        {
            [Guide("The unique identifier of the column.")]
            [Guide("Each column type has a system-assigned GUID that identifies its purpose.")]
            [Guide("This key links to the column definition in the system schema.")]
            [ProtoMember(1)] public Guid Key { get; set; }
            [Guide("Whether this column is enabled and visible.")]
            [Guide("Check to show this column in lists and reports, uncheck to hide it.")]
            [Guide("Hidden columns can still be used for filtering and sorting operations.")]
            [ProtoMember(2)] public bool Enabled { get; set; }
        }
    }
}
