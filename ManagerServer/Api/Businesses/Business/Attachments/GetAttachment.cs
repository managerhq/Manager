using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.Attachments
{
    [ProtoContract]
    internal sealed class GetAttachment : GetObjectEndpoint<Model.Attachment>
    {
    }
}
