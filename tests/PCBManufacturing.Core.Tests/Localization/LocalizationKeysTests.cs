using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using PCBManufacturing.Core;
using PCBManufacturing.Core.Localization;
using PCBManufacturing.Core.Localization.Keys;

namespace PCBManufacturing.Core.Tests.Localization;

public class LocalizationKeysTests
{
    private static readonly string[] DottedSuffixes = ["Description", "Value", "Hint"];
    private static readonly Regex BindingKey = new(
        @"(?:Localization|Text)\[(?<key>[A-Za-z][A-Za-z0-9]*(?:\.[A-Za-z0-9]+)*)\]",
        RegexOptions.CultureInvariant | RegexOptions.Compiled);
    private static readonly Regex RawKeyLiteral = new(
        "\"(?<key>(?:App|Tab|Status|Action|Theme|Preferences|Field|Summary|Quote|Order|Validation|Impact|Group|Material|Color|Thickness|Param)(?:\\.[A-Za-z0-9]+)+)\"",
        RegexOptions.CultureInvariant | RegexOptions.Compiled);
    [Fact]
    public void Constants_MatchTheirMemberPathAndBothLanguageFiles()
    {
        var constants = ReadConstants();
        foreach (var (path, value) in constants)
            Assert.Equal(path, value);
        var english = ReadLanguage("en.json");
        var russian = ReadLanguage("ru.json");
        var keys = constants.Select(item => item.Value).OrderBy(key => key, StringComparer.Ordinal).ToArray();
        Assert.Equal(keys, english.Keys.OrderBy(key => key, StringComparer.Ordinal));
        Assert.Equal(keys, russian.Keys.OrderBy(key => key, StringComparer.Ordinal));
    }
    [Fact]
    public void Constants_ResolveToATranslationInEveryLanguage()
    {
        var service = new JsonLocalizationService(ContentFiles.LocalizationDirectory(AppContext.BaseDirectory));
        var keys = ReadConstants().Select(item => item.Value).ToArray();
        Assert.Contains(Order.Error.Project, keys);
        AssertTranslations(service, keys);
        service.SetCulture("ru");
        AssertTranslations(service, keys);
    }
    [Fact]
    public void Views_BindOnlyKnownKeys()
    {
        var known = ReadConstants().Select(item => item.Value).ToHashSet(StringComparer.Ordinal);
        var bindings = new List<string>();
        foreach (var file in SourceFiles("*.xaml").Concat(SourceFiles("*.axaml")))
        {
            var text = File.ReadAllText(file);
            foreach (Match match in BindingKey.Matches(text))
            {
                var key = match.Groups["key"].Value;
                Assert.True(known.Contains(key), $"{Path.GetFileName(file)} binds unknown key '{key}'.");
                bindings.Add(key);
            }
        }
        Assert.Contains(App.Title, bindings);
    }
    [Fact]
    public void ProductionCode_UsesConstantsInsteadOfRawKeyLiterals()
    {
        var offenders = new List<string>();
        foreach (var file in SourceFiles("*.cs"))
        {
            if (file.Contains($"{Path.DirectorySeparatorChar}Keys{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase))
                continue;
            var lines = File.ReadAllLines(file);
            for (var index = 0; index < lines.Length; index++)
            {
                if (RawKeyLiteral.IsMatch(lines[index]))
                    offenders.Add($"{Path.GetFileName(file)}:{index + 1}");
            }
        }
        Assert.Empty(offenders);
    }
    private static void AssertTranslations(JsonLocalizationService service, IEnumerable<string> keys)
    {
        foreach (var key in keys)
        {
            var translated = service[key];
            Assert.False(string.IsNullOrWhiteSpace(translated), key);
            Assert.NotEqual(key, translated);
        }
    }
    private static IReadOnlyList<(string Path, string Value)> ReadConstants()
    {
        var keys = new List<(string Path, string Value)>();
        var roots = typeof(JsonLocalizationService).Assembly.GetTypes()
            .Where(type => type is { IsClass: true, IsAbstract: true, IsSealed: true, DeclaringType: null }
                && type.Namespace == typeof(App).Namespace);
        foreach (var root in roots)
            Collect(root, [root.Name], keys);
        return keys;
    }
    private static void Collect(Type type, IReadOnlyList<string> prefix, List<(string Path, string Value)> keys)
    {
        const BindingFlags flags = BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly;
        foreach (var field in type.GetFields(flags))
        {
            if (!field.IsLiteral || field.FieldType != typeof(string))
                continue;
            var path = string.Join('.', prefix) + "." + Segment(field.Name);
            var value = field.GetRawConstantValue() as string;
            Assert.False(string.IsNullOrWhiteSpace(value), field.Name);
            keys.Add((path, value));
        }
        foreach (var nested in type.GetNestedTypes(BindingFlags.Public))
            Collect(nested, [.. prefix, nested.Name], keys);
    }
    private static string Segment(string name)
    {
        if (name.Length > 1 && name[0] == '_' && char.IsDigit(name[1]))
            name = name[1..];
        foreach (var suffix in DottedSuffixes)
        {
            if (name.EndsWith(suffix, StringComparison.Ordinal) && name.Length > suffix.Length)
                return string.Concat(name.AsSpan(0, name.Length - suffix.Length), ".", suffix);
        }
        return name;
    }
    private static Dictionary<string, string> ReadLanguage(string fileName)
    {
        var path = Path.Combine(ContentFiles.LocalizationDirectory(AppContext.BaseDirectory), fileName);
        return JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(path))
            ?? throw new InvalidDataException(fileName);
    }
    private static IEnumerable<string> SourceFiles(string pattern)
    {
        var source = Path.Combine(FindRepositoryRoot(), "src");
        return Directory.EnumerateFiles(source, pattern, SearchOption.AllDirectories)
            .Where(file => !file.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase)
                && !file.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase));
    }
    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (Directory.Exists(Path.Combine(directory.FullName, "src", "PCBManufacturing.Core")))
                return directory.FullName;
            directory = directory.Parent;
        }
        throw new DirectoryNotFoundException("Could not locate the repository root from the test output directory.");
    }
}
