using IssueTrackerApi.Data;
using Riok.Mapperly.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace IssueTrackerApi.Models;

public class Catalog
{
}


public class CatalogResponseItemModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

[Mapper]
public static partial class CatalogItemMapper
{
    public static partial IQueryable<CatalogResponseItemModel> ProjectToModel(this IQueryable<CatalogItem> q);
}

public class CatalogResponseModel
{
    public IList<CatalogResponseItemModel>? Catalog { get; set; }
}


public record IssueRequestModel
{
    [Required, MaxLength(1024)]
    public string Description { get; set; } = string.Empty;

}

public record IssueResponseModel
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public IssueStatus Status { get; set; }
    public IssueResponseSoftwareInfoModel Software { get; set; } = new();

}


public record IssueResponseSoftwareInfoModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
public enum IssueStatus { Pending };
