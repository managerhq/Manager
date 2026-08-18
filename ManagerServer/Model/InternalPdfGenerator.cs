using System;
using ProtoBuf;
using ManagerServer.Model.Attributes;
using ManagerServer.Model.Enums;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [Singleton]
    [ProtoContract]
    [Guid("7e9fe6d7-d3a4-4456-981f-8112184b5517")]
    public sealed class InternalPdfGenerator : Object
    {
        [Guide("Check this box to enable the internal PDF generator for creating PDF documents directly.")]
        [ProtoMember(1)] public bool Enabled { get; set; }
        [Guide("Select the default page size for generated PDFs (e.g., A4, Letter, Legal).")]
        [ProtoMember(2), IfTrue(nameof(Enabled)), NoLabel, Prepend(nameof(Strings.PageSize))] public PageSize PageSize { get; set; }
    }
}
