using Marten;
using SoftwareCatalogService.Outgoing;

namespace HelpDeskStreamAcl.Handlers;

public static class CatalogHandler
{
    public static async Task HandleAsync(SoftwareCatalogItemCreated message, IDocumentSession session, ILogger logger)
    {
        logger.LogInformation("Got a software item {0}", message.Name);
        session.Store(message);
        await session.SaveChangesAsync();

    }
}
