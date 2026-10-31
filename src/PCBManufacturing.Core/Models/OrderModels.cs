namespace PCBManufacturing.Core.Models;

public sealed class OrderDraft
{
    public required string ProjectName { get; init; }
    public required string Postcode { get; init; }
    public required string Notes { get; init; }
    public required string MaterialKey { get; init; }
    public required string MaskKey { get; init; }
    public required string MaskHex { get; init; }
    public required string ThicknessKey { get; init; }
    public required string SizeKey { get; init; }
    public required string FinishKey { get; init; }
    public required int Layers { get; init; }
    public required int Quantity { get; init; }
    public required decimal Total { get; init; }
    public required string Currency { get; init; }
    public required int ProductionDays { get; init; }
}

public sealed class OrderConfirmation
{
    public string Number { get; init; } = "";
    public DateTimeOffset CreatedAt { get; init; }
    public string ProjectName { get; init; } = "";
    public string Postcode { get; init; } = "";
    public string Notes { get; init; } = "";
    public string MaterialKey { get; init; } = "";
    public string MaskKey { get; init; } = "";
    public string MaskHex { get; init; } = "#1B7A3A";
    public string ThicknessKey { get; init; } = "";
    public string SizeKey { get; init; } = "";
    public string FinishKey { get; init; } = "";
    public int Layers { get; init; }
    public int Quantity { get; init; }
    public decimal Total { get; init; }
    public string Currency { get; init; } = "USD";
    public int ProductionDays { get; init; }
}

public sealed class DialogRow
{
    public required string Label { get; init; }
    public required string Value { get; init; }
}

public sealed class DialogContent
{
    public required string Title { get; init; }
    public required string Message { get; init; }
    public required string ConfirmText { get; init; }
    public string? CancelText { get; init; }
    public bool ShowCancel => !string.IsNullOrWhiteSpace(CancelText);
    public IReadOnlyList<DialogRow> Rows { get; init; } = [];
}
