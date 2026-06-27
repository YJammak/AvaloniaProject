using System;
using System.Globalization;

namespace AvaloniaProject.Services;

public interface ILocalizationService
{
    CultureInfo CurrentCulture { get; }

    string this[string key] { get; }

    event EventHandler? CultureChanged;

    CultureInfo ResolveStartupCulture();
    CultureInfo MapToSupportedCulture(CultureInfo culture);
    string GetSupportedCultureName(string cultureName);
    void SetCulture(string cultureName);
    void SetCulture(CultureInfo culture);
    string GetString(string key);
}
