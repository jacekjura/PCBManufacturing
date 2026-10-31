using PCBManufacturing.Core.Graphics;

namespace PCBManufacturing.Core.Tests.Graphics;

public class IconGeometryTests
{
    [Fact]
    public void Icons_ArePathMarkup()
    {
        foreach (var name in IconGeometry.Names)
        {
            var icon = IconGeometry.Get(name);
            Assert.StartsWith("M", icon.Geometry.Trim());
        }
    }
}
