using SQLite;

namespace FlickrApp.Entities;

[Table("APhotos")]
public class Photo
{
    [PrimaryKey, Column("id")] public string Id { get; set; }

    [Column("owner")] public string Owner { get; set; }

    [Column("secret")] public string Secret { get; set; }
}