using SQLite;

namespace FlickrApp.Entities;

[Table("Photos")]
public class Photo
{
    [PrimaryKey] [Column("id")] public string Id { get; set; }

    [Column("title")] public string? Title { get; set; }

    [Column("description")] public string? Description { get; set; }

    [Column("owner_nsid")] public string? OwnerNsid { get; set; }

    [Column("owner")] public string? OwnerUsername { get; set; }

    [Column("secret")] public string Secret { get; set; }

    [Column("server")] public string Server { get; set; }

    [Column("farm")] public int Farm { get; set; }

    [Column("date_uploaded")] public string? DateUploaded { get; set; }

    [Column("views")] public string? Views { get; set; }

    [Column("local_filepath")] public string? LocalFilePath { get; set; }
}