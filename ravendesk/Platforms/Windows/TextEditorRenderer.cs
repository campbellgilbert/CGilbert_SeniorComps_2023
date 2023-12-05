using Microsoft.Maui.Controls.Compatibility.Platform.UWP;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Platform;
using Microsoft.UI.Xaml.Controls;
using ravendesk;
using ravendesk.Platforms.Windows;

[assembly: ExportRenderer(typeof(TextEditor), typeof(TextEditorRenderer))]
namespace ravendesk.Platforms.Windows
{
    public class TextEditorRenderer : EditorRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                // Subscribe to the SelectionChanged event of the native TextBox
                Control.SelectionChanged += OnSelectionChanged;
            }
        }

        private void OnSelectionChanged(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                string selectedText = textBox.SelectedText;
                // Here, you can handle the selected text as needed
                // For example, invoke a method on the shared CustomEditor
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && Control != null)
            {
                // Unsubscribe from the SelectionChanged event
                Control.SelectionChanged -= OnSelectionChanged;
            }

            base.Dispose(disposing);
        }
    }
}
