using AvaloniaProject.ViewModels;
using Ursa.ReactiveUIExtension;

namespace AvaloniaProject.Views;

public partial class MainView : ReactiveUrsaView<MainViewModel>
{
    public MainView()
    {
        InitializeComponent();
    }
}
