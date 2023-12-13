using CommunityToolkit.Maui.Media;
using CommunityToolkit.Maui.Storage;

namespace ravendesk
{
    //DEPRECATED
    //can't remove or change name of mainpage, so this stays such that code arch remains the same.
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