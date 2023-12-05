namespace ravendesk
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            this.Loaded += MainPage_Loaded;

        }

        private void MainPage_Loaded(object sender, EventArgs e)
        {
            Application.Current.MainPage.Navigation.PushModalAsync(new TextEditor(), true);
        }
        /*
            private void OnFileSelectButtonClicked(object sender, EventArgs e)
        {
            Application.Current.MainPage.Navigation.PushModalAsync(new FileSelectPage(), true);
        }

        private void OnTextEditButtonClicked(object sender, EventArgs e)
        {
            Application.Current.MainPage.Navigation.PushModalAsync(new TextEditor(), true);
        }

        private void OnCopilotButtonClicked(object sender, EventArgs e)
        {
            Application.Current.MainPage.Navigation.PushModalAsync(new CopilotPage(), true);
        }*/
    }
}