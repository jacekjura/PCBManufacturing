using System.Windows.Media;
using PCBManufacturing.Core.Graphics;

namespace PCBManufacturing.Wpf.Tests.Graphics;

public sealed class IconGeometryTests
{
    [Fact]
    public void Icons_ParseAsWpfGeometry()
    {
        StaThread.Run(() =>
        {
            foreach (var name in IconGeometry.Names)
            {
                var icon = IconGeometry.Get(name);
                var geometry = Geometry.Parse(icon.Geometry);
                Assert.False(geometry.Bounds.IsEmpty, name);
            }
        });
    }
}
