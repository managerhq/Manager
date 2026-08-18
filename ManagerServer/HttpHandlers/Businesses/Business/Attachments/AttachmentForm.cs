using System;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ProtoBuf;

namespace ManagerServer.HttpHandlers.Businesses.Business.Attachments
{
    [ProtoContract]
    [Title(nameof(Strings.Attachment), nameof(Strings.Edit))]
    [Guide("The `Edit Attachment` form allows you to rename an existing attachment without re-uploading the file.")]
    [Guide("This is useful when you need to correct a filename or make it more descriptive for easier identification.")]
    [Guide("The form contains the following field:")]
    [Fields(typeof(ManagerServer.Model.Attachment))]
    internal sealed class AttachmentForm : NakedVueForm<ManagerServer.Model.Attachment>
    {
    }
}
