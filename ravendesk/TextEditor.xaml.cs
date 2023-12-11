using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Media;
using CommunityToolkit.Maui.Storage;
using Microsoft.Maui.Controls;
using System.Globalization;
using System.Text;
using System.Threading;
using CommunityToolkit.Maui;
using System.Collections.Generic;
using Microsoft.Maui.Graphics;
//using Windows.Globalization;
using System.Globalization;

namespace ravendesk;

public partial class TextEditor : ContentPage
{
    //private readonly IFileSaver fileSaver;
    //private readonly IFolderPicker folderPicker;
    private TextEditor editor;

    private readonly ISpeechToText speechToText;

    public Command ListenCommand { get; set; }
    public Command ListenCancelCommand { get; set; }
    public string RecognitionText { get; set; }
    public TextEditor(ISpeechToText speechToText/*IFileSaver fileSaver, IFolderPicker folderPicker*/)
    {
        //this.fileSaver = fileSaver;
        //this.folderPicker = folderPicker;
        InitializeComponent();
        this.speechToText = speechToText;

        /*ListenCommand = new Command(Listen);
            ListenCancelCommand = new Command(ListenCancel);
        BindingContext = this;*/

        this.Loaded += TextEditor_Loaded;
    }
    private async void TextEditor_Loaded(object sender, EventArgs e)
    {
        await Clipboard.Default.SetTextAsync(null);
    }

    //HELPER FUNCTIONS
    public string SelectText(object sender, EventArgs e)
    {
        string sel = "";
        int start = textEditor.CursorPosition;
        int length = textEditor.SelectionLength;
        sel = textEditor.Text.Substring(start, length);
        return sel;
    }

    //FLYOUT MENU METHODS
    private async void OnCopyClicked(object sender, EventArgs e)
    {
        string sel = SelectText(sender, e);
        await Clipboard.Default.SetTextAsync(sel);
        return;
    }

    private async void OnCutClicked(object sender, EventArgs e)
    {
        int start = textEditor.CursorPosition;
        int length = textEditor.SelectionLength;
        
        //if no text is selected -- just paste wherever you are
        string beforeCursor = textEditor.Text.Substring(0, start);
        string afterCursor = textEditor.Text.Substring(start + length);

        string sel = SelectText(sender, e);
        await Clipboard.Default.SetTextAsync(sel);

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

        if (!string.IsNullOrEmpty(clipboardText)) {
            insert = clipboardText;
        } else {
            await DisplayAlert("No text", "Please enter some text", "OK");
            return;
        }

        if (textEditor.Text == null) {
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

    //BUTTON METHODS

    private async void OnSTTClicked(object sender, EventArgs e)
    {
        //Talkstart.IsVisible = false;
        //Talkend.IsVisible = true;
        int start = textEditor.CursorPosition;
        string beforeCursor = textEditor.Text.Substring(0, start);
        string afterCursor = textEditor.Text.Substring(start);
        CancellationToken cancellationToken = new CancellationToken();
        await StartListening(cancellationToken);
        textEditor.Text = beforeCursor + RecognitionText + afterCursor;

        //text editor dot text at start plus equals recognition text
        //Talkstart.IsVisible = true;
        //Talkend.IsVisible = false;
    }

    /*private async void OnSTTEndClicked(object sender, EventArgs e)
    {
        //await SpeechToText.StopListenAsync(CancellationToken.None);
        await StopListening()
    }*/

    async Task StartListening(CancellationToken cancellationToken)
    {
        var isGranted = await speechToText.RequestPermissions(cancellationToken);
        if (!isGranted)
        {
            await Toast.Make("Permission not granted").Show(CancellationToken.None);
            return;
        }

        speechToText.RecognitionResultUpdated += OnRecognitionTextUpdated;
        speechToText.RecognitionResultCompleted += OnRecognitionTextCompleted;
        await SpeechToText.StartListenAsync(CultureInfo.CurrentCulture, CancellationToken.None);
    }

    async Task StopListening(CancellationToken cancellationToken)
    {
        await SpeechToText.StopListenAsync(CancellationToken.None);
        SpeechToText.Default.RecognitionResultUpdated -= OnRecognitionTextUpdated;
        SpeechToText.Default.RecognitionResultCompleted -= OnRecognitionTextCompleted;
    }

    void OnRecognitionTextUpdated(object? sender, SpeechToTextRecognitionResultUpdatedEventArgs args)
    {
        RecognitionText += args.RecognitionResult;
    }

    void OnRecognitionTextCompleted(object? sender, SpeechToTextRecognitionResultCompletedEventArgs args)
    {
        RecognitionText = args.RecognitionResult;
    }


    /*
    async Task Listen(CancellationToken cancellationToken)
    {
        var isGranted = await speechToText.RequestPermissions(cancellationToken);
        if (!isGranted)
        {
            await Toast.Make("Permission not granted").Show(CancellationToken.None);
            return;
        }

        var recognitionResult = await speechToText.ListenAsync(CultureInfo.GetCultureInfo("en"),
                                            new Progress<string>(partialText => {
                                                RecognitionText += partialText + " ";
                                            }), cancellationToken);

        if (recognitionResult.IsSuccessful) {
            RecognitionText = recognitionResult.Text;
        } else {
            await Toast.Make(recognitionResult.Exception?.Message ?? "Unable to recognize speech").Show(CancellationToken.None);
        }
    }

    async Task StopListening(CancellationToken cancellationToken)
    {
        await SpeechToText.StopListenAsync(CancellationToken.None);
    }*/

    private void OnBoldClicked(object sender, EventArgs e)
    {
        string sel = SelectText(sender, e);
        if (sel != null) {

        }

    }

    private void OnItalicsClicked(object sender, EventArgs e)
    {

    }

    private void OnFontChanged(object sender, EventArgs e)
    {

    }
    private void OnFontSizeChanged(object sender, EventArgs e)
    {

    }
    
    private void OnTextColorChanged(object sender, EventArgs e)
    {

    }

    //OTHER
    private void OnSearched(object sender, EventArgs e)
    {

    }



}