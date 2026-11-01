using System.Text.RegularExpressions;

namespace PCBManufacturing.Core.Services.Validation;

/// <summary>
/// Accepts a US ZIP, a UK postcode, or a 6-digit Russian postal index.
/// </summary>
public sealed partial class PostcodeValidator : IPostcodeValidator
{
    public PostcodeValidationResult Validate(string? postcode)
    {
        if (string.IsNullOrWhiteSpace(postcode))
            return PostcodeValidationResult.InvalidResult(PostcodeError.Empty);

        var collapsed = Whitespace().Replace(postcode.Trim(), " ");
        if (UsZip().IsMatch(collapsed) || RussianIndex().IsMatch(collapsed) || UkPostcode().IsMatch(collapsed))
            return PostcodeValidationResult.Valid;

        return PostcodeValidationResult.InvalidResult(PostcodeError.Invalid);
    }

    [GeneratedRegex(@"^\d{5}(?:-\d{4})?$", RegexOptions.CultureInvariant)]
    private static partial Regex UsZip();

    [GeneratedRegex(@"^\d{6}$", RegexOptions.CultureInvariant)]
    private static partial Regex RussianIndex();

    [GeneratedRegex(@"^(?:GIR\s?0AA|[A-Z]{1,2}\d[A-Z\d]?\s?\d[A-Z]{2})$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex UkPostcode();

    [GeneratedRegex(@"\s+")]
    private static partial Regex Whitespace();
}
