namespace PCBManufacturing.Core.Models.Quote;

public sealed record QuoteImpactLine(string LabelKey, decimal Amount, decimal Modifier, int Days);
