using IssueTrackerApi.Models;

namespace IssueTrackerApi.Data;

public class Issue
{
    public Guid Id { get; set; }

    public Guid SoftwareId { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public IssueStatus Status { get; set; }
}
