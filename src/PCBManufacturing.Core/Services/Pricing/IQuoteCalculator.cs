using PCBManufacturing.Core.Models.Quote;

namespace PCBManufacturing.Core.Services.Pricing;

public interface IQuoteCalculator
{
    QuoteResult Calculate(QuoteRequest request);
}
