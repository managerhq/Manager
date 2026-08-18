using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.Projects
{
    [ProtoContract]
    internal sealed class GetProject : GetObjectEndpoint<Model.Project>
    {
    }
}
