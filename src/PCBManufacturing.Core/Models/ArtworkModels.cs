namespace PCBManufacturing.Core.Models;

public sealed class PcbArtworkRequest
{
    public required double WidthMm { get; init; }
    public required double HeightMm { get; init; }
    public required int LayerCount { get; init; }
    public required double ThicknessMm { get; init; }
    public required string MaskHex { get; init; }
    public required string SubstrateHex { get; init; }
    public required string FinishId { get; init; }
}

public sealed record VectorShape
{
    public required string Geometry { get; init; }
    public required ShapeRole Role { get; init; }
    public required string FillHex { get; init; }
    public required string StrokeHex { get; init; }
    public double StrokeThickness { get; init; }
    public double Opacity { get; init; } = 1;
}
