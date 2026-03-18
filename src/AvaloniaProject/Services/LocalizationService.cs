using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace AvaloniaProject.Services;

public sealed class LocalizationService
{
    private const string DefaultCultureName = "en-US";
    private const string SimplifiedChineseCultureName = "zh-Hans";

    private static readonly ResourceManager ResourceManager =
        new("AvaloniaProject.Resources.Strings", Assembly.GetExecutingAssembly());

    public static LocalizationService Instance { get; } = new();

    public CultureInfo CurrentCulture { get; private set; } = CultureInfo.GetCultureInfo(DefaultCultureName);

    public string this[string key] => GetString(key);

    private LocalizationService() { }

    public event EventHandler? CultureChanged;

    public CultureInfo ResolveStartupCulture()
    {
        var systemCulture = CultureInfo.InstalledUICulture;
        if (string.IsNullOrWhiteSpace(systemCulture.Name))
            systemCulture = CultureInfo.CurrentUICulture;

        return MapToSupportedCulture(systemCulture);
    }

    public CultureInfo MapToSupportedCulture(CultureInfo culture)
    {
        var cultureName = culture.Name;
        if (cultureName.StartsWith("zh", StringComparison.OrdinalIgnoreCase))
            return CultureInfo.GetCultureInfo(SimplifiedChineseCultureName);

        return CultureInfo.GetCultureInfo(DefaultCultureName);
    }

    public string GetSupportedCultureName(string cultureName)
    {
        return MapToSupportedCulture(CultureInfo.GetCultureInfo(cultureName)).Name;
    }

    public void SetCulture(string cultureName)
    {
        SetCulture(CultureInfo.GetCultureInfo(cultureName));
    }

    public void SetCulture(CultureInfo culture)
    {
        culture = MapToSupportedCulture(culture);
        var hasChanged = !CurrentCulture.Name.Equals(culture.Name, StringComparison.OrdinalIgnoreCase);

        CurrentCulture = culture;
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;

        if (hasChanged)
            CultureChanged?.Invoke(this, EventArgs.Empty);
    }

    public string GetString(string key)
    {
        return ResourceManager.GetString(key, CurrentCulture) ?? key;
    }
}
