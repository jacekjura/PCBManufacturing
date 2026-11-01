namespace PCBManufacturing.Core.Services.Ordering;

public interface IOrderNumberFactory
{
    string Create(DateTimeOffset timestamp);
}
