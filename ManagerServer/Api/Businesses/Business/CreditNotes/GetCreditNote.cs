using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.CreditNotes
{
    [ProtoContract]
    internal sealed class GetCreditNote : GetObjectEndpoint<Model.CreditNote>
    {
    }
}
