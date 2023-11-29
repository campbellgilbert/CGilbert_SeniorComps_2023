using ChatGptNet;
using ChatGptNet.Models;
using Microsoft.Maui.Controls;
using OpenAI.Assistants;
using OpenAI;
using OpenAI.Threads;

namespace ravendesk;

public partial class CopilotDEMOPage : ContentPage
{
    private IChatGptClient _chatGptClient;
    private String entryText;
    private String followupText;
    private OpenAI.OpenAIClient api;
    private string threadID;

    public CopilotDEMOPage()
	{
		InitializeComponent();
        this.Loaded += CopilotDEMOPage_Loaded;

    }

    private async void CopilotDEMOPage_Loaded(object sender, EventArgs e)
    {
        //create assistant

        var ident = new OpenAIAuthentication("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");
        var api = new OpenAIClient(ident);
        string aiPersona = "You are a writer and editor who is very well-experienced in and passionate about many genres. " +
                "You are intuitive, creative, and deeply passionate about your craft. You have high standards for yourself and your peers. " +
                "Some of your favorite works include Beloved by Toni Morrison, Finnegans Wake by James Joyce, and This Is How You Lose The Time War " +
                "by Amal el-Mohtar and Max Gladstone. Your feedback is specific, focused, and actionable, and always peppered with guiding questions. ";

        var assistant = await api.AssistantsEndpoint.CreateAssistantAsync(
            new CreateAssistantRequest(
                name: "Raven",
                instructions: aiPersona,
                model: "gpt-4"));
        this.api = api;
        var thread = await api.ThreadsEndpoint.CreateThreadAsync();
        this.threadID = thread.Id;

        /*var api = new OpenAIClient();
        var assistant = await api.AssistantsEndpoint.CreateAssistantAsync(
            new CreateAssistantRequest(
                name: "Math Tutor",
                instructions: "You are a personal math tutor. Answer questions briefly, in a sentence or less.",
                model: "gpt-4-1106-preview"));
        var messages = new List<Message> { "I need to solve the equation `3x + 11 = 14`. Can you help me?" };
        var threadRequest = new CreateThreadRequest(messages);
        var run = await assistant.CreateThreadAndRunAsync(threadRequest);
        Console.WriteLine($"Created thread and run: {run.ThreadId} -> {run.Id} -> {run.CreatedAt}");*/

        //FIXME -- figure out some way to do copy/paste so user can put in their own stuff instead 
        //text entry automatically has the cliipboard text
        string clipboardText = await Clipboard.Default.GetTextAsync();
        if (!string.IsNullOrEmpty(clipboardText))
        {
            entryText = clipboardText;
        } else
        {
            entryText = "the quick brown fox jumped over the lazy dog.";
            //await DisplayAlert("No text", "Please enter some text", "OK");
            return;
        }

        //_chatGptClient = Handler.MauiContext.Services.GetService<IChatGptClient>();
        await GetInitResponse();
    }

    private async Task GetInitResponse()
    {
        _sessionGuid = Guid.NewGuid();

        try
        {
            SmallLabel.Text = "Loading response...";


            //FIXME: REFACTOR TO USE ASSISTANT API
            //retrieve assistant/thread
            var thread = await api.ThreadsEndpoint.RetrieveThreadAsync(threadID);
            var request = new CreateMessageRequest("Give X pieces of feedback (X being an appropriate number based on the length and intricacy of " +
                "the text, do NOT say what X is, just give the feedback on the following: " + entryText);
            var message = await thread.CreateMessageAsync(request);

            /*
            var thread = await api.ThreadsEndpoint.CreateThreadAsync();
            var request = new CreateMessageRequest("Hello world!");
            var message = await api.ThreadsEndpoint.CreateMessageAsync(thread.Id, request);
            // OR use extension method for convenience!
            var message = await thread.CreateMessageAsync("Hello World!");
            Console.WriteLine($"{message.Id}: {message.Role}: {message.PrintContent()}");
             
            ChatGptResponse response = await _chatGptClient.AskAsync(_sessionGuid,
                "Persona:You are a writer and editor who is very well-experienced in and passionate about many genres. " + 
                "You are intuitive, creative, and deeply passionate about your craft. You have high standards for yourself and your peers. " +
                "Some of your favorite works include Beloved by Toni Morrison, Finnegans Wake by James Joyce, and This Is How You Lose The Time War by Amal el-Mohtar and Max Gladstone. " +
                "Your feedback is specific, focused, and actionable, and always peppered with guiding questions." +
                "Give X pieces of feedback (X being an appropriate number based on the length and intricacy of the text, do NOT say what X is, just give the feedback) " +
                "on the following: " + entryText);

            */
            SmallLabel.Text = message.ToString();
            picker.IsVisible = true;
            MoreFB.IsVisible = true;
            PickEntry.IsVisible = true;
            return;
        }
        catch
        {
            await DisplayAlert("oopsy woopsy!!!", "i did a widdle fucky wucky :(", "OK");
            return;
        }
    }

    private void OnPickerSelected(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;
        if (selectedIndex != -1)
        {
            followupText = picker.Items[selectedIndex];
        }
    }

    private async void OnEntryFilled(object sender, EventArgs e)
    {
        followupText = followupText + ((Entry)sender).Text + "?";
        await GetFollowupResponse();
    }


    private async Task GetFollowupResponse()
    {
        _sessionGuid = Guid.NewGuid();

        try
        {
            //FIXME: REFACTOR TO USE ASSISTANT API
            //retrieve assistant
            //retrieve thread

            SmallLabel.Text = "Loading response...";
            ChatGptResponse response = await _chatGptClient.AskAsync(_sessionGuid,
                "Persona:You are a writer and editor who is very well-experienced in and passionate about many genres. " +
                "You are intuitive, creative, and deeply passionate about your craft. You have high standards for yourself and your peers. " +
                "Some of your favorite works include Beloved by Toni Morrison, Finnegans Wake by James Joyce, and This Is How You Lose The Time War by Amal el-Mohtar and Max Gladstone. " +
                "Your feedback is specific, focused, and actionable, and always peppered with guiding questions." +
                "Give X pieces of feedback (X being an appropriate number based on the length and intricacy of the text, do NOT say what X is, just give the feedback) " +
                "on the following: " + entryText);


            var message = response.GetContent();
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