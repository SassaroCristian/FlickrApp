using SQLite;
using SQLiteNetExtensions.Attributes;

namespace FlickrApp.Entities;

[Table("APHOTODETAIL")]
public class DetailEntity
{
    [PrimaryKey]
    [Column("APDTID")]
    [ForeignKey(typeof(PhotoEntity))]
    public string Id { get; init; }

    [Column("APDTDESCRIPTION")] public string? Description { get; set; }

    [Column("APDTTAGS")] public string? Tags { get; set; }

    [Column("APDTOWNERNSID")] public string? OwnerNsid { get; set; }

    [Column("APDTOWNER")] public string? OwnerUsername { get; set; }

    [Column("APDTFARM")] public int Farm { get; set; }

    [Column("APDTLICENSE")] public string? License { get; set; }

    [Column("APDTDATEUPLOADED")] public string? DateUploaded { get; set; }

    [Column("APDTVIEWS")] public string? Views { get; set; }
        
    [OneToOne("Id", ReadOnly = true)] public PhotoEntity? Photo { get; set; }
}