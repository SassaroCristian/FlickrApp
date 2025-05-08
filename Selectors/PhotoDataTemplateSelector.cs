using FlickrApp.Entities;
using FlickrApp.Models;

namespace FlickrApp.Selectors;

public class PhotoDataTemplateSelector : DataTemplateSelector
{
    public DataTemplate? PhotoTemplate { get; set; }
    public DataTemplate? FlickrPhotoTemplate { get; set; }

    protected override DataTemplate? OnSelectTemplate(object item, BindableObject container)
    {
        return item switch
        {
            Photo photo => PhotoTemplate,
            FlickrPhoto flickrPhoto => FlickrPhotoTemplate,
            _ => null
        };
    }
}