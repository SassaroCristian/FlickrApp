using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FlickrApp.Models
{
    public class FlickrDetails
    {
        [JsonPropertyName("id")] public string? Id { get; set; }

        [JsonPropertyName("secret")] public string? Secret { get; set; }

        [JsonPropertyName("server")] public string? Server { get; set; }

        [JsonPropertyName("farm")] public int Farm { get; set; }

        [JsonPropertyName("dateuploaded")] public string? DateUploaded { get; set; }

        [JsonPropertyName("isfavorite")] public int? IsFavorite { get; set; }

        [JsonPropertyName("license")] public string? License { get; set; }

        [JsonPropertyName("safety_level")] public string? SafetyLevel { get; set; }

        [JsonPropertyName("rotation")] public int Rotation { get; set; }

        [JsonPropertyName("owner")] public Owner? Owner { get; set; }

        [JsonPropertyName("title")] public Title? Title { get; set; }

        [JsonPropertyName("description")] public Description? Description { get; set; }

        [JsonPropertyName("visibility")] public Visibility? Visibility { get; set; }

        [JsonPropertyName("dates")] public Dates? Dates { get; set; }

        [JsonPropertyName("views")] public string? Views { get; set; }

        [JsonPropertyName("editability")] public Editability? Editability { get; set; }

        [JsonPropertyName("publiceditability")]
        public PublicEditability? PublicEditability { get; set; }

        [JsonPropertyName("usage")] public Usage? Usage { get; set; }

        [JsonPropertyName("comments")] public Comments? Comments { get; set; }

        [JsonPropertyName("notes")] public Notes? Notes { get; set; }

        [JsonPropertyName("people")] public People? People { get; set; }

        [JsonPropertyName("tags")] public Tags? Tags { get; set; }

        [JsonPropertyName("location")] public Location? Location { get; set; }

        [JsonPropertyName("geoperms")] public Geoperms? Geoperms { get; set; }

        [JsonPropertyName("urls")] public Urls? Urls { get; set; }

        [JsonPropertyName("media")] public string? Media { get; set; }
    }

    public class Comments
    {
        [JsonPropertyName("_content")] public string? Content { get; set; }
    }

    public class Country
    {
        [JsonPropertyName("_content")] public string? Content { get; set; }

        [JsonPropertyName("woeid")] public string? Woeid { get; set; }
    }

    public class County
    {
        [JsonPropertyName("_content")] public string? Content { get; set; }

        [JsonPropertyName("woeid")] public string? Woeid { get; set; }
    }

    public class Dates
    {
        [JsonPropertyName("posted")] public string? Posted { get; set; }

        [JsonPropertyName("taken")] public string? Taken { get; set; }

        [JsonPropertyName("takengranularity")] public int Takengranularity { get; set; }

        [JsonPropertyName("takenunknown")] public string? Takenunknown { get; set; }

        [JsonPropertyName("lastupdate")] public string? Lastupdate { get; set; }
    }

    public class Description
    {
        [JsonPropertyName("_content")] public string? Content { get; set; }
    }

    public class Editability
    {
        [JsonPropertyName("cancomment")] public int? CanComment { get; set; }

        [JsonPropertyName("canaddmeta")] public int? CanAddMeta { get; set; }
    }

    public class Geoperms
    {
        [JsonPropertyName("ispublic")] public int? IsPublic { get; set; }

        [JsonPropertyName("iscontact")] public int? IsContact { get; set; }

        [JsonPropertyName("isfriend")] public int? IsFriend { get; set; }

        [JsonPropertyName("isfamily")] public int? IsFamily { get; set; }
    }

    public class Gift
    {
        [JsonPropertyName("gift_eligible")] public bool GiftEligible { get; set; }

        [JsonPropertyName("eligible_durations")]
        public List<object>? EligibleDurations { get; set; }

        [JsonPropertyName("new_flow")] public bool NewFlow { get; set; }
    }

    public class Locality
    {
        [JsonPropertyName("_content")] public string? Content { get; set; }

        [JsonPropertyName("woeid")] public string? Woeid { get; set; }
    }

    public class Location
    {
        [JsonPropertyName("latitude")] public string? Latitude { get; set; }

        [JsonPropertyName("longitude")] public string? Longitude { get; set; }

        [JsonPropertyName("accuracy")] public string? Accuracy { get; set; }

        [JsonPropertyName("context")] public string? Context { get; set; }

        [JsonPropertyName("locality")] public Locality? Locality { get; set; }

        [JsonPropertyName("county")] public County? County { get; set; }

        [JsonPropertyName("region")] public Region? Region { get; set; }

        [JsonPropertyName("country")] public Country? Country { get; set; }

        [JsonPropertyName("neighbourhood")] public Neighbourhood? Neighbourhood { get; set; }
    }

    public class Neighbourhood
    {
        [JsonPropertyName("_content")] public string? Content { get; set; }

        [JsonPropertyName("woeid")] public string? Woeid { get; set; }
    }

    public class Notes
    {
        [JsonPropertyName("note")] public List<object>? Note { get; set; }
    }

    public class Owner
    {
        [JsonPropertyName("nsid")] public string? Nsid { get; set; }

        [JsonPropertyName("username")] public string? Username { get; set; }

        [JsonPropertyName("realname")] public string? Realname { get; set; }

        [JsonPropertyName("location")] public string? Location { get; set; }

        [JsonPropertyName("iconserver")] public string? Iconserver { get; set; }

        [JsonPropertyName("iconfarm")] public int Iconfarm { get; set; }

        [JsonPropertyName("path_alias")] public string? PathAlias { get; set; }

        [JsonPropertyName("gift")] public Gift? Gift { get; set; }
    }

    public class People
    {
        [JsonPropertyName("haspeople")] public int? HasPeople { get; set; }
    }

    public class PublicEditability
    {
        [JsonPropertyName("cancomment")] public int? CanComment { get; set; }

        [JsonPropertyName("canaddmeta")] public int? CanAddMeta { get; set; }
    }

    public class Region
    {
        [JsonPropertyName("_content")] public string? Content { get; set; }

        [JsonPropertyName("woeid")] public string? Woeid { get; set; }
    }

    public class Tag
    {
        [JsonPropertyName("id")] public string? Id { get; set; }

        [JsonPropertyName("author")] public string? Author { get; set; }

        [JsonPropertyName("authorname")] public string? Authorname { get; set; }

        [JsonPropertyName("raw")] public string? Raw { get; set; }

        [JsonPropertyName("_content")] public string? Content { get; set; }

        [JsonPropertyName("machine_tag")] public int? MachineTag { get; set; }
    }

    public class Tags
    {
        [JsonPropertyName("tag")] public List<Tag>? Tag { get; set; }
    }

    public class Title
    {
        [JsonPropertyName("_content")] public string? Content { get; set; }
    }

    public class Url
    {
        [JsonPropertyName("type")] public string? Type { get; set; }

        [JsonPropertyName("_content")] public string? Content { get; set; }
    }

    public class Urls
    {
        [JsonPropertyName("url")] public List<Url>? Url { get; set; }
    }

    public class Usage
    {
        [JsonPropertyName("candownload")] public int? CanDownload { get; set; }

        [JsonPropertyName("canblog")] public int? CanBlog { get; set; }

        [JsonPropertyName("canprint")] public int? CanPrint { get; set; }

        [JsonPropertyName("canshare")] public int? CanShare { get; set; }
    }

    public class Visibility
    {
        [JsonPropertyName("ispublic")] public int? IsPublic { get; set; }

        [JsonPropertyName("isfriend")] public int? IsFriend { get; set; }

        [JsonPropertyName("isfamily")] public int? IsFamily { get; set; }
    }
}