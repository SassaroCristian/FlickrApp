namespace FlickrApp.Models.Lookups;

public static class SortOptions
{
    public static SortCriterion InterestingnessDesc => new("interestingness-desc", "Most Interesting");
    public static SortCriterion InterestingnessAsc => new("interestingness-asc", "Least Interesting");
    public static SortCriterion DatePostedDesc => new("date-posted-desc", "Newest First");
    public static SortCriterion DatePostedAsc => new("date-posted-asc", "Oldest First");
    public static SortCriterion Relevance => new("relevance", "Relevance");

    public static List<SortCriterion> All =>
    [
        InterestingnessDesc,
        InterestingnessAsc,
        DatePostedDesc,
        DatePostedAsc,
        Relevance
    ];
}