namespace PCBManufacturing.Core.Models.Artwork;

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
