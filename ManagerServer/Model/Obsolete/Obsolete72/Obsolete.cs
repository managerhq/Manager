using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
using ManagerServer.Model.Attributes;

namespace ManagerServer.Model.Obsolete.Obsolete72
{
    [ProtoContract]
    [Guid("2ed8987e-6e9b-496d-bacb-ee9d62a3a7ba")]
    public sealed class Obsolete : Object
    {
        [ProtoMember(1)] public ManagerServer.Model.AmortizationEntry AmortizationEntry;
        [ProtoMember(2)] public ManagerServer.Model.BillableTime BillableTime;
        [ProtoMember(3)] public ManagerServer.Model.CreditNote CreditNote;
        [ProtoMember(4)] public ManagerServer.Model.DebitNote DebitNote;
        [ProtoMember(5)] public ManagerServer.Model.DepreciationEntry DepreciationEntry;
        [ProtoMember(6)] public ManagerServer.Model.ExpenseClaim ExpenseClaim;
        [ProtoMember(7)] public ManagerServer.Model.InterAccountTransfer InterAccountTransfer;
        [ProtoMember(8)] public ManagerServer.Model.InventoryTransfer InventoryTransfer;
        [ProtoMember(9)] public ManagerServer.Model.InventoryWriteOff InventoryWriteOff;
        [ProtoMember(10)] public ManagerServer.Model.JournalEntry JournalEntry;
        [ProtoMember(11)] public ManagerServer.Model.LatePaymentFee LatePaymentFee;
        [ProtoMember(12)] public ManagerServer.Model.Payslip Payslip;
        [ProtoMember(13)] public ManagerServer.Model.WithholdingTaxReceipt WithholdingTaxReceipt;
        [ProtoMember(14)] public ManagerServer.Model.Payment Payment;
        [ProtoMember(15)] public ManagerServer.Model.Receipt Receipt;
    }
}
