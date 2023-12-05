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

    public TextEditor(/*IFileSaver fileSaver, IFolderPicker folderPicker*/)
    {
        //this.fileSaver = fileSaver;
        //this.folderPicker = folderPicker;
        InitializeComponent();
        this.Loaded += TextEditor_Loaded;
    }
    private async void TextEditor_Loaded(object sender, EventArgs e)
    {
        await Clipboard.Default.SetTextAsync(null);
    }

    private int _cursorPosition = 0;
    public int CursorPosition
    {
        get => _cursorPosition;
        set
        {
            _cursorPosition = value;
            OnPropertyChanged(nameof(CursorPosition));
            OnPropertyChanged(nameof(SelectedText));
        }
    }

    private int _selectionLength = 0;
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

    private string _text = "Hello World";
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

    //startindex longer than length of string
    public string SelectedText
    {
        get
        {
            if (Text == null || Text == string.Empty)
                return string.Empty;
            if (SelectionLength <= 0)
                return string.Empty;
            if (CursorPosition + SelectionLength > Text.Length)
                return string.Empty;
            
            return textEditor.Text.Substring(CursorPosition, SelectionLength);
        }
    }
    /*
    public string SelectedText
    {
        get
        {
            if (Text == null || Text == string.Empty)
                return string.Empty;
            if (textEditor.SelectionLength <= 0)
                return string.Empty;

            //start at 20, move to 50
            //cursorpos = 50, sellen = 30
            //start at cursorPos - selLen, length selectionLength

            //start at 3, move to 5
            //cursorpos = 5, sellen = 2
            //start at cursorPos - selLen = 3, length selectionLength
            //when moving right, cursorpos will always be more or equal to sellen

            //cursorpos 105, sellen = 2
            //but what if we start at 107?? how do we know if we started at 107 or 107???
            //if cursorposition
            //okay maybe we can only select left to right. that's fine. this is fine

            //start at cursorpos, length sellen
            /*

            if (textEditor.CursorPosition >= textEditor.SelectionLength)
            {
                return Text.Substring(textEditor.CursorPosition - textEditor.SelectionLength, textEditor.SelectionLength);
            } //if (textEditor.CursorPosition < textEditor.SelectionLength + textEditor.CursorPosition)
            
            //start at 50, move to 20
            //cursorpos = 20, sellen = 30
            //start atcursorPos, length selectionLength

            //start at 30, move to 5
            //cursorpos = 5, sellen = 25
            //start at cursorpos, length 

            //start at 107, move to 105
            //cursorpos = 105, sellen = 2
            //start at cursorpos, end at sellen


            return Text.Substring(textEditor.CursorPosition, textEditor.SelectionLength);
        }
    }
    private async void OnCopyClicked(object sender, EventArgs e)
    {
        int selStart = textEditor.CursorPosition;
        int selEnd = textEditor.SelectionLength;

        int cursPos = textEditor.CursorPosition;
        int selLen = textEditor.SelectionLength;

        //move right -- start at 15, move to 20
        //cursPos = 20, selectionlength = 5, selLen + cursPos = 20
        //selStart = cursPos - selLen
        //selEnd = cursPos
        /*

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
    
    */
    //startindex longer than length of string


    private async void OnCopyClicked(object sender, EventArgs e)
    {
        int start = textEditor.CursorPosition;
        int length = textEditor.SelectionLength;
        string sel = "";

        sel = textEditor.Text.Substring(start, length);
        await Clipboard.Default.SetTextAsync(sel);
        /*
        if (textEditor.CursorPosition > textEditor.SelectionLength)
            sel = Text.Substring(CursorPosition, SelectionLength);
        else
            sel = Text.Substring(textEditor.CursorPosition, textEditor.SelectionLength);*/
        //await DisplayAlert("No text", "selLen: " + textEditor.SelectionLength + "; cursorPos: " + textEditor.CursorPosition, "OK");

        //await DisplayAlert("hehehe", "selLen: " + textEditor.SelectionLength + "; cursorPos: " + textEditor.CursorPosition + "\n sel : " + sel, "OK");
        return;
    }

    private async void OnCutClicked(object sender, EventArgs e)
    {
        int start = textEditor.CursorPosition;
        int length = textEditor.SelectionLength;

        //if no text is selected -- just paste wherever you are
        string beforeCursor = textEditor.Text.Substring(0, start);
        string afterCursor = textEditor.Text.Substring(start + length);

        textEditor.Text = beforeCursor + afterCursor;
        return;
    }



    private async void OnPasteClicked(object sender, EventArgs e)
    {
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

        if (textEditor.Text == null)
        {
            textEditor.Text = insert;
            return;
        }

        int start = textEditor.CursorPosition;

        //if no text is selected -- just paste wherever you are
        string beforeCursor = textEditor.Text.Substring(0, start);
        string afterCursor = textEditor.Text.Substring(start);

        textEditor.Text = beforeCursor + insert + afterCursor;
        return;
    }


    private async void OnFullFeedbackClicked(object sender, EventArgs e)
    {
        await Clipboard.Default.SetTextAsync(textEditor.Text);
        var newWindow = new Window(new CopilotPage());
        Application.Current.OpenWindow(newWindow);
    }

    private void OnPartialFeedbackClicked(object sender, EventArgs e)
    {
        //await Clipboard.Default.SetTextAsync(SelectedText);
        OnCopyClicked(sender, e);
        var newWindow = new Window(new CopilotPage());
        Application.Current.OpenWindow(newWindow);
    }
}