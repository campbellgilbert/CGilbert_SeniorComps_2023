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

    /*  private void OnSaveClicked(object sender, EventArgs e)
      {

          string text = textEditor.Text;
          File.WriteAllText("savedDocument.txt", text);
      }

      async Task SaveFile(CancellationToken cancellationToken)
      {
          using var stream = new MemoryStream(Encoding.Default.GetBytes("Hello from the Community Toolkit!"));
          var fileSaverResult = await FileSaver.Default.SaveAsync("test.txt", stream, cancellationToken);
          if (fileSaverResult.IsSuccessful)
          {
              await Toast.Make($"The file was saved successfully to location: {fileSaverResult.FilePath}").Show(cancellationToken);
          }
          else
          {
              await Toast.Make($"The file was not saved successfully with error: {fileSaverResult.Exception.Message}").Show(cancellationToken);
          }
      }

   /*   private async void OnFileSelectButtonClicked(object sender, EventArgs e)
      {
          var folderPickerResult = await folderPicker.PickAsync(cancellationToken);
          if (folderPickerResult.IsSuccessful)
          {
              await Toast.Make($"Folder picked: Name - {folderPickerResult.Folder.Name}, Path - {folderPickerResult.Folder.Path}", ToastDuration.Long).Show(cancellationToken);
          }
          else
          {
              await Toast.Make($"Folder is not picked, {folderPickerResult.Exception.Message}").Show(cancellationToken);
          }
      }
   */

    private void OnCopilotClicked(object sender, EventArgs e)
    {
        //FIXME -- this just copies the whole fucking thing bruh there's gotta be a better way
        OnCopyClicked(sender, e);
        //FIXME -- paste highlighted text to new copilot
        var newWindow = new Window(new CopilotDEMOPage());
        Application.Current.OpenWindow(newWindow);
    }
}