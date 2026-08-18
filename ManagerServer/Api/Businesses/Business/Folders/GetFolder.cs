using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.Folders
{
    [ProtoContract]
    internal sealed class GetFolder : GetObjectEndpoint<Model.Folder>
    {
    }
}
