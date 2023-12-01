using ChatGptNet;
using ChatGptNet.Models;
using Microsoft.Maui.Controls;

namespace ravendesk;

public partial class CopilotDEMOPage : ContentPage
{
    private string entryText;
    private IChatGptClient _chatGptClient;

    public CopilotDEMOPage()
    {
        InitializeComponent();
        this.Loaded += CopilotDEMOPage_Loaded;
    }

    private async void CopilotDEMOPage_Loaded(object sender, EventArgs e)
    {
        string clipboardText = await Clipboard.Default.GetTextAsync();
        if (!string.IsNullOrEmpty(clipboardText))
        {
            entryText = clipboardText;
        }
        else
        {
            await DisplayAlert("No text", "Please enter some text", "OK");
            return;
        }

        _chatGptClient = Handler.MauiContext.Services.GetService<IChatGptClient>();
        await GetTest();
    }

    private async Task GetTest()
    {
        _sessionGuid = Guid.NewGuid();

        try
        {
            //FIXME: REFACTOR TO USE ASSISTANT API
            SmallLabel.Text = "Loading response...";
            
            ChatGptResponse response = await _chatGptClient.AskAsync(_sessionGuid,
                "Persona:You are a writer and editor who is very well-experienced in and passionate about many genres. " +
                "You are intuitive, creative, and deeply passionate about your craft. You have high standards for yourself and your peers. " +
                "Some of your favorite works include Beloved by Toni Morrison, Finnegans Wake by James Joyce, and This Is How You Lose The Time War by Amal el-Mohtar and Max Gladstone. " +
                "Your feedback is specific, focused, and actionable, and always peppered with guiding questions." +
                "Give X pieces of feedback (X being an appropriate number based on the length and intricacy of the text, do NOT say what X is, just give the feedback) " +
                "on the following: " + entryText);

           
            var message = response.GetContent().ToString();
            SmallLabel.Text = message;
            return;
        }
        catch
        {
            await DisplayAlert("oopsy woopsy!!!", "i did a widdle fucky wucky :(", "OK");
            return;
        }
    }



    private Guid _sessionGuid = Guid.Empty;


}