using Micacord.ViewModels;
using Microsoft.UI.Xaml;
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

    private async void AddButton_Click(object sender, RoutedEventArgs e)
    {
        var addServerDialog = new AddServerDialog();
        addServerDialog.XamlRoot = ContentArea.XamlRoot; // Set the XamlRoot
        await addServerDialog.ShowAsync();
    }
}
