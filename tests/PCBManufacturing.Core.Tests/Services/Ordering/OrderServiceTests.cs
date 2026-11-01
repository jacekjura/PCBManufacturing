using PCBManufacturing.Core.Localization;
using PCBManufacturing.Core.Models;
using PCBManufacturing.Core.Ordering;

namespace PCBManufacturing.Core.Tests.Ordering;

public class OrderServiceTests
{
    [Fact]
    public async Task Submit_UsesTheClockAndNumberFactory()
    {
        var clock = new FixedClock(new DateTimeOffset(2026, 10, 29, 18, 0, 0, TimeSpan.Zero));
        var service = new OrderService(new FixedNumbers("PCB-20261029-1001"), clock, TimeSpan.Zero);
        var draft = new OrderDraft
        {
            ProjectName = "AC-9001",
            Postcode = "94105",
            Notes = "",
            MaterialKey = Material.Fr4,
            MaskKey = Color.Green,
            MaskHex = "#1B7A3A",
            ThicknessKey = Thickness._16,
            SizeKey = Param.Size100,
            FinishKey = Param.Hasl,
            Layers = 2,
            Quantity = 10,
            Total = 250m,
            Currency = "USD",
            ProductionDays = 5
        };

        var confirmation = await service.SubmitAsync(draft);

        Assert.Equal("PCB-20261029-1001", confirmation.Number);
        Assert.Equal(250m, confirmation.Total);
        Assert.Equal(new DateTimeOffset(2026, 10, 29, 18, 0, 0, TimeSpan.Zero), confirmation.CreatedAt);
    }

    private sealed class FixedClock(DateTimeOffset now) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => now;
    }

    private sealed class FixedNumbers(string number) : IOrderNumberFactory
    {
        public string Create(DateTimeOffset timestamp) => number;
    }
}
