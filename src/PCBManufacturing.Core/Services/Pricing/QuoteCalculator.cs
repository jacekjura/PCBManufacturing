using PCBManufacturing.Core.Localization;
using PCBManufacturing.Core.Models;

namespace PCBManufacturing.Core.Pricing;

public interface IQuoteCalculator
{
    QuoteResult Calculate(QuoteRequest request);
}

public sealed class QuoteCalculator : IQuoteCalculator
{
    public QuoteResult Calculate(QuoteRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (request.BasePricePerBoard <= 0)
            throw new ArgumentOutOfRangeException(nameof(request), "Base price must be positive.");

        var quantity = request.Quantity.Quantity.GetValueOrDefault();
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be a positive board count.", nameof(request));

        var steps = new (string Key, decimal Modifier, int Days)[]
        {
            (Quote.Size, request.Size.PriceModifier, request.Size.ProductionDaysExtra),
            (Quote.Material, request.Material.PriceModifier, request.Material.ProductionDays),
            (Quote.Thickness, request.Thickness.PriceModifier, request.Thickness.ProductionDaysExtra),
            (Quote.Layers, request.Layers.PriceModifier, request.Layers.ProductionDaysExtra),
            (Quote.Finish, request.Finish.PriceModifier, request.Finish.ProductionDaysExtra),
            (Quote.Quantity, request.Quantity.PriceModifier, request.Quantity.ProductionDaysExtra)
        };

        decimal running = request.BasePricePerBoard;
        var raw = new List<(string Key, decimal Amount, decimal Modifier, int Days)>
        {
            (Quote.Base, running, 1m, 0)
        };

        foreach (var (key, modifier, days) in steps)
        {
            if (modifier <= 0)
                throw new ArgumentOutOfRangeException(nameof(request), $"Modifier for {key} must be positive.");

            var next = running * modifier;
            raw.Add((key, next - running, modifier, days));
            running = next;
        }

        var unit = decimal.Round(running, 2, MidpointRounding.AwayFromZero);
        var rounded = raw.Select(line => decimal.Round(line.Amount, 2, MidpointRounding.AwayFromZero)).ToList();
        rounded[^1] += unit - rounded.Sum();

        var lines = new List<QuoteImpactLine>(raw.Count);
        for (var index = 0; index < raw.Count; index++)
            lines.Add(new QuoteImpactLine(raw[index].Key, rounded[index], raw[index].Modifier, raw[index].Days));

        return new QuoteResult
        {
            UnitPrice = unit,
            Total = unit * quantity,
            ProductionDays = steps.Sum(step => step.Days),
            Quantity = quantity,
            Currency = request.Currency,
            Lines = lines
        };
    }
}
