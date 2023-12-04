using OpenAI;
using OpenAI.Assistants;
using OpenAI.Threads;

namespace ravendesk;

public partial class FollowupPopup : ContentPage
{
    private OpenAI.OpenAIClient api;
    private string threadID;
    private string runID;
    private string text;
    private ThreadResponse thread;
    private AssistantResponse assistant;
    private RunResponse run;
    
    public FollowupPopup(string threadID, string runID, string text)
	{
        InitializeComponent();
        this.threadID = threadID;
        this.runID = runID;
        this.text = text;
        this.Loaded += FollowupPopup_Loaded;
	}
    private async void FollowupPopup_Loaded(object Sender, EventArgs e)
    {
        //STEP 1: retrieve api, assistant, thread, and run

        var ident = new OpenAIAuthentication("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");
        this.api = new OpenAIClient(ident);
        this.assistant = await api.AssistantsEndpoint.RetrieveAssistantAsync("asst_ORPuajHCiRDrjjG1eY92ysLO");
        this.thread = await api.ThreadsEndpoint.RetrieveThreadAsync(threadID);

        //step 2: pass to response to display the response
    }



    public async Task DisplayResponse()
    {
        var message = await this.thread.CreateMessageAsync(this.text);
        this.run = await api.ThreadsEndpoint.RetrieveRunAsync(threadID, runID);
        this.run = await run.UpdateAsync();
        while (run.Status != RunStatus.Completed)
        {
            MainLabel.Text = "Response loading...";
            run = await run.UpdateAsync();
        }
        var messageList = await api.ThreadsEndpoint.ListMessagesAsync(thread.Id);
        string output = messageList.Items[0].PrintContent();
        MainLabel.Text = output;
        /*
         var request = "Give X pieces of feedback (X being an appropriate number based on the length and intricacy of " +
                "the text, do NOT say what X is, just give the feedback on the following: " + entryText;
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
        
         */
        MainLabel.Text = "yippee!";
    }
}