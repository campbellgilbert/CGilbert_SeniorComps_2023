using OpenAI;
using OpenAI.Assistants;
using OpenAI.Threads;
using System.Runtime.CompilerServices;

namespace ravendesk;

public partial class FollowupPopup : ContentPage
{

    public FollowupPopup(string response)
    {
        InitializeComponent();
        MainLabel.Text = response;
    }

}