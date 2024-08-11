using Micacord.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace Micacord.Views;

public sealed partial class MessagesPage : Page
{
    public MessagesViewModel ViewModel
    {
        get;
    }

    public MessagesPage()
    {
        ViewModel = App.GetService<MessagesViewModel>();
        InitializeComponent();
    }
}
