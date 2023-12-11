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
using System.Text;

namespace ravendesk;

public partial class TextEditor : ContentPage
{
    private readonly IFileSaver fileSaver;
    //private readonly IFolderPicker folderPicker;
    private TextEditor editor;
    CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

    public TextEditor(IFileSaver fileSaver /*, IFolderPicker folderPicker*/)
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

    //HELPER FUNCTIONS
    public string SelectText(object sender, EventArgs e)
    {
        string sel = "";
        int start = textEditor.CursorPosition;
        int length = textEditor.SelectionLength;
        sel = textEditor.Text.Substring(start, length);
        return sel;
    }


    public async void OnSaveClicked(object sender, EventArgs args)
    {
        await SaveFile(cancellationTokenSource.Token);
    }
    async Task SaveFile(CancellationToken cancellationToken)
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
            //textEditor.Text = fileSaverResult.FilePath.ToString();
            //await Toast.Make($"The file was saved successfully to location: {fileSaverResult.FilePath}").Show(cancellationToken);
        }
        
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