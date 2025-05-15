using System.Text.Json.Serialization;

namespace FlickrApp.Models;

public class FlickrLicense
{
    [JsonPropertyName("id")] public int Id { get; set; }

    [JsonPropertyName("name")] public string? Name { get; set; }

    [JsonPropertyName("url")] public string? Url { get; set; }
}