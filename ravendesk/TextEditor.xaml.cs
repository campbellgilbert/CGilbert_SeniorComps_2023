using CommunityToolkit.Maui.Storage;
using System.Text;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech;

namespace ravendesk;

public partial class TextEditor : ContentPage
{
    private TextEditor editor;

    //file & save variables
    private readonly IFileSaver fileSaver;
    CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

    //STT
    static string speechKey = Environment.GetEnvironmentVariable("SPEECH_KEY");
    static string speechRegion = Environment.GetEnvironmentVariable("SPEECH_REGION");

    /*
     * LOAD/BASE METHODS
     * Instantiates and loads base word processor.
     */

    public TextEditor(IFileSaver fileSaver)
    {
        this.fileSaver = fileSaver;
        //this.folderPicker = folderPicker;
        InitializeComponent();

        this.Loaded += TextEditor_Loaded;
    }
    private async void TextEditor_Loaded(object sender, EventArgs e)
    {
        await Clipboard.Default.SetTextAsync(null);
    }

    /*
     * HELPERS
     * Called by buttons/flyouts/etc/
     */
    public string SelectText(object sender, EventArgs e)
    {
        string sel = "";
        int start = textEditor.CursorPosition;
        int length = textEditor.SelectionLength;
        sel = textEditor.Text.Substring(start, length);
        return sel;
    }
    
    private async Task SaveFile(CancellationToken cancellationToken)
    {
        if (textEditor.Text == null)
        {
            await DisplayAlert("No text", "Please enter some text", "OK");
            return;
        }
        using var stream = new MemoryStream(Encoding.Default.GetBytes(textEditor.Text));
        var fileSaverResult = await FileSaver.Default.SaveAsync("NewFile.txt", stream, cancellationToken);
        if (!fileSaverResult.IsSuccessful)
        {
            await DisplayAlert("Save Unsuccessful", "Something went wrong.", "OK");
            return;
        }
    }

    private async void GetSpeechRecognitionResult(SpeechRecognitionResult speechRecognitionResult)
    {
        switch (speechRecognitionResult.Reason)
        {
            case ResultReason.RecognizedSpeech:
                textEditor.Text += speechRecognitionResult.Text.ToString();
                break;
            case ResultReason.NoMatch:
                await DisplayAlert("Error: NOMATCH", "Speech unrecognizable.", "OK");
                break;
            case ResultReason.Canceled:
                var cancellation = CancellationDetails.FromResult(speechRecognitionResult);
                await DisplayAlert("CANCELED", cancellation.Reason.ToString(), "OK");

                if (cancellation.Reason == CancellationReason.Error)
                {
                    await DisplayAlert("CANCELED: Error", "Error Code:" + cancellation.ErrorCode
                        + "\n Error Details: " + cancellation.ErrorDetails +
                        "\n Check your speech resource key and region values.", "OK");
                }
                break;
        }
    }

    public async Task<FileResult> PickFile(PickOptions options)
    {
        try
        {
            var result = await FilePicker.Default.PickAsync(options);
            if (result != null)
            {
                if (result.FileName.EndsWith("txt", StringComparison.OrdinalIgnoreCase))
                {
                    var filepath = result.FullPath;
                    //using var stream = await result.OpenReadAsync();
                }
            }
            return result;
        }
        catch (Exception ex)
        {
            await DisplayAlert("File Selection Error", "Something went wrong.", "OK");
        }

        return null;
    }

    public async Task PullUpFile(string filepath)
    {
        //if file already open, give option to save
        if (textEditor.Text != null)
        {
            //save current file
            await SaveFile(cancellationTokenSource.Token);
            //then clear out the editor
            textEditor.Text = null;
        }

        if (!File.Exists(filepath))
        {
            await DisplayAlert("Error: No Such File", "Something went wrong.", "OK");
        }
        using (StreamReader sr = File.OpenText(filepath))
        {
            textEditor.Text = sr.ReadToEnd();
        }
    }

    /*
     * BUTTON/TOOLBAR ONCLICKEDS
     */
    public async void OnSaveClicked(object sender, EventArgs args)
    {
        await SaveFile(cancellationTokenSource.Token);
    }
   
    public async void OnFileSelectClicked(object sender, EventArgs args)
    {
        var supportedFiletypes = new FilePickerFileType(
                new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.WinUI, new[] { ".txt", ".rtf" } }, // file extension
                    { DevicePlatform.macOS, new[] { "txt" } }, // UTType values
                });

        PickOptions options = new()
        {
            PickerTitle = "Please select a .txt file.",
            FileTypes = supportedFiletypes,
        };
        FileResult filePicked = await PickFile(options);
        await PullUpFile(filePicked.FullPath);
    }

    private void OnFontChanged(object sender, EventArgs e)
    {
        if (FontSizePicker.SelectedIndex > 0)
        {
            this.textEditor.FontFamily = FontPicker.SelectedItem.ToString();
        }
        else
        {
            textEditor.FontFamily = "Open Sans";
        }
    }
    
    private void OnFontSizeChanged(object sender, EventArgs e)
    {
        if (FontSizePicker.SelectedIndex > 0)
        {
            this.textEditor.FontSize = double.Parse(FontSizePicker.SelectedItem.ToString());
        } else
        {
            textEditor.FontSize = 14;
        }
    }

    private async void OnSTTClicked(object sender, EventArgs e)
    {
        try
        {
            var speechConfig = SpeechConfig.FromSubscription(speechKey, speechRegion);
            speechConfig.SpeechRecognitionLanguage = "en-US";

            using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
            using var speechRecognizer = new SpeechRecognizer(speechConfig, audioConfig);

            Talkstart.FontAttributes = FontAttributes.Italic;
            Talkstart.Text = "Speak into your microphone...";

            var speechRecognitionResult = await speechRecognizer.RecognizeOnceAsync();
            GetSpeechRecognitionResult(speechRecognitionResult);
        } 
        catch
        {
            await DisplayAlert("Error: Speech to Text Failure", "Something went wrong.", "OK");
        }
        Talkstart.Text = "Speech To Text";
        Talkstart.FontAttributes = FontAttributes.None;
    }


    /*
     * FLYOUT MENU ONCLICKEDS
     */
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
        var newWindow = new Window(new FeedbackPage());
        Application.Current.OpenWindow(newWindow);
    }

    private void OnPartialFeedbackClicked(object sender, EventArgs e)
    {
        OnCopyClicked(sender, e);
        var newWindow = new Window(new FeedbackPage());
        Application.Current.OpenWindow(newWindow);
    }

}