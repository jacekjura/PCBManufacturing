using PCBManufacturing.Core.Graphics;

namespace PCBManufacturing.Core.Tests.Graphics;

public class PathMarkupTests
{
    [Fact]
    public void RoundedRectsAndCircles_ArePathMarkup()
    {
        Assert.Contains("Z", PathMarkup.RoundedRect(0, 0, 100, 40, 8));
        Assert.Contains("A", PathMarkup.Circle(10, 10, 4));
    }
}
