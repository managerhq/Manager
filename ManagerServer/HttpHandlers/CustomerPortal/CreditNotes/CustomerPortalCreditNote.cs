using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.HttpHandlers.CustomerPortal.CreditNotes
{
    [ProtoContract]
    class CustomerPortalCreditNote : View<ManagerServer.Api.Businesses.Business.CreditNotes.GetCreditNoteView>
    {
    }
}
