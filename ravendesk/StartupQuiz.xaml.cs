namespace ravendesk;

public partial class StartupQuiz : ContentPage
{
	public StartupQuiz()
	{
		InitializeComponent();
	}

    private void OnSaveClicked(object sender, EventArgs e)
    {
        // Save multiple properties
        Microsoft.Maui.Storage.Preferences.Default.Set("Genre", GenreEntry.Text);
        Microsoft.Maui.Storage.Preferences.Default.Set("Vibe", VibeEntry.Text);
        Microsoft.Maui.Storage.Preferences.Default.Set("Type", TypeEntry.Text);
        Microsoft.Maui.Storage.Preferences.Default.Set("Summary", SummaryEntry.Text);
        // Add more properties as needed
        //Microsoft.Maui.Storage.Preferences.
        //Application.Current.SavePropertiesAsync(); // Asynchronously save the properties
        Application.Current.MainPage.Navigation.PushModalAsync(new TextEditor(), true);
    }

    
}