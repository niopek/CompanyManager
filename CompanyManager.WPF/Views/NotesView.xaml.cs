using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace CompanyManager.WPF.Views
{
    /// <summary>
    /// Logika interakcji dla klasy NotesView.xaml
    /// </summary>
    public partial class NotesView : UserControl
    {
        public NotesView()
        {
            InitializeComponent();

            WeakReferenceMessenger.Default.Register<string, string>(this, "NotesView", (r, message) =>
            {
                SetUpRichTextBoxContent(message);
            });
        }

        private void SetUpRichTextBoxContent(string message)
        {
            var flowDoc = new FlowDocument();
            var range = new TextRange(flowDoc.ContentStart, flowDoc.ContentEnd);
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(message)))
            {
                range.Load(stream, DataFormats.Xaml);
            }

            richTextBoxNote.Document = flowDoc;
        }

        private static CancellationTokenSource? _debounceToken;

        private void RichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is RichTextBox richTextBox)
            {
                _debounceToken?.Cancel();
                _debounceToken = new CancellationTokenSource();
                var token = _debounceToken.Token;

                Task.Delay(500, token).ContinueWith(t =>
                {
                    if (!t.IsCanceled)
                    {
                        richTextBox.Dispatcher.Invoke(() =>
                        {
                            var range = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
                            using (var stream = new MemoryStream())
                            {
                                range.Save(stream, DataFormats.Xaml);
                                stream.Seek(0, SeekOrigin.Begin);

                                using (var reader = new StreamReader(stream))
                                {
                                    var message = reader.ReadToEnd();
                                    WeakReferenceMessenger.Default.Send(message, "NotesViewModel");
                                }
                            }
                        });
                    }
                }, TaskScheduler.Default);
            }
        }

        private void BoldButton_Click(object sender, RoutedEventArgs e)
        {
            // Pobieranie zaznaczenia
            var selection = richTextBoxNote.Selection;

            // Sprawdzanie obecnego stanu pogrubienia
            if (selection.GetPropertyValue(TextElement.FontWeightProperty) is FontWeight weight && weight == FontWeights.Bold)
            {
                selection.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Normal);
            }
            else
            {
                selection.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
            }
        }

        private void ItalicButton_Click(object sender, RoutedEventArgs e)
        {
            var selection = richTextBoxNote.Selection;

            if (selection.GetPropertyValue(TextElement.FontStyleProperty) is FontStyle style && style == FontStyles.Italic)
            {
                selection.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Normal);
            }
            else
            {
                selection.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Italic);
            }
        }

        private void UnderlineButton_Click(object sender, RoutedEventArgs e)
        {
            var selection = richTextBoxNote.Selection;

            var currentDecorations = selection.GetPropertyValue(Inline.TextDecorationsProperty);
            if (currentDecorations == TextDecorations.Underline)
            {
                selection.ApplyPropertyValue(Inline.TextDecorationsProperty, null);
            }
            else
            {
                selection.ApplyPropertyValue(Inline.TextDecorationsProperty, TextDecorations.Underline);
            }
        }

        private void FontSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FontSizeComboBox.SelectedItem is ComboBoxItem selectedItem && double.TryParse(selectedItem.Content.ToString(), out double fontSize))
            {
                richTextBoxNote.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, fontSize);
            }
        }
    }
}
