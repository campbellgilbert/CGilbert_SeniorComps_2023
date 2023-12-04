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
            SmallLabel.Text = "loading...";
            //BIG FUCKING ISSUE: times out if text entered is too long
            entryText = entryText.ReplaceLineEndings(" ");

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
                //SmallLabel.Text = entryText;
                SmallLabel.Text = run.Status.ToString();
                run = await run.UpdateAsync();
            }

            //retrieve messages & print correct msg
            var messageList = await api.ThreadsEndpoint.ListMessagesAsync(thread.Id);
            string output = messageList.Items[0].PrintContent();
            SmallLabel.Text = output;

            this.run = run;
            this.thread = await thread.UpdateAsync();

            //picker.IsVisible = true;
            //MoreFB.IsVisible = true;
            PickEntry.IsVisible = true;
            FollowUp.IsVisible = true;

            /*let's just get the follow

            /*while True:
                run = client.beta.threads.runs.retrieve(thread_id=thread.id, run_id=run.id)
                print("run status", run.status)
                if run.status=="requires_action":
                    function_name, arguments, function_id  = get_function_details(run)
                    function_response = execute_function_call(function_name,arguments)
                    run = submit_tool_outputs(run,thread,function_id,function_response)
                    continue
                if run.status=="completed":
                    messages = client.beta.threads.messages.list(thread_id=thread.id)
                    latest_message = messages.data[0]
                    text = latest_message.content[0].text.value
                    print(text)
                    user_input = input()
                    if user_input == "STOP":
                      break
                    run,thread = create_message_and_run(assistant=assistant,query=user_input,thread=thread)
                    continue;
                time.sleep(1)


            while (PickEntry.Text != "end")
            {
                var newMsg = await thread.CreateMessageAsync(PickEntry.Text);
                var newRun = await thread.CreateRunAsync(assistant);

                while (run.Status != RunStatus.Completed)
                {
                    //SmallLabel.Text = entryText;
                    SmallLabel.Text = run.Status.ToString();
                    run = await run.UpdateAsync();
                }

                //retrieve messages & print correct msg
                messageList = await api.ThreadsEndpoint.ListMessagesAsync(thread.Id);
                output = messageList.Items[0].PrintContent();
                SmallLabel.Text = output;

            }*/

            //end run here???

            return;
        }
        catch
        {
            await DisplayAlert("Uh oh!", "Something went wrong.", ":(");
            return;
        }
    }

   //EVERYTHING'S WORKING THAT SHOULD BE WORKING
   //all that's left is bells, whistles, and making it all look nice

    //this is deeply fucking stupid but: let's put the new info on a new page
    private async void OnFollowUpClicked(object sender, EventArgs e)
    {

        //var thread = await api.ThreadsEndpoint.RetrieveThreadAsync(this.thread.Id);

        //do we need to retrieve thread and run once they've started??
        //send message and run
        
        

        var message = await thread.CreateMessageAsync(PickEntry.Text);
        var run = await thread.CreateRunAsync(assistant);

        while (run.Status != RunStatus.Completed)
        {
            //SmallLabel.Text = entryText;
            //SmallLabel.Text = run.Status.ToString();
            run = await run.UpdateAsync();
        }

        //retrieve messages & print correct msg
        var messageList = await api.ThreadsEndpoint.ListMessagesAsync(thread.Id);
        var output = messageList.Items[0].PrintContent();

        var newWindow = new Window(new FollowupPopup(output));
        Application.Current.OpenWindow(newWindow);

        /*
        var request = followup;
        var message = await thread.CreateMessageAsync(request);
        

        /*
        var run = await api.ThreadsEndpoint.RetrieveRunAsync("thread-id", "run-id");
        // OR use extension method for convenience!
        var run = await thread.RetrieveRunAsync("run-id");
        var run = await run.UpdateAsync();

        picker.IsVisible = false;
        MoreFB.IsVisible = false;
        PickEntry.IsVisible = false;
        FollowUp.Text = "Loading response...";
        


        var newWindow = new Window(new FollowupPopup(this.thread.Id, this.run, this.followup));
        //retrieve thread, run
        //send new message
        //get response
        //send response to pop-up

        //create new pop-up*/


    }

}