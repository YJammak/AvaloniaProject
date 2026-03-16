using System;
using System.ComponentModel;
using System.Globalization;

namespace AvaloniaProject.Services;

public sealed class LocalizationSource : INotifyPropertyChanged
{
    public static LocalizationSource Instance { get; } = new();

    public string this[string key] => LocalizationService.Instance[key];

    public CultureInfo ThemeCulture =>
        LocalizationService.Instance.CurrentCulture.Name.StartsWith("zh", StringComparison.OrdinalIgnoreCase)
            ? CultureInfo.GetCultureInfo("zh-CN")
            : CultureInfo.GetCultureInfo("en-US");

    private LocalizationSource()
    {
        LocalizationService.Instance.CultureChanged += (_, _) =>
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ThemeCulture)));
        };
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
