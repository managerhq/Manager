using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerServer.Api.Businesses.Business.DebitNotes
{
    [ProtoContract]
    internal sealed class GetDebitNote : GetObjectEndpoint<Model.DebitNote>
    {
    }
}
