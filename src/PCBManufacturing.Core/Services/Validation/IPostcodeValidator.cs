namespace PCBManufacturing.Core.Services.Validation;

public interface IPostcodeValidator
{
    PostcodeValidationResult Validate(string? postcode);
}
