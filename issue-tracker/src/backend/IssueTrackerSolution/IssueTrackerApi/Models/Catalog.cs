using IssueTrackerApi.Data;
using Riok.Mapperly.Abstractions;

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

