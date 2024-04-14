namespace IssueTrackerApi.Data;

public class CatalogItem
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; }

    public bool Retired { get; set; } = false;
    public ICollection<Issue> Issues { get; set; } = [];
}
