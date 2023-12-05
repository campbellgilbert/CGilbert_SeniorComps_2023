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
    private int _cursorPosition = 0;
    private int _selectionLength = 0;
    private string _text = "Hello World";



    public TextEditor(/*IFileSaver fileSaver, IFolderPicker folderPicker*/)
    {
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

    /*public int CursorPosition
    {
        get => _cursorPosition;
        set
        {
            _cursorPosition = value;
            OnPropertyChanged(nameof(CursorPosition));
            OnPropertyChanged(nameof(SelectedText));
        }
    }

    public int SelectionLength
    {
        get => _selectionLength;
        set
        {
            _selectionLength = value;
            OnPropertyChanged(nameof(SelectionLength));
            OnPropertyChanged(nameof(SelectedText));
        }
    }
    */
    public string Text
    {
        get => _text;
        set
        {
            _text = value;
            OnPropertyChanged(nameof(Text));
            OnPropertyChanged(nameof(SelectedText));
        }
    }

    public string SelectedText
    {
        get
        {
            if (Text == null || Text == string.Empty)
                return string.Empty;
            if (textEditor.SelectionLength <= 0)
                return string.Empty;
            /*if (textEditor.CursorPosition + textEditor.SelectionLength > Text.Length)
                return string.Empty;*/
            return Text.Substring(textEditor.CursorPosition, textEditor.SelectionLength);
        }
    }

    //
    private async void OnCopyClicked(object sender, EventArgs e)
    {
        int selStart;
        int selEnd;

        int cursPos = textEditor.CursorPosition;
        int selLen = textEditor.SelectionLength;

        //move right -- start at 15, move to 20
        //cursPos = 20, selectionlength = 5, selLen + cursPos = 20
        //selStart = cursPos - selLen
        //selEnd = cursPos


        if (cursPos == cursPos + selLen)
        {
            selStart = cursPos - selLen;
            selEnd = cursPos;
        }
        //move left -- start at 20, move to 15
        //cursPos = 15, selectionlength = 5, selLen + cursPos = 20
        //selStart = cursPos
        //selEnd = cursPos + selLen
        else
        {
            selStart = cursPos;
            selEnd = cursPos + selLen;

        }
        //problem is in moving to right

        string sel = Text.Substring(selStart, selEnd);
        await Clipboard.Default.SetTextAsync(sel);
        //await Clipboard.Default.SetTextAsync("This text was highlighted in the UI.");
        return;
    }

    //we don't NEED this but we DO need the selectedText to work. like, real bead. 
    private async void OnPasteClicked(object sender, EventArgs e)
    {
        //FIXME -- always pastes at position 0
        if (textEditor.CursorPosition < 0)
        {
            return;
        }
        string clipboardText = await Clipboard.Default.GetTextAsync();
        string insert;
        if (!string.IsNullOrEmpty(clipboardText))
        {
            insert = clipboardText;
        }
        else
        {
            await DisplayAlert("No text", "Please enter some text", "OK");
            return;
        }
        /*
        string beforeCursor = textEditor.Text.Substring(0, textEditor.CursorPosition);
        string afterCursor = textEditor.Text.Substring(textEditor.CursorPosition);
        */

        string beforeCursor = textEditor.Text.Substring(0, textEditor.CursorPosition);
        string afterCursor = textEditor.Text.Substring(textEditor.CursorPosition);


        //theres gotta be a better fucking way to do this dawg

        //textEditor.Text = textEditor.Text.Substring(0, CursorPosition) + insert + textEditor.Text.Substring(CursorPosition);

        textEditor.Text = beforeCursor + insert + afterCursor;
        return;
    }


    private async void OnFullFeedbackClicked(object sender, EventArgs e)
    {
        await Clipboard.Default.SetTextAsync(textEditor.Text);
        var newWindow = new Window(new CopilotPage());
        Application.Current.OpenWindow(newWindow);
    }

    private async void OnPartialFeedbackClicked(object sender, EventArgs e)
    {
        await Clipboard.Default.SetTextAsync(SelectedText);
        var newWindow = new Window(new CopilotPage());
        Application.Current.OpenWindow(newWindow);
    }
}