using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;

namespace CompanyManager.WPF.Converters;

[ValueConversion(typeof(string), typeof(FlowDocument))]
public class DocumentToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is FlowDocument flowDoc)
        {
            var range = new TextRange(flowDoc.ContentStart, flowDoc.ContentEnd);

            using (var stream = new MemoryStream())
            {
                range.Save(stream, DataFormats.Xaml);
                stream.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        return string.Empty;
    }

    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        
        if (value is string text)
        {
            var flowDoc = new FlowDocument();
            var range = new TextRange(flowDoc.ContentStart, flowDoc.ContentEnd);
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            {
                range.Load(stream, DataFormats.Xaml);
            }

            return flowDoc;
        }

        return null;
    }
}
