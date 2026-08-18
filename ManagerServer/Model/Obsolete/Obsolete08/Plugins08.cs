using System;
using ManagerServer.Model.Enums;

namespace ManagerServer.Model.Obsolete.Obsolete08
{
    internal static class Plugins08
    {
        // Universal
        public static Guid BankAccounts = new Guid("072e61fe-7308-4579-8780-9bf8174a346c");
        public static Guid CashAccounts = new Guid("2f6a94f8-5f40-4731-9dd8-9b63bffd2c7e");
        public static Guid SalesInvoices = new Guid("b34db5da-c3aa-42d4-9934-df1a3e35b9c2");
        public static Guid PurchaseInvoices = new Guid("800026eb-6f80-42e6-b9e9-cb959911f90b");
        public static Guid SalesQuotes = new Guid("266ab882-46b6-44e0-87a5-c070ebe1109e");
        public static Guid SalesInvoiceItems = new Guid("b51254f4-2819-4040-a2c3-c2e4ad4b6524");
        public static Guid ReportingOnCashBasis = new Guid("66a269e5-c42f-42f2-a95a-150f2bd20ba6");
        public static Guid ExpenseClaims = new Guid("821128b6-ee43-4418-8105-76f98a4f3ee8");
        public static Guid TaxCodes = new Guid("2a04c37f-2f61-40c0-a2f8-b2bf9e01ebe6");
        public static Guid SampleChartOfAccounts = new Guid("8086fe06-2a04-4272-9f8d-0868bf6bde8a");
        public static Guid InvoiceLogo = new Guid("3a7017cd-6d57-40c1-911e-8e7bd52768c7");
        public static Guid CustomerStatements = new Guid("1fec4889-375b-4ea1-96d1-dcf1254b7e27");
        public static Guid TaxExclusivePricingForInvoices = new Guid("37b0a31e-b176-470f-9df0-7d7e963d06dd");
        public static Guid PurchaseOrderNumberFieldToInvoices = new Guid("068c72a4-0004-4011-a0af-311f9575ac64");
        public static Guid GeneralLedgerSummary = new Guid("774c0d8d-bcfe-4f1f-8f45-9598e0f5b96c");
        public static Guid SalesInvoiceCloning = new Guid("801e8be5-9853-4f89-a201-373ddf236175");
        public static Guid PurchaseInvoiceCloning = new Guid("016adc53-638b-4603-a42f-fd2104a21bae");
        public static Guid PurchaseOrderCloning = new Guid("9e2750f7-8fb7-49d9-8568-c805a0134765");
        public static Guid PurchaseOrders = new Guid("b818adae-e475-43b8-af59-fc9f5965fede");
        public static Guid CreditNotes = new Guid("2d9b7175-e68d-4523-ba2a-8c0e1fd62f34");
        public static Guid Export = new Guid("5fee27a6-a79f-40e6-864d-10b930af963d");
        public static Guid SummaryStartDate = new Guid("e251ed88-4d13-400a-9ead-6af39e3fa249");

        // Obsolete
        public static Guid Obsolete_AustraliaGst = new Guid("7cf57cd5-6f44-43ed-9327-3473a6cd854c");
        public static Guid Obsolete_NewZealandGst = new Guid("a6ac8738-89c0-4a26-b2a1-a6d704d0ccba");
        public static Guid Obsolete_UnitedKingdomVat = new Guid("eff70a63-c0d3-40c3-be1e-a7b47ef47e23");
        public static Guid Obsolete_IndiaServiceTax = new Guid("c117b7ca-eb77-4a63-ba64-6c86574b4105");
        public static Guid Obsolete_IndiaCentralStateTax = new Guid("8492e19f-e784-408f-b892-db2e754cd78a");
        public static Guid Obsolete_SouthAfricaVat = new Guid("acfa1675-1000-40d5-b45d-66ff522d4303");
        public static Guid Obsolete_PhilippinesVat = new Guid("9750cfb1-100c-4e94-a29d-3cef0af8154a");
        public static Guid Obsolete_NorwayVat = new Guid("c1dee970-6def-45ab-85bc-6129ccd8e5bc");
        public static Guid Obsolete_BelgiumVat = new Guid("c8a4ba61-008a-46ed-9077-fb9c4ea2dd29");
    }
}