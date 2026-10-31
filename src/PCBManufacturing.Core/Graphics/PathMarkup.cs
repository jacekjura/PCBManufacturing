using System.Globalization;

namespace PCBManufacturing.Core.Graphics;

public static class PathMarkup
{
    public static string RoundedRect(double x, double y, double width, double height, double radius)
    {
        if (width <= 0 || height <= 0)
            throw new ArgumentOutOfRangeException(nameof(width), "Rectangle size must be positive.");

        var r = Math.Clamp(radius, 0, Math.Min(width, height) / 2);
        if (r <= 0)
        {
            return FormattableString.Invariant($"M {N(x)},{N(y)} H {N(x + width)} V {N(y + height)} H {N(x)} Z");
        }

        var right = x + width;
        var bottom = y + height;
        return FormattableString.Invariant(
            $"M {N(x + r)},{N(y)} H {N(right - r)} A {N(r)},{N(r)} 0 0 1 {N(right)},{N(y + r)} V {N(bottom - r)} A {N(r)},{N(r)} 0 0 1 {N(right - r)},{N(bottom)} H {N(x + r)} A {N(r)},{N(r)} 0 0 1 {N(x)},{N(bottom - r)} V {N(y + r)} A {N(r)},{N(r)} 0 0 1 {N(x + r)},{N(y)} Z");
    }

    public static string Circle(double centerX, double centerY, double radius)
    {
        if (radius <= 0)
            throw new ArgumentOutOfRangeException(nameof(radius), "Radius must be positive.");

        var r = N(radius);
        var cx = N(centerX);
        var cy = N(centerY);
        var left = N(centerX - radius);
        var right = N(centerX + radius);
        return FormattableString.Invariant($"M {left},{cy} A {r},{r} 0 1 1 {right},{cy} A {r},{r} 0 1 1 {left},{cy} Z");
    }

    public static string Line(double x1, double y1, double x2, double y2) =>
        FormattableString.Invariant($"M {N(x1)},{N(y1)} L {N(x2)},{N(y2)}");

    public static string Polyline(params (double X, double Y)[] points)
    {
        if (points.Length < 2)
            throw new ArgumentException("A polyline needs at least two points.", nameof(points));

        var first = points[0];
        var commands = FormattableString.Invariant($"M {N(first.X)},{N(first.Y)}");
        for (var index = 1; index < points.Length; index++)
            commands += FormattableString.Invariant($" L {N(points[index].X)},{N(points[index].Y)}");
        return commands;
    }

    private static string N(double value) => value.ToString("0.###", CultureInfo.InvariantCulture);
}
