using System;

namespace ManagerServer.Model.Master
{
    public static class AccountKeys
    {
        // Balance sheet accounts
        public static readonly Guid Suspense = new Guid("11211c9e-0988-4d16-8bf2-fa39487123aa");
        public static readonly Guid CashAtBank = new Guid("6d4af96a-0959-4bb2-9160-fa825ec67c43");
        public static readonly Guid CashOnHand = new Guid("a084e4be-981b-4b7e-8331-56b0eb3a6729");
        public static readonly Guid RetainedEarnings = new Guid("74dfd025-d68e-4a99-9c78-5d43e17c0e09");
        public static readonly Guid AccountsPayable = new Guid("dac7ba37-0ccd-45e5-906e-548e6c50df37");
        public static readonly Guid AccountsReceivable = new Guid("d1489e95-bb28-4f5d-b42e-67d3291b3893");
        public static readonly Guid InventoryOnHand = new Guid("0fb45a62-fc42-43a8-a776-782e8b5ffc96");
        public static readonly Guid FixedAssets = new Guid("4a0e8917-fee2-4033-9161-48dd513fdb73");
        public static readonly Guid FixedAssetsAccumulatedDepreciation = new Guid("f813a6c8-1ead-46bd-8911-f12714be193c");
        public static readonly Guid IntangibleAssets = new Guid("31d369e3-32c7-4bd2-bb83-9c1c58010c1a");
        public static readonly Guid IntangibleAssetsAccumulatedAmortization = new Guid("aa12d048-bfbd-47dc-a5b8-03e35c417996");
        public static readonly Guid CapitalAccounts = new Guid("054dfae1-c34a-475e-abde-49e0385ffc9a");
        public static readonly Guid BillableTimeUnbilled = new Guid("9f3c0a1a-dd0b-4b5c-ba50-3e4939a0e90c");
        public static readonly Guid EmployeeClearingAccount = new Guid("650a36fe-801f-4031-8d5b-ab422d061fca");
        public static readonly Guid ExpenseClaims = new Guid("f728124f-c6b6-4dad-82c5-22fc0d8d0571");
        public static readonly Guid BillableExpensesAssetAccount = new Guid("059dbfb9-1c80-4043-887f-0fc441099fe0");
        public static readonly Guid WithholdingTaxReceivable = new Guid("c66de1bf-6f63-4bc8-9452-0b019e41c47f");
        public static readonly Guid WithholdingTax = new Guid("8f75a810-abd0-4d89-a6a2-66c9003a60e2");
        public static readonly Guid SpecialAccounts = new Guid("ef49facb-203b-4b45-aebd-99af4645700b");
        public static readonly Guid ProductionInProgress = new Guid("30a1b83c-68a8-4f2c-ae70-25b0acc2d12a");
        public static readonly Guid TaxPayable = new Guid("30c697fa-4196-438a-ab5a-1957478034b1");

        // Profit & Loss accounts
        public static readonly Guid InventorySales = new Guid("ea44f579-9548-4954-baf0-48538aceff1e");
        public static readonly Guid InventoryPurchases = new Guid("aa80b662-3642-4c08-b328-2fccf132ceb1");
        public static readonly Guid BillableExpensesCost = new Guid("234d263d-cf0e-4e3e-85ca-ef899016e58a");
        public static readonly Guid FixedAssetDepreciation = new Guid("fb6fdbfd-b39f-4674-8928-10c2bdd87e58");
        public static readonly Guid FixedAssetsLossOnDisposal = new Guid("428ea9ba-4679-4568-b05b-7fcf62504893");
        public static readonly Guid IntangibleAssetsAmortization = new Guid("83d56444-fed8-4de8-8e58-e325a370ae44");
        public static readonly Guid IntangibleAssetsGainLossOnDisposal = new Guid("43e14984-202e-4e9e-b843-d417dddcd3d2");
        public static readonly Guid BillableTimeMovement = new Guid("03d41bd1-8dd4-4ce3-9b82-5ce17a40171a");
        public static readonly Guid BillableTimeInvoiced = new Guid("8d86390b-b90f-4cf6-862b-9b4050449b91");
        public static readonly Guid CurrencyGainLoss = new Guid("635ddd64-1176-4d35-b1c2-2d7d3bb12bb6");
        public static readonly Guid RoundingExpense = new Guid("2aa99eac-faca-4017-a157-edbbbcb160ac");
        public static readonly Guid BillableExpensesInvoiced = new Guid("1ae41f36-c83a-428c-be23-a99714110807");
        public static readonly Guid LatePaymentFees = new Guid("841b2acb-8bb5-4742-864e-4226fa421f44");

        // Obsolete
        internal static readonly Guid Obsolete_CustomerCredits = new Guid("5c888cc8-3553-4548-9afb-5b5c6202a44c");
        internal static readonly Guid Obsolete_SupplierCredits = new Guid("3bed9439-7555-4824-9dd5-36d9514c5158");
        internal static readonly Guid Obsolete_CashAtBank = new Guid("204e8601-b911-44c6-b026-4e6a43b08bdf");
        internal static readonly Guid Obsolete_CashOnHand = new Guid("fbb24910-d872-4f19-9268-9f1f5bdcc18f");
    }
}