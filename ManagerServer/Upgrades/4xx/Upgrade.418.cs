using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManagerServer.Model;
using ManagerServer.Model.Enums;

namespace ManagerServer
{
    public static partial class Upgrade
    {
        private static async Task<IEnumerable<Model.Object>> Upgrade418(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;

            var businessDetails = objects.Single<BusinessDetails>();
            if (string.IsNullOrWhiteSpace(businessDetails.Obsolete_Country)) return null;

            using (var tx = objects.BeginTransaction())
            {
                void Add(string name, string endpoint)
                {
                    tx.InsertOrReplace2(new CustomButton()
                    {
                        Key = Guid.NewGuid(),
                        Name = name,
                        Source = ExtensionSource.Url,
                        Endpoint = endpoint,
                        Placement = "reports"
                    });
                }

                if (businessDetails.Obsolete_Country == "en-AE")
                {
                    Add("VAT201- VAT Return (En)", "/extensions/ae/vat201-vat-return-en.html");
                    Add("VAT201- إقرار ضريبة القيمة المضافة", "/extensions/ae/vat201.html");
                }
                else if (businessDetails.Obsolete_Country == "ar-AE")
                {
                    Add("VAT201- VAT Return (En)", "/extensions/ar-ae/vat201-vat-return-en.html");
                    Add("VAT201- إقرار ضريبة القيمة المضافة", "/extensions/ar-ae/vat201.html");
                }
                else if (businessDetails.Obsolete_Country == "en-AU")
                {
                    Add("Business Activity Statement", "/extensions/au/business-activity-statement.html");
                    if (objects.OfType<Employee>().Length != 0)
                    {
                        Add("PAYG payment summary — individual non-business", "/extensions/au/payg-payment-summary-individual-non-business.html");
                        Add("Single Touch Payroll Worksheet (Phase 2)", "/extensions/au/single-touch-payroll-worksheet-phase-2.html");
                    }
                    if (objects.OfType<Supplier>().Length != 0)
                    {
                        Add("Taxable Payments Annual Report (TPAR)", "/extensions/au/taxable-payments-annual-report-tpar.html");
                    }
                }
                else if (businessDetails.Obsolete_Country == "en-BH")
                {
                    Add("Simplified VAT Return Form", "/extensions/bh/simplified-vat-return-form.html");
                    Add("VAT Return Form", "/extensions/bh/vat-return-form.html");
                }
                else if (businessDetails.Obsolete_Country == "en-GB")
                {
                    Add("VAT Calculation Worksheet", "/extensions/gb/vat-calculation-worksheet.html");
                }
                else if (businessDetails.Obsolete_Country == "en-GH")
                {
                    Add("Communication Service Tax Returns", "/extensions/gh/communication-service-tax-returns.html");
                    Add("Standard Rate NHIL, GET Fund & Covid-19 Levy Return.", "/extensions/gh/standard-rate-nhil-get-fund-covid-19-levy-return.html");
                    Add("Standard Rate VAT Return", "/extensions/gh/standard-rate-vat-return.html");
                    Add("Tourism Levy Payable Return", "/extensions/gh/tourism-levy-payable-return.html");
                    Add("VAT Flat Rate Scheme (VFRS) And Covid-19 Levy Return", "/extensions/gh/vat-flat-rate-scheme-vfrs-and-covid-19-levy-return.html");
                }
                else if (businessDetails.Obsolete_Country == "en-IE")
                {
                    Add("Return of Trading Details", "/extensions/ie/return-of-trading-details.html");
                    Add("VAT3 Return", "/extensions/ie/vat3-return.html");
                }
                else if (businessDetails.Obsolete_Country == "mk-MK")
                {
                    Add("ДДВ-04", "/extensions/mk/04.html");
                }
                else if (businessDetails.Obsolete_Country == "nl-NL")
                {
                    Add("Concept BTW Aangifte", "/extensions/nl/concept-btw-aangifte.html");
                }
                else if (businessDetails.Obsolete_Country == "en-NZ")
                {
                    Add("GST Return", "/extensions/nz/gst-return.html");
                }
                else if (businessDetails.Obsolete_Country == "ar-OM")
                {
                    Add("استمارة الإقرار الضريبي", "/extensions/om/report-72ceb271.html");
                }
                else if (businessDetails.Obsolete_Country == "ar-SA")
                {
                    Add("VAT Return Form", "/extensions/sa/vat-return-form.html");
                    Add("إقرار ضريبة القيمة المضافة", "/extensions/sa/report-6ba84ca4.html");
                }
                else if (businessDetails.Obsolete_Country == "sk-SK")
                {
                    Add("Daňové priznanie - Daň z pridanej hodnoty", "/extensions/sk-sk/priznanie-k-dph.html");
                }
                else if (businessDetails.Obsolete_Country == "en-ZA")
                {
                    Add("Employee IRP5", "/extensions/za/employee-irp5.html");
                    Add("VAT201 - Vendor Declaration", "/extensions/za/vat201-vendor-declaration.html");
                }

                tx.Commit();
            }

            return null;
        }
    }
}
