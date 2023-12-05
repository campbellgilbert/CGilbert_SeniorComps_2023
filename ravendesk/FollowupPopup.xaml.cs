using OpenAI;
using OpenAI.Assistants;
using OpenAI.Threads;
using System.Runtime.CompilerServices;

namespace ravendesk;

public partial class FollowupPopup : ContentPage
{

    public FollowupPopup(string response, string title)
    {
        InitializeComponent();
        MainLabel.Text = response;
        Title.Text = title;
    }

    public void DisplayText(string text)
    {
        MainLabel.Text = text; 
    }
}