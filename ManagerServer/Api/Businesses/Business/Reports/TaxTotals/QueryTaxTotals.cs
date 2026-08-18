using ManagerServer.Endpoints;

namespace ManagerServer.Api.Businesses.Business.Reports.TaxTotals
{
    internal sealed class QueryTaxTotals : AuthorizedEndpoint<TaxTotalsCalculator.TaxCodeTotals[]>
    {
        public Model.TaxTotals Value { get; set; }

        public override TaxTotalsCalculator.TaxCodeTotals[] AuthorizedHandle()
        {
            if (Value == null)
            {
                throw new BadRequestException($"Request body is missing a '{nameof(Value)}' property containing the {nameof(Model.TaxTotals)} report parameters.");
            }
            return TaxTotalsCalculator.Calculate(Business, Value);
        }
    }
}
