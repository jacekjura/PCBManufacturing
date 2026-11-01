namespace PCBManufacturing.Core.Models.Artwork;

public sealed record VectorShape
{
    public required string Geometry { get; init; }
    public required ShapeRole Role { get; init; }
    public required string FillHex { get; init; }
    public required string StrokeHex { get; init; }
    public double StrokeThickness { get; init; }
    public double Opacity { get; init; } = 1;
}
