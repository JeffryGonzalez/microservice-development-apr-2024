using IssueTrackerApi.Data;
using Microsoft.EntityFrameworkCore;
using SoftwareCatalogService.Outgoing;

namespace IssueTrackerApi.Handlers;

public class CatalogHandler(IssuesDataContext context, ILogger<CatalogHandler> logger)
{
    // whenever we get (From Kafka) a message saying a catalog item is created, run this here code.

    public async Task Handle(SoftwareCatalogItemCreated message)
    {
        logger.LogInformation("Got a new piece of software {0}", message.Name);
        // convert this to a catalog item and save it in our database.
        var newItem = new CatalogItem
        {
            Id = Guid.Parse(message.Id),
            Description = message.Description,
            Retired = false,
            Title = message.Name
        };

        context.Catalog.Add(newItem);
        await context.SaveChangesAsync();
    }

    public async Task Handle(SoftwareCatalogItemRetired message)
    {
        var item = await context.Catalog.SingleOrDefaultAsync(i => i.Id == Guid.Parse(message.Id));

        if (item is not null)
        {
            item.Retired = true;
            await context.SaveChangesAsync();
        }
    }
}
