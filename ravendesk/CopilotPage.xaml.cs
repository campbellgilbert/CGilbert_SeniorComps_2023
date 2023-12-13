using ChatGptNet;
using ChatGptNet.Models;
using Microsoft.Maui.Controls;
using OpenAI.Assistants;
using OpenAI;
using OpenAI.Threads;
using Microsoft.Maui;
using OpenAI.Files;
//using AuthenticationServices;

namespace ravendesk;

public partial class CopilotPage : ContentPage
{
    private String entryText;
    private String followupText;
    private OpenAI.OpenAIClient api;
    private ThreadResponse thread;
    private AssistantResponse assistant;
    private string filepath;
         
    public CopilotPage()
	{
        InitializeComponent();

        //if no file is passed in, clear it out so nothing gets left over + delete any prev files from the assistant
        this.filepath = null; 
        this.Loaded += CopilotDEMOPage_Loaded;
    }

    public CopilotPage(string filepath)
    {
        InitializeComponent();
        this.filepath = filepath;
        this.Loaded += CopilotDEMOPage_Loaded;
    }

    private async void CopilotDEMOPage_Loaded(object sender, EventArgs e)
    {
        //STEP 1: create (retrieve) assistant
        var ident = new OpenAIAuthentication("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");
        this.api = new OpenAIClient(ident);
        this.assistant = await api.AssistantsEndpoint.RetrieveAssistantAsync("asst_ORPuajHCiRDrjjG1eY92ysLO");

            //STEP 1.5: delete any previous assistant files to clear things out
        /*var filesList = await assistant.ListFilesAsync();
        if (filesList.Items.Count > 0)
        {
            foreach (AssistantFileResponse file in filesList.Items)
            {
                bool isDeleted = await assistant.DeleteFileAsync(file.Id);
            }
        }*/

        //STEP 2: create thread
        this.thread = await api.ThreadsEndpoint.CreateThreadAsync();

        //STEP 3: Either copy and paste all text, or input file, and get feedback.
        string clipboardText = await Clipboard.Default.GetTextAsync();
        if (filepath != null)
        {
            await GetInitResponse(filepath);
        }
        else if (!string.IsNullOrEmpty(clipboardText))
        {
            entryText = clipboardText;  
            await GetInitResponse();
        } 
        else
        {
            await DisplayAlert("No text", "Please upload the file or enter some text", "OK");
            return;
        }
    }

    private async Task GetInitResponse()
    {
        try
        {
            entryText = entryText.ReplaceLineEndings(" ");

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

    //Inputs an entire .txt file to the assistant to get a response. To be used for longer inputs.
    private async Task GetInitResponse(string filepath)
    {
        try
        {
            //retrieve thread
            var thread = await api.ThreadsEndpoint.RetrieveThreadAsync(this.thread.Id);
            try
            {
                //upload file to assistant
                var assistantFile = await assistant.UploadFileAsync(filepath);
                
            } catch
            {
                await DisplayAlert("Uh oh!", "Issue with file uploading file at: " + filepath, "OK");
                return;
            }
            //send message and run
            var request = "Give X pieces of feedback (X being an appropriate number based on the length and intricacy of " +
                "the text, do NOT say what X is) on the file attached.";
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

        //FIXME: create new window, have it display the loading screen, then show completed text
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