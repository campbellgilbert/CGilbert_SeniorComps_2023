using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Storage;
using Microsoft.Maui.Controls;
using System.Text;
using System.Threading;

namespace ravendesk;

public partial class TextEditor : ContentPage
{
    //private readonly IFileSaver fileSaver;
    //private readonly IFolderPicker folderPicker;
    private TextEditor editor;
    public TextEditor(/*IFileSaver fileSaver, IFolderPicker folderPicker*/) {
        //this.fileSaver = fileSaver;
        //this.folderPicker = folderPicker;
        InitializeComponent();
    }

    void OnEditorTextChanged(object sender, TextChangedEventArgs e)
    {
        string oldText = e.OldTextValue;
        string newText = e.NewTextValue;
        string myText = textEditor.Text;
    }

    private async void OnCopyClicked(object sender, EventArgs e)
    {
        await Clipboard.Default.SetTextAsync(textEditor.Text);
        return;
    }

    private void OnCopilotClicked(object sender, EventArgs e)
    {
        //FIXME -- this just copies the whole fucking thing bruh there's gotta be a better way
        OnCopyClicked(sender, e);
        //FIXME -- paste highlighted text to new copilot
        var newWindow = new Window(new CopilotDEMOPage());
        Application.Current.OpenWindow(newWindow);
    }
}