using System;
using System.ComponentModel;
using System.Globalization;
using Splat;

namespace AvaloniaProject.Services;

public sealed class LocalizationSource : INotifyPropertyChanged
{
    private static readonly Lazy<LocalizationSource> LazyInstance = new(() =>
    {
        var localization = Locator.Current.GetService<ILocalizationService>();
        if (localization is null)
            throw new InvalidOperationException(
                "ILocalizationService is not registered. Ensure RegisterServices() is called before XAML loads.");
        return new LocalizationSource(localization);
    });

    public static LocalizationSource Instance => LazyInstance.Value;

    private readonly ILocalizationService _localization;

    public string this[string key] => _localization[key];

    public CultureInfo ThemeCulture =>
        _localization.CurrentCulture.Name.StartsWith("zh", StringComparison.OrdinalIgnoreCase)
            ? CultureInfo.GetCultureInfo("zh-CN")
            : CultureInfo.GetCultureInfo("en-US");

    public LocalizationSource(ILocalizationService localization)
    {
        _localization = localization;
        _localization.CultureChanged += (_, _) =>
        {
            // string.Empty signals "all properties changed" — more reliable than "Item[]"
            // for Avalonia compiled bindings on indexer paths.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
        };
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
