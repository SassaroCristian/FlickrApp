using System.Globalization;
using FlickrApp.Models;

namespace FlickrApp.Converters;

public class TagListToStringConverter : IValueConverter
{
    private const string NotAvailableText = "N/A";
    private const string Separator = ", ";

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is List<Tag> tagList && tagList.Any())
        {
            var tagStrings = tagList
                .Select(tag => !string.IsNullOrWhiteSpace(tag.Raw) ? tag.Raw : tag.Content)
                .Where(tagContent => !string.IsNullOrWhiteSpace(tagContent));

            if (tagStrings.Any()) return string.Join(Separator, tagStrings);
        }

        return NotAvailableText;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}