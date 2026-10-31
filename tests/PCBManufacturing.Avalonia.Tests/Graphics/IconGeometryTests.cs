using Avalonia.Media;
using PCBManufacturing.Core.Graphics;

namespace PCBManufacturing.AvaloniaApp.Tests.Graphics;

[Collection("Avalonia")]
public sealed class IconGeometryTests
{
    [Fact]
    public void Icons_ParseAsAvaloniaGeometry()
    {
        foreach (var name in IconGeometry.Names)
        {
            var icon = IconGeometry.Get(name);
            var geometry = Geometry.Parse(icon.Geometry);
            Assert.True(geometry.Bounds.Width > 0 && geometry.Bounds.Height > 0, name);
        }
    }
}
