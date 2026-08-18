using System;
using System.Collections.Generic;
using ProtoBuf;
using ManagerServer.Model.Enums;

namespace ManagerServer.Model.Obsolete.Obsolete76
{
    [ProtoContract]
    public sealed class TransactionLine
    {
        [ProtoMember(9)] public Guid? Item;
        [ProtoMember(2)] public Guid? Account;
        [ProtoMember(22)] public Guid? MemberAccount;
        [ProtoMember(37)] public Guid? BillableExpenseCustomer;
        [ProtoMember(45)] public Guid? BillableExpenseSalesInvoice;
        [ProtoMember(39)] public Guid? Invoice;
        [ProtoMember(1)] public string Description;
        [ProtoMember(6)] public decimal? Qty;
        [ProtoMember(12)] public decimal? Amount;
        [ProtoMember(23)] public decimal? Discount;
        [ProtoMember(42)] public decimal? DiscountAmount;
        [ProtoMember(14)] public decimal? Debit;
        [ProtoMember(15)] public decimal? Credit;
        [ProtoMember(5)] public Guid? TaxCode;
        [ProtoMember(24)] public Guid? TrackingCode;
        [ProtoMember(32)] public decimal? ProposedAccountAmount;
        [ProtoMember(41)] public Dictionary<Guid, string> CustomFields;
        [ProtoMember(47)] public Guid? WithholdingTaxPayableSupplier;
        [ProtoMember(48)] public Guid? Project;

        [ProtoMember(40)] public Guid? Obsolete_Disbursement;
        [ProtoMember(38)] public Guid? Obsolete_Account;
        [ProtoMember(7)] public Guid? Obsolete_SalesInvoice;
        [ProtoMember(8)] public Guid? Obsolete_PurchaseInvoice;
        [ProtoMember(26)] public Guid? Obsolete_Employee;
        [ProtoMember(27)] public Guid? Obsolete_ExpenseClaimPayer;
        [ProtoMember(35)] public Guid? Obsolete_IntangibleAsset;
        [ProtoMember(17)] public Guid? Obsolete_InventoryItem;
        [ProtoMember(18)] public Guid? Obsolete_Customer;
        [ProtoMember(19)] public Guid? Obsolete_Supplier;
        [ProtoMember(20)] public Guid? Obsolete_FixedAsset;
        [ProtoMember(21)] public Guid? Obsolete_Member;
        [ProtoMember(36)] public Guid? Obsolete_Item;
        [ProtoMember(10)] public Guid? Obsolete_PurchaseInvoiceItem;
        [ProtoMember(16)] public int? Obsolete_Discount;
        [ProtoMember(25)] public string Obsolete_EquityReason;
        [ProtoMember(28)] public decimal? Obsolete_CurrencyAmount;
        [ProtoMember(30)] public DisbursementStatus Obsolete_DisbursementStatus;
        [ProtoMember(31)] public Guid? Obsolete_DisbursementSalesInvoice;
        [ProtoMember(3)] public DateTime? Obsolete_DisbursementWriteOffDate;
        [ProtoMember(33)] public Guid? Obsolete_Cheque;
        [ProtoMember(34)] public Guid? Obsolete_BankDeposit;
        [ProtoMember(11)] public Guid? Obsolete_BankAccount;
        [ProtoMember(13)] public Guid? Obsolete_CashAccount;
        [ProtoMember(43)] public decimal? Obsolete_Amount;
        [ProtoMember(44)] public Guid? Obsolete_Invoice;
        [ProtoMember(29)] public Guid? Obsolete_BillableExpense;
        [ProtoMember(46)] public Guid? Obsolete_CustomerOrSupplier;

        public TransactionLine Clone(bool resetDiscount = false)
        {
            var o = (TransactionLine)this.MemberwiseClone();
            if (resetDiscount)
            {
                o.Discount = null;
                o.DiscountAmount = null;
            }
            return o;
        }
    }
}