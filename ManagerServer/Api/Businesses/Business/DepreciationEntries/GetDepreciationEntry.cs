using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.DepreciationEntries
{
    [ProtoContract]
    internal sealed class GetDepreciationEntry : GetObjectEndpoint<Model.DepreciationEntry>
    {
    }
}
