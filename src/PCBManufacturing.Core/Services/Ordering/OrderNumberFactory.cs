namespace PCBManufacturing.Core.Services.Ordering;

public sealed class OrderNumberFactory : IOrderNumberFactory
{
    public string Create(DateTimeOffset timestamp) =>
        $"PCB-{timestamp:yyyyMMdd}-{Random.Shared.Next(1000, 10000)}";
}
