using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;

namespace Micacord.UserControls
{
    public sealed partial class ServerListItem : UserControl
    {
        public ServerListItem()
        {
            InitializeComponent();
        }

        // Public properties for setting from outside
        public string ServerName
        {
            get => ServerNameTextBlock.Text;
            set => ServerNameTextBlock.Text = value;
        }

        public BitmapImage ServerPicture
        {
            get => ServerPictureImage.Source as BitmapImage;
            set => ServerPictureImage.Source = value;
        }
    }
}
