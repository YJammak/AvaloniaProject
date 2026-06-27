namespace AvaloniaProject.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public ViewModelBase MainView { get; }

    public MainWindowViewModel(MainViewModel mainView)
    {
        MainView = mainView;
    }
}
