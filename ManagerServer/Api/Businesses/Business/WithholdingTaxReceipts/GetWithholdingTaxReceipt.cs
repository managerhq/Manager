using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.WithholdingTaxReceipts
{
    [ProtoContract]
    internal sealed class GetWithholdingTaxReceipt : GetObjectEndpoint<Model.WithholdingTaxReceipt>
    {
    }
}
