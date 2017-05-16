using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;

namespace ReproUnder
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _rtb.Focus();
        }

        private void OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            // italic
            object temp = _rtb.Selection.GetPropertyValue(TextElement.FontStyleProperty);
            _tbbItalic.IsChecked =
                temp != DependencyProperty.UnsetValue &&
                temp != null &&
                temp.Equals(FontStyles.Italic);

            // bold
            temp = _rtb.Selection.GetPropertyValue(TextElement.FontWeightProperty);
            _tbbBold.IsChecked = temp != DependencyProperty.UnsetValue &&
                                 temp != null &&
                                 temp.Equals(FontWeights.Bold);

            // underline
            temp = _rtb.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            _tbbUnderline.IsChecked = temp != DependencyProperty.UnsetValue &&
                                      temp != null &&
                                      temp.Equals(TextDecorations.Underline);
        }

        private void OnGetXamlClick(object sender, RoutedEventArgs e)
        {
            _txtXaml.Text = XamlWriter.Save(_rtb.Document);
            _tabText.SelectedItem = _tbiXaml;
        }

        private static void AddDocument(FlowDocument source, FlowDocument target)
        {
            TextRange rngSource = new TextRange(source.ContentStart, source.ContentEnd);
            MemoryStream stream = new MemoryStream();
            XamlWriter.Save(rngSource, stream);
            rngSource.Save(stream, DataFormats.XamlPackage);

            TextRange rngTarget = new TextRange(target.ContentEnd, target.ContentEnd);
            rngTarget.Load(stream, DataFormats.XamlPackage);
        }

        private void OnLoadFaultyXamlClick(object sender, RoutedEventArgs e)
        {
            _txtXaml.Text = "<FlowDocument PagePadding=\"5,0,5,0\" " +
                            "xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">" +
                            "<Paragraph><Span Foreground=\"#FFFF0000\">hello</Span> " +
                            "<Span><Span.TextDecorations><TextDecoration Location=\"Underline\" /></Span.TextDecorations>under</Span></Paragraph></FlowDocument>";
            _tabText.SelectedItem = _tbiXaml;
        }

        private void OnSetXamlClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _rtb.Document.Blocks.Clear();
                FlowDocument doc = (FlowDocument)XamlReader.Parse(_txtXaml.Text);
                AddDocument(doc, _rtb.Document);
                _tabText.SelectedItem = _tbiRtf;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                MessageBox.Show(ex.Message);
            }
        }
    }
}
