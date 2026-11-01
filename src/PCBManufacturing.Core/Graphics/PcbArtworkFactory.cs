using PCBManufacturing.Core.Models.Artwork;

namespace PCBManufacturing.Core.Graphics;

public sealed class PcbArtworkFactory : IPcbArtworkFactory
{
    public const double BoardWidth = 1000;
    public const double BoardHeight = 700;
    public const double StackWidth = 1000;
    public const double StackHeight = 320;
    public IReadOnlyList<VectorShape> CreateBoard(PcbArtworkRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        var (x, y, width, height) = Fit(request.WidthMm, request.HeightMm, BoardWidth, BoardHeight, 56);
        var radius = Math.Min(22, Math.Min(width, height) * 0.045);
        var finish = FinishColor(request.FinishId);
        var silk = IsLight(request.MaskHex) ? "#243024" : "#F7F4EA";
        var shapes = new List<VectorShape>
        {
            Shape(PathMarkup.RoundedRect(x - 10, y - 10, width + 20, height + 20, radius + 8), ShapeRole.Substrate, request.SubstrateHex, "", 0),
            Shape(PathMarkup.RoundedRect(x, y, width, height, radius), ShapeRole.Mask, request.MaskHex, "#2A2A2A", 3)
        };
        var padWidth = Math.Clamp(width * 0.07, 16, 42);
        var padHeight = Math.Clamp(height * 0.055, 12, 28);
        var columns = width >= height ? 4 : 3;
        const int rows = 2;
        var left = x + width * 0.16;
        var top = y + height * 0.24;
        var columnSpan = columns == 1 ? 0 : (width * 0.68) / (columns - 1);
        var rowSpan = rows == 1 ? 0 : height * 0.34;
        for (var row = 0; row < rows; row++)
        {
            var centers = new List<double>();
            var pads = new List<VectorShape>();
            for (var column = 0; column < columns; column++)
            {
                var cx = left + column * columnSpan;
                var cy = top + row * rowSpan;
                centers.Add(cx);
                pads.Add(Shape(
                    PathMarkup.RoundedRect(cx - padWidth / 2, cy - padHeight / 2, padWidth, padHeight, 3),
                    ShapeRole.Pad,
                    finish,
                    "#1F1F1F",
                    1));
            }
            if (centers.Count > 1)
            {
                var yLine = top + row * rowSpan;
                shapes.Add(Shape(
                    PathMarkup.Line(centers[0], yLine, centers[^1], yLine),
                    ShapeRole.Trace,
                    "",
                    finish,
                    Math.Max(2, padHeight * 0.18),
                    0.85));
            }
            shapes.AddRange(pads);
        }
        var connectorTop = y + height * 0.84;
        var connectorCount = Math.Clamp((int)(width / 28), 6, 14);
        var connectorSpan = width * 0.7 / connectorCount;
        var connectorLeft = x + width * 0.15;
        for (var index = 0; index < connectorCount; index++)
        {
            var cx = connectorLeft + index * connectorSpan;
            shapes.Add(Shape(
                PathMarkup.RoundedRect(cx, connectorTop, Math.Min(14, connectorSpan * 0.55), height * 0.07, 1.5),
                ShapeRole.Pad,
                finish,
                "",
                0));
        }
        var holeRadius = Math.Clamp(Math.Min(width, height) * 0.028, 7, 16);
        var insetX = Math.Max(holeRadius + 8, width * 0.06);
        var insetY = Math.Max(holeRadius + 8, height * 0.07);
        foreach (var (cx, cy) in new[]
        {
            (x + insetX, y + insetY),
            (x + width - insetX, y + insetY),
            (x + insetX, y + height - insetY),
            (x + width - insetX, y + height - insetY)
        })
        {
            shapes.Add(Shape(PathMarkup.Circle(cx, cy, holeRadius), ShapeRole.Hole, finish, "", 0));
            shapes.Add(Shape(PathMarkup.Circle(cx, cy, holeRadius * 0.55), ShapeRole.Hole, "#141414", "", 0));
        }
        var silkInset = Math.Min(width, height) * 0.12;
        shapes.Add(Shape(
            PathMarkup.RoundedRect(x + silkInset, y + silkInset, width - silkInset * 2, height * 0.12, 4),
            ShapeRole.Silk,
            "",
            silk,
            1.5));
        shapes.Add(Shape(
            PathMarkup.Polyline((x + silkInset, y + silkInset), (x + silkInset + 16, y + silkInset + 8), (x + silkInset, y + silkInset + 16)),
            ShapeRole.Silk,
            "",
            silk,
            1.6));
        return shapes;
    }
    public IReadOnlyList<VectorShape> CreateLayerStack(PcbArtworkRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        var count = Math.Max(1, request.LayerCount);
        var finish = FinishColor(request.FinishId);
        var layers = new List<(string Hex, double Weight, ShapeRole Role)>
        {
            (request.MaskHex, 0.55, ShapeRole.Mask)
        };
        for (var index = 0; index < count; index++)
        {
            layers.Add((finish, 0.32, ShapeRole.Copper));
            if (index < count - 1)
            {
                var dielectric = index % 2 == 0 ? request.SubstrateHex : Shade(request.SubstrateHex, 0.72);
                layers.Add((dielectric, 1.35, ShapeRole.Core));
            }
        }
        layers.Add((request.MaskHex, 0.55, ShapeRole.Mask));
        var totalWeight = layers.Sum(layer => layer.Weight);
        var stackHeight = Math.Clamp(150 + request.ThicknessMm * 28, 170, 250);
        var stackWidth = 680d;
        var left = (StackWidth - stackWidth) / 2;
        var top = (StackHeight - stackHeight) / 2;
        var shapes = new List<VectorShape>();
        var cursor = top;
        foreach (var (hex, weight, role) in layers)
        {
            var height = stackHeight * (weight / totalWeight);
            shapes.Add(Shape(PathMarkup.RoundedRect(left, cursor, stackWidth, Math.Max(3, height), 2), role, hex, "#00000000", 0));
            cursor += height;
        }
        shapes.Add(Shape(
            PathMarkup.RoundedRect(left - 2, top - 2, stackWidth + 4, cursor - top + 4, 3),
            ShapeRole.Silk,
            "",
            "#2A2A2A",
            2));
        return shapes;
    }
    public static string FinishColor(string? finishId) => finishId switch
    {
        "enig" => "#D4AF37",
        "osp" => "#B87333",
        _ => "#D5D8DC"
    };
    private static VectorShape Shape(string geometry, ShapeRole role, string fill, string stroke, double strokeThickness, double opacity = 1) =>
        new()
        {
            Geometry = geometry,
            Role = role,
            FillHex = fill,
            StrokeHex = stroke,
            StrokeThickness = strokeThickness,
            Opacity = opacity
        };
    private static (double X, double Y, double Width, double Height) Fit(double widthMm, double heightMm, double canvasWidth, double canvasHeight, double margin)
    {
        var availableWidth = canvasWidth - margin * 2;
        var availableHeight = canvasHeight - margin * 2;
        var width = Math.Max(widthMm, 1);
        var height = Math.Max(heightMm, 1);
        var scale = Math.Min(availableWidth / width, availableHeight / height);
        var fittedWidth = width * scale;
        var fittedHeight = height * scale;
        return ((canvasWidth - fittedWidth) / 2, (canvasHeight - fittedHeight) / 2, fittedWidth, fittedHeight);
    }
    private static bool IsLight(string hex)
    {
        var (red, green, blue) = Parse(hex);
        var luminance = (0.2126 * red + 0.7152 * green + 0.0722 * blue) / 255d;
        return luminance > 0.72;
    }
    private static string Shade(string hex, double factor)
    {
        var (red, green, blue) = Parse(hex);
        return $"#{Scale(red, factor):X2}{Scale(green, factor):X2}{Scale(blue, factor):X2}";
    }
    private static int Scale(int channel, double factor) =>
        Math.Clamp((int)Math.Round(channel * factor), 0, 255);
    private static (int R, int G, int B) Parse(string hex)
    {
        var value = hex.Trim().TrimStart('#');
        if (value.Length != 6)
            return (120, 120, 120);
        return (
            Convert.ToInt32(value[..2], 16),
            Convert.ToInt32(value[2..4], 16),
            Convert.ToInt32(value[4..6], 16));
    }
}
