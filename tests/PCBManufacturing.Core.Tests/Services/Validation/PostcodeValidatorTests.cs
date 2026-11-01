using PCBManufacturing.Core.Validation;

namespace PCBManufacturing.Core.Tests.Validation;

public class PostcodeValidatorTests
{
    private readonly PostcodeValidator _validator = new();

    [Theory]
    [InlineData("94105")]
    [InlineData("94105-1234")]
    [InlineData("SW1A 1AA")]
    [InlineData("sw1a1aa")]
    [InlineData("EC1A 1BB")]
    [InlineData("101000")]
    [InlineData("  GIR 0AA ")]
    public void AcceptsUsUkAndRussianCodes(string postcode) =>
        Assert.True(_validator.Validate(postcode).IsValid);

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("simas")]
    [InlineData("12")]
    [InlineData("ABCDE")]
    [InlineData("9410")]
    public void RejectsEmptyAndMalformedCodes(string postcode)
    {
        var result = _validator.Validate(postcode);
        Assert.False(result.IsValid);
        Assert.Equal(string.IsNullOrWhiteSpace(postcode) ? PostcodeError.Empty : PostcodeError.Invalid, result.Error);
    }
}
