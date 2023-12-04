using ChatGptNet;
using ChatGptNet.Models;
using Microsoft.Maui.Controls;
using OpenAI.Assistants;
using OpenAI;
using OpenAI.Threads;

namespace ravendesk;

public partial class CopilotDEMOPage : ContentPage
{
    private String entryText;
    private String followupText;
    private OpenAI.OpenAIClient api;
    private ThreadResponse thread;
    private AssistantResponse assistant;
    private RunResponse run;
    private string followup;
         
    public CopilotDEMOPage()
	{
        InitializeComponent();
        this.Loaded += CopilotDEMOPage_Loaded;

    }

    private async void CopilotDEMOPage_Loaded(object sender, EventArgs e)
    {
        //STEP 1: create (retrieve) assistant
        var ident = new OpenAIAuthentication("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");
        var api = new OpenAIClient(ident);
        var assistant = await api.AssistantsEndpoint.RetrieveAssistantAsync("asst_ORPuajHCiRDrjjG1eY92ysLO");


        //STEP 2: create thread and run
        var thread = await api.ThreadsEndpoint.CreateThreadAsync();
        this.assistant = assistant;
        this.api = api;
        this.thread = thread;


        //Copy and paste entire file into chat to be reviewed.
        //Currently no way to do a single section
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
            //BIG FUCKING ISSUE: times out if text entered is too long

            //trunctuate text if it's too long?
            //retrieve thread
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

            this.run = run;
            this.thread = await thread.UpdateAsync();


            picker.IsVisible = true;
            MoreFB.IsVisible = true;
            PickEntry.IsVisible = true;
            FollowUp.IsVisible = true;
            return;
        }
        catch
        {
            await DisplayAlert("Uh oh!", "Something went wrong.", ":(");
            return;
        }
    }

    //continued: have AI create possible follow ups
    
    private async void OnPickerSelected(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;
        if (selectedIndex != -1)
        {
            this.followup = picker.Items[selectedIndex] + this.followup;
        }
        else
        {
            await DisplayAlert("No selection", "Please select an option.", "OK");
            return;
        }
    }

    private async void OnEntryFilled(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(((Entry)sender).Text))
        {
            this.followup = this.followup + ((Entry)sender).Text + "?";
        } else
        {
            await DisplayAlert("No text", "Please enter some text.", "OK");
            return;
        }
        
    }

    //this is deeply fucking stupid but: let's put the new info on a new page
    private void OnFollowUpClicked(object sender, EventArgs e)
    {
        picker.IsVisible = false;
        MoreFB.IsVisible = false;
        PickEntry.IsVisible = false;
        FollowUp.Text = "Loading response...";
        


        var newWindow = new Window(new FollowupPopup(this.thread.Id, this.run, this.followup));
        //retrieve thread, run
        //send new message
        //get response
        //send response to pop-up

        //create new pop-up


    }

}