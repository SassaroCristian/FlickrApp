using System.Globalization;

namespace FlickrApp.Converters;

public class NullOrEmptyToNaConverter : IValueConverter
{
    private const string NotAvailableText = "N/A";

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null) return NotAvailableText;

        if (value is string str) return string.IsNullOrWhiteSpace(str) ? NotAvailableText : str;

        return value;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}