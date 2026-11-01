namespace PCBManufacturing.Core.Validation;

public enum PostcodeError
{
    None,
    Empty,
    Invalid
}

public readonly record struct PostcodeValidationResult(bool IsValid, PostcodeError Error)
{
    public static PostcodeValidationResult Valid { get; } = new(true, PostcodeError.None);

    public static PostcodeValidationResult InvalidResult(PostcodeError error) => new(false, error);
}
