using ManagerServer.Model.Obsolete.Obsolete76;
using System;
using System.Collections.Generic;

namespace ManagerServer.Model
{
    public abstract class ITransactionLine
    {
        public virtual Guid? GetItem() => null;
        public virtual Guid? GetAccount() => null;
        public virtual Guid? GetBankOrCashAccount() => null;
        public virtual Guid? GetAccountsReceivableCustomer() => null;
        public virtual Guid? GetAccountsReceivableSalesInvoice() => null;
        public virtual Guid? GetCapitalAccount() => null;
        public virtual Guid? GetSubAccount() => null;
        public virtual Guid? GetBillableExpenseCustomer() => null;
        public virtual Guid? GetBillableExpenseSalesInvoice() => null;
        public virtual Guid? GetAccountsPayableSupplier() => null;
        public virtual Guid? GetAccountsPayablePurchaseInvoice() => null;
        public virtual Guid? GetWithholdingTaxPayableSupplier() => null;
        public virtual Guid? GetEmployee() => null;
        public virtual Guid? GetSpecialAccount() => null;
        public virtual Guid? GetFixedAsset() => null;
        public virtual Guid? GetIntangibleAsset() => null;
        public virtual Guid? GetInvestment() => null;
        public virtual Guid? GetExpenseClaimPayer() => null;
        public virtual Guid? GetTaxCode() => null;
        public virtual Guid? GetDivision() => null;
        public virtual Guid? GetInterAccountTransferAccount() => null;
        public virtual Type GetType2() => GetType();

        protected virtual string GetLineDescription() => null;
        protected virtual decimal? GetQty() => null;
        protected virtual decimal? GetUnitPrice() => null;
        protected virtual decimal? GetDiscountPercentage() => null;
        protected virtual decimal? GetDiscountAmount() => null;
        protected virtual decimal? GetAmount() => null;
        protected virtual Guid? GetProject() => null;

        public Guid? GetProject(Transaction o) => o.HasLineProject() ? GetProject() : null;
        public string GetLineDescription(Transaction o) => o.GetHasLineDescription() ? GetLineDescription() : null;
        public decimal? GetQty(Transaction o) => o.HasLineQty() ? GetQty() : null;
        public decimal? GetAmount(Transaction o) => !(o.HasLineQty() && o.HasLineUnitPrice()) ? GetAmount() : null;
        public decimal? GetUnitPrice(Transaction o) => o.HasLineQty() && o.HasLineUnitPrice() ? GetUnitPrice() : null;
        public decimal? GetDiscountPercentage(Transaction o) => o.GetLineDicountType() == Enums.DiscountType.Percentage ? GetDiscountPercentage() : null;
        public decimal? GetDiscountAmount(Transaction o) => o.GetLineDicountType() == Enums.DiscountType.ExactAmount ? GetDiscountAmount() : null;
        public virtual decimal? GetDebit() => null;
        public virtual decimal? GetCredit() => null;
        public virtual decimal? GetProposedAccountAmount() => null;
        //public virtual decimal? GetProposedCostOfGoodsSoldAmount() => null;
        public virtual Dictionary<Guid, string> GetCustomFields() => null;
        public virtual CustomFields GetCustomFields2() => null;
        public virtual decimal? GetInvestmentAverageCost() => null;

        public string GetDescriptionOrNull(Transaction o)
        {
            if (!string.IsNullOrWhiteSpace(GetLineDescription(o))) return GetLineDescription(o);
            return null;
        }

        public decimal GetLineTotal(Transaction o)
        {
            var lineTotal = 0m;
            try
            {
                if (GetDebit().HasValue) lineTotal += GetDebit().Value;
                if (GetCredit().HasValue) lineTotal -= GetCredit().Value;
                if (GetAmount(o).HasValue) lineTotal += GetAmount(o).Value;
                if (GetUnitPrice(o).HasValue)
                {
                    if (GetQty(o).HasValue) lineTotal += (GetUnitPrice(o).Value * GetQty(o).Value);
                    else lineTotal += GetUnitPrice(o).Value;
                }
                return lineTotal;
            }
            catch (OverflowException)
            {
                return 0m;
            }
        }

        public bool HasDiscount(Transaction o)
        {
            if ((GetDiscountAmount(o) ?? 0m) != 0m) return true;
            if ((GetDiscountPercentage(o) ?? 0m) != 0m) return true;
            return false;
        }

        public bool HasDebitCreditAmountOrUnitPrice(Transaction o)
        {
            if (GetDebit().HasValue && GetDebit().Value != 0m) return true;
            if (GetCredit().HasValue && GetCredit().Value != 0m) return true;
            if (GetAmount().HasValue && GetAmount().Value != 0m) return true;
            if (GetUnitPrice(o).HasValue && GetUnitPrice(o).Value != 0m) return true;
            return false;
        }
    }
}
