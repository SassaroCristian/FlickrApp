namespace FlickrApp.Models.Lookups;

public record SearchNavigationParams
{
    public string? SearchText { get; init; }
    public string? SearchTags { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public string? LicenseId { get; init; }
    public string? ContentType { get; init; }
    public string? GeoContext { get; init; }
    public string? SortCriterionValue { get; init; }
}