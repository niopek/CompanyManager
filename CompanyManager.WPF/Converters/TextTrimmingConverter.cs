using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CompanyManager.WPF.Converters;

public class TextTrimmingConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string inputText && parameter is string paramString && int.TryParse(paramString, out int maxLength))
        {
            // Usuń znaki takie jak tab, enter, itp. (spacje zostają)
            string cleanedText = Regex.Replace(inputText, @"[\t\r\n]", string.Empty);

            // Skróć tekst, jeśli jest dłuższy niż maxLength
            if (cleanedText.Length > maxLength)
            {
                return cleanedText.Substring(0, maxLength) + "...";
            }

            return cleanedText;
        }

        return value; // Zwróć oryginalny tekst, jeśli nie pasuje do warunków
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException("TextTrimmingConverter supports only one-way conversion.");
    }
}