using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace AvaloniaProject.Services;

public sealed class LocalizationService
{
    private static readonly ResourceManager ResourceManager =
        new("AvaloniaProject.Resources.Strings", Assembly.GetExecutingAssembly());

    public static LocalizationService Instance { get; } = new();

    public CultureInfo CurrentCulture { get; private set; } = CultureInfo.GetCultureInfo("en-US");

    public string this[string key] => GetString(key);

    private LocalizationService() { }

    public event EventHandler? CultureChanged;

    public void SetCulture(string cultureName)
    {
        SetCulture(CultureInfo.GetCultureInfo(cultureName));
    }

    public void SetCulture(CultureInfo culture)
    {
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
