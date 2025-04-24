using System.Text.Json.Serialization;

namespace FlickrApp.Models;

public class FlickrApiResponses
{
    public class GetRecent
    {
        [JsonPropertyName("photos")] public FlickrPhotos Photos { get; set; }

        [JsonPropertyName("stat")] public string Stat { get; set; }
    }

    public class Search
    {
        [JsonPropertyName("photos")] public FlickrPhotos Photos { get; set; }

        [JsonPropertyName("stat")] public string Stat { get; set; }
    }

    public class GetDetails
    {
        [JsonPropertyName("photo")] public FlickrDetails? Details { get; set; }

        [JsonPropertyName("stat")] public string Stat { get; set; }
    }

    public class GetComments
    {
        [JsonPropertyName("comments")] public FlickrComments? Comments { get; set; }

        [JsonPropertyName("stat")] public string Stat { get; set; }
    }
}

public class FlickrPhotos
{
    [JsonPropertyName("page")] public int Page { get; set; }

    [JsonPropertyName("pages")] public int Pages { get; set; }

    [JsonPropertyName("perpage")] public int Perpage { get; set; }

    [JsonPropertyName("total")] public int Total { get; set; }

    [JsonPropertyName("photo")] public List<FlickrPhoto> List { get; set; }
}

public class FlickrComments
{
    [JsonPropertyName("photo_id")] public string PhotoId { get; set; }

    [JsonPropertyName("comment")] public List<FlickrComment>? List { get; set; }
}