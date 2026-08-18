using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.Reports.FixedAssetSummary
{
    [ProtoContract]
    internal sealed class GetFixedAssetSummary : GetObjectEndpoint<Model.FixedAssetSummary>
    {
    }
}
