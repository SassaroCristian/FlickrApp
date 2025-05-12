using System.ComponentModel.DataAnnotations.Schema;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace FlickrApp.Entities;

[SQLite.Table("APHOTO")]
public class PhotoEntity
{
    [PrimaryKey] [SQLite.Column("APHOID")] public string Id { get; set; }

    [SQLite.Column("APHOTITLE")] public string? Title { get; set; }

    [SQLite.Column("APHOSECRET")] public string? Secret { get; set; }

    [SQLite.Column("APHOSERVER")] public string? Server { get; set; }

    [SQLite.Column("APHOLOCALFILEPATH")] public string? LocalFilePath { get; set; }

    [NotMapped] private const string baseUrl = "https://live.staticflickr.com";

    [Ignore] public string LargeUrl => $"{baseUrl}/{Server}/{Id}_{Secret}_b.jpg";
    
    [Ignore] public string MediumUrl => $"{baseUrl}/{Server}/{Id}_{Secret}_m.jpg";

    [Ignore] public string SmallUrl => $"{baseUrl}/{Server}/{Id}_{Secret}_s.jpg";
    
    [Ignore] public bool IsSavedLocally => File.Exists(LocalFilePath);

    [Ignore]
    public string DisplayLargeSource =>
        (IsSavedLocally ? LocalFilePath : MediumUrl) ?? throw new InvalidOperationException();

    [Ignore]
    public string DisplayMediumSource =>
        (IsSavedLocally ? LocalFilePath : MediumUrl) ?? throw new InvalidOperationException();

    [Ignore]
    public string DisplaySmallSource =>
        (IsSavedLocally ? LocalFilePath : MediumUrl) ?? throw new InvalidOperationException();
    
    [OneToOne("Id", CascadeOperations = CascadeOperation.All)]
    public DetailEntity? Detail { get; set; }
}