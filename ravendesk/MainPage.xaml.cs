using CommunityToolkit.Maui.Media;
using CommunityToolkit.Maui.Storage;

namespace ravendesk
{
    public partial class MainPage : ContentPage
    {
        private readonly IFileSaver fileSaver;
        public MainPage()
        {
            InitializeComponent();
            this.Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, EventArgs e)
        {
            Application.Current.MainPage.Navigation.PushModalAsync(new TextEditor(fileSaver), true);
        }

    }
}