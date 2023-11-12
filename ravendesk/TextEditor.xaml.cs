namespace ravendesk;

public partial class TextEditor : ContentPage
{
    public TextEditor()
    {
        InitializeComponent();
    }

    private void OnSaveClicked(object sender, EventArgs e)
    {
        string text = textEditor.Text;
        File.WriteAllText("savedDocument.txt", text);
    }

    private void OnFileSelectButtonClicked(object sender, EventArgs e)
    {
        Application.Current.MainPage.Navigation.PushModalAsync(new FileSelectPage(), true);
    }

    private void OnCopilotClicked(object sender, EventArgs e)
    {
        var newWindow = new Window(new CopilotDEMOPage());
        Application.Current.OpenWindow(newWindow);
    }
}