using ChatGptNet;
using ChatGptNet.Models;
using Microsoft.Maui.Controls;

namespace ravendesk;

public partial class CopilotDEMOPage : ContentPage
{
    private IChatGptClient _chatGptClient;

    public CopilotDEMOPage()
	{
		InitializeComponent();
        this.Loaded += CopilotDEMOPage_Loaded;
    }

    private void CopilotDEMOPage_Loaded(object sender, EventArgs e)
    {
        _chatGptClient = Handler.MauiContext.Services.GetService<IChatGptClient>();
    }
    private async void OnFeedbackClicked(object sender, EventArgs e)
    {
        await GetTest();
    }

    private async Task GetTest()
    {
        if (string.IsNullOrWhiteSpace(TextEntry.Text))
        {
            await DisplayAlert("No text", "Please enter some text", "OK");
            return;
        }

        
        string genre = Microsoft.Maui.Storage.Preferences.Default.Get("Genre", "Realistic");
        string vibes = Microsoft.Maui.Storage.Preferences.Default.Get("Vibe", "'Ulysses' by James Joyce, " +
            "'Beloved' by Toni Morrison, and 'Anna Karenina' by Leo Tolstoy");
        string type = Microsoft.Maui.Storage.Preferences.Default.Get("Type", "Short Story");
        string summary = Microsoft.Maui.Storage.Preferences.Default.Get("Summary", "just a chill lowkey guy who " +
            "does his own thing and doesnt give a fuck about what anyone thinks");

        _sessionGuid = Guid.NewGuid();

        try
        {
            //FIXME: REFACTOR TO USE ASSISTANT API
            SmallLabel.Text = "Loading response...";
            ChatGptResponse response = await _chatGptClient.AskAsync(_sessionGuid,
                "You are a writer and editor who is very well-experienced in and passionate about " + genre +
                ". Some of your favorite things include " + vibes +
                ". You are intuitive, creative, and deeply passionate about your craft. You have high standards for yourself" +
                " and your peers. You are editing a/an " + type + " about " + summary +
                ".  Give 6 pieces of actionable, specific feedback, including guiding questions, about the following section" +
                " of writing, taken from the aforementioned work: " + TextEntry.Text);

            /*
             * "You are a writer and editor who is very well-experienced in and passionate about [genre]. Some of your favorite things include [works].
             You are intuitive, creative, and deeply passionate about your craft. You have high standards for yourself and your peers. 
             You are editing a/an [piece type] about [summary]. Give 6 pieces of actionable, specific feedback, including guiding questions, 
             about the following section, taken from the aforementioned work: " 
             */
            var message = response.GetContent();
            SmallLabel.Text = message;
            return;
        }
        catch
        {
            await DisplayAlert("oopsy woopsy!!!", "i did a widdle fucky wucky :(", "OK");
            return;
        }
        //private Guid _sessionGuid = Guid.Empty;
    }


    

    private Guid _sessionGuid = Guid.Empty;


}