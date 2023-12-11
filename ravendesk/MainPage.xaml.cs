using CommunityToolkit.Maui.Media;

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
            //DisplayAlert("No text", "SOMETHING HAS GONE HORRIBLY WRONG", "OK");
            ISpeechToText speechToText = new ISpeechToText();
            Application.Current.MainPage.Navigation.PushModalAsync(new TextEditor(speechToText), true);
        }
        private void OnTextEditorClicked(object sender, EventArgs e)
        {
            Application.Current.MainPage.Navigation.PushModalAsync(new TextEditor(speechToText), true);
        }
    }
}