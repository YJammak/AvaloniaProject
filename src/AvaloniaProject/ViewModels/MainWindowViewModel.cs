namespace AvaloniaProject.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public ViewModelBase MainView { get; } = new MainViewModel();
}
