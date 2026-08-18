using System;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.CustomerPortal.Summary
{
    [ProtoContract]
    class CustomerPortal : Template
    {
        protected override void InnerGet()
        {
            var customerPortal = ApplicationData.Businesses.Get(Business).SingleOrDefault<ManagerServer.Model.CustomerPortal>(CustomerPortal);
            var customer = ApplicationData.Businesses.Get(Business).SingleOrDefault<ManagerServer.Model.Customer>(customerPortal.Customer);

            using (Div(@class: "row"))
            {
                using (Div(@class: "col-6"))
                {
                    using (Div(@class: "card"))
                    {
                        using (Div(@class: "card-header p-3 fw-bold", style: "background-color: #F5F5F5; box-shadow: inset 1px 1px 0px #fff; color: #333; text-shadow: 1px 1px 0 #fff; color: #ccc; font-size: .875rem"))
                        {
                            Write(Strings.Customer);
                        }
                        using (Div(@class: "list-group list-group-flush", style: "font-size: 0.75rem"))
                        {
                            using (Div(@class: "list-group-item"))
                            {
                                using (H6(@class: "card-title text-muted")) Write(Strings.Name);
                                using (P(@class: "card-text")) Write(customer.Name);
                            }
                            using (Div(@class: "list-group-item"))
                            {
                                using (H6(@class: "card-title text-muted")) Write(Strings.BillingAddress);
                                using (P(@class: "card-text")) Write(customer.BillingAddress?.Replace("\n", "<br />"));
                            }
                            using (Div(@class: "list-group-item"))
                            {
                                using (H6(@class: "card-title text-muted")) Write(Strings.DeliveryAddress);
                                using (P(@class: "card-text")) Write(customer.DeliveryAddress?.Replace("\n", "<br />"));
                            }
                            using (Div(@class: "list-group-item"))
                            {
                                using (H6(@class: "card-title text-muted")) Write(Strings.Email);
                                using (P(@class: "card-text")) Write(customer.Email);
                            }
                        }
                    }
                }
            }
        }
    }
}
