namespace AvaloniaProject.ViewModels.Pages;

public interface IPageViewModel
{
    string Name { get; }
    string Icon { get; }
    int IconSize { get; set; }
    int Index { get; }
}
