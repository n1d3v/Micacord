using Micacord.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace Micacord.Views;

public sealed partial class ServersPage : Page
{
    public ServersViewModel ViewModel
    {
        get;
    }

    public ServersPage()
    {
        ViewModel = App.GetService<ServersViewModel>();
        InitializeComponent();
    }
}
