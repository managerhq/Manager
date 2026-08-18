using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.FixedAssets
{
    [ProtoContract]
    internal sealed class GetFixedAsset : GetObjectEndpoint<Model.FixedAsset>
    {
    }
}
