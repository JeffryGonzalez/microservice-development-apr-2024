using IssueTrackerApi.Data;
using Microsoft.EntityFrameworkCore;
using SoftwareCatalogService.Outgoing;

namespace IssueTrackerApi.Handlers;

public class CatalogHandler(IssuesDataContext context, ILogger<CatalogHandler> logger)
{
    // whenever we get (From Kafka) a message saying a catalog item is created, run this here code.

    public async Task Handle(SoftwareCatalogItemCreated message)
    {
        // TODO: Upsert?
        logger.LogInformation("Got a new piece of software {0}", message.Name); // TODO: This was probably dumb.
        // convert this to a catalog item and save it in our database.
        // if that thing doesn't exist, add it, otherwise update it

        var storedItem = await context
            .ActiveCatalogItems
            .SingleOrDefaultAsync(i =>
            i.Id == Guid.Parse(message.Id));
        if (storedItem is null)
        {
            var newItem = new CatalogItem
            {
                Id = Guid.Parse(message.Id),
                Description = message.Description,
                Retired = false,
                Title = message.Name
            };

            context.Catalog.Add(newItem);
        }
        else
        {
            storedItem.Title = message.Name;
            storedItem.Description = message.Description;

        }


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
        else
        {
            throw new RaceConditionException();
        }
    }
}
public class RaceConditionException : ArgumentOutOfRangeException { }