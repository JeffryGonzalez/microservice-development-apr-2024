using Marten;
using SoftwareCatalogService.Outgoing;

namespace HelpDeskStreamAcl.Handlers;

public static class SoftwareCatalogHandler
{
    public static async Task HandleAsync(SoftwareCatalogItemCreated message, IDocumentSession session)
    {
        session.Store(message);
        await session.SaveChangesAsync();
    }
}
