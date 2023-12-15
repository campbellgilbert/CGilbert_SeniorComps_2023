using ChatGptNet;
using ChatGptNet.Models;
using Microsoft.Maui.Controls;
using OpenAI.Assistants;
using OpenAI;
using OpenAI.Threads;
using Microsoft.Maui;

namespace ravendesk;
public partial class FeedbackPage : ContentPage
{
    private String entryText;
    private String followupText;
    private OpenAI.OpenAIClient api;
    private ThreadResponse thread;
    private AssistantResponse assistant;
         
    public FeedbackPage()
	{
        InitializeComponent();
        this.Loaded += FeedbackPage_Loaded;
    }

    private async void FeedbackPage_Loaded(object sender, EventArgs e)
    {
        //STEP 1: create (retrieve) assistant
	string apiKey = "!!!FIXME!!! ADD YOUR OWN API KEY HERE";
        var ident = new OpenAIAuthentication(apiKey);
        var api = new OpenAIClient(ident);
        var assistant = await api.AssistantsEndpoint.RetrieveAssistantAsync("asst_ORPuajHCiRDrjjG1eY92ysLO"); //retrieve Huginn model

        //STEP 2: create thread
        var thread = await api.ThreadsEndpoint.CreateThreadAsync();
        this.assistant = assistant;
        this.api = api;
        this.thread = thread;

        //Copy and paste entire file into chat to be reviewed.
        //potentially make it such that you can upload a full file?

        string clipboardText = await Clipboard.Default.GetTextAsync();
        if (!string.IsNullOrEmpty(clipboardText))
        {
            entryText = clipboardText;
        } else
        {
            await DisplayAlert("No text", "Please enter some text", "OK");
            return;
        }

        await GetInitResponse();
    }

    private async Task GetInitResponse()
    {
        try
        {
            entryText = entryText.ReplaceLineEndings(" ");
            
            //retrieve created thread
            var thread = await api.ThreadsEndpoint.RetrieveThreadAsync(this.thread.Id);

            //send message and run
            var request = "Give X pieces of feedback (X being an appropriate number based on the length and intricacy of " +
                "the text, do NOT say what X is) on the following: " + entryText;
            var message = await thread.CreateMessageAsync(request);
            var run = await thread.CreateRunAsync(assistant);

            while (run.Status != RunStatus.Completed)
            {
                SmallLabel.Text = "Response loading...";
                run = await run.UpdateAsync();
            }

            //retrieve messages & print correct msg
            var messageList = await api.ThreadsEndpoint.ListMessagesAsync(thread.Id);
            string output = messageList.Items[0].PrintContent();
            SmallLabel.Text = output;

            this.thread = await thread.UpdateAsync();

            FollowupPicker.IsVisible = true;
            PickEntry.IsVisible = true;
            FollowUpButton.IsVisible = true;

            return;
        }
        catch
        {
            await DisplayAlert("Uh oh!", "Something went wrong.", ":(");
            return;
        }
    }

    //FOLLOWUPS
    private async void OnPickerSelected(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;
        if (selectedIndex != -1)
        {
            followupText = picker.Items[selectedIndex];
        } else
        {
            await DisplayAlert("No selection", "Please select a response", "OK");
            return;
        }
    }

    private async void OnEntryFilled(object sender, EventArgs e)
    {
        var entry = (Entry)sender;
        followupText = followupText + ((Entry)sender).Text + "?";

        if (!string.IsNullOrEmpty(followupText))
        {
            followupText = followupText + ((Entry)sender).Text + "?";
        } else {
            await DisplayAlert("No text", "Please enter some text", "OK");
            return;
        }
    }

    private async void OnFollowUpClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(followupText))
        {
           await DisplayAlert("No text", "Please pick a response and enter appropriate text", "OK");
            return;
        }

        var message = await thread.CreateMessageAsync(followupText);
        var run = await thread.CreateRunAsync(assistant);

        //since we just want to keep the same run, let the button show loading
        while (run.Status != RunStatus.Completed)
        {
            FollowUpButton.Text = "Response loading...";
            run = await run.UpdateAsync();
        }

        //retrieve messages
        var messageList = await api.ThreadsEndpoint.ListMessagesAsync(thread.Id);
        var output = messageList.Items[0].PrintContent();

        //print msg to new window
        var newWindow = new Window(new FollowupPopup(output, "Continued Feedback"));
        newWindow.MaximumHeight = 900;
        newWindow.MaximumWidth = 400;
        Application.Current.OpenWindow(newWindow);

        FollowUpButton.Text = "Get follow-up suggestions";
    }
   
}
