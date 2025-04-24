using System.Text.Json.Serialization;

namespace FlickrApp.Models;

public class FlickrComment
{
    [JsonPropertyName("id")] public string Id { get; set; }

    [JsonPropertyName("author")] public string Author { get; set; }

    [JsonPropertyName("author_is_deleted")]
    public int AuthorIsDeleted { get; set; }

    [JsonPropertyName("authorname")] public string Authorname { get; set; }

    [JsonPropertyName("iconserver")] public string Iconserver { get; set; }

    [JsonPropertyName("iconfarm")] public int Iconfarm { get; set; }

    [JsonPropertyName("datecreate")] public string Datecreate { get; set; }

    [JsonPropertyName("permalink")] public string Permalink { get; set; }

    [JsonPropertyName("path_alias")] public string PathAlias { get; set; }

    [JsonPropertyName("realname")] public string Realname { get; set; }

    [JsonPropertyName("_content")] public string Content { get; set; }
}