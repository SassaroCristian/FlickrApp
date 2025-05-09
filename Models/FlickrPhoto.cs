using System.Text.Json.Serialization;

namespace FlickrApp.Models;

public class FlickrPhoto
{
    private const string baseUrl = "https://live.staticflickr.com";
    [JsonPropertyName("id")] public string Id { get; set; }

    [JsonPropertyName("owner")] public string Owner { get; set; }

    [JsonPropertyName("secret")] public string Secret { get; set; }

    [JsonPropertyName("server")] public string Server { get; set; }

    [JsonPropertyName("farm")] public int Farm { get; set; }

    [JsonPropertyName("title")] public string Title { get; set; }

    [JsonPropertyName("ispublic")] public int Ispublic { get; set; }

    [JsonPropertyName("isfriend")] public int Isfriend { get; set; }

    [JsonPropertyName("isfamily")] public int Isfamily { get; set; }

    public string LargeUrl => $"{baseUrl}/{Server}/{Id}_{Secret}_b.jpg";

    private string? _mediumUrl = string.Empty;

    public string? MediumUrl
    {
        get => string.IsNullOrWhiteSpace(_mediumUrl) ? $"{baseUrl}/{Server}/{Id}_{Secret}_m.jpg" : _mediumUrl;
        set => _mediumUrl = value;
    }
    
    public string SmallUrl => $"{baseUrl}/{Server}/{Id}_{Secret}_s.jpg";
}