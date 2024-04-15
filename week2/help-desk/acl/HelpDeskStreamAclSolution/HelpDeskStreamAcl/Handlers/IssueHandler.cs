using HelpDeskStreamAcl.Outgoing;
using IssueTrackerApi.Outgoing;
using Marten;
using SoftwareCatalogService.Outgoing;
using Wolverine;

namespace HelpDeskStreamAcl.Handlers;

public static class IssueHandler
{
    public static async Task Handle(IssueCreated message, IDocumentSession session, IMessageBus bus)
    {
        session.Store(message);
        var sw = await session.LoadAsync<SoftwareCatalogItemCreated>(message.SoftwareId);
        if (sw is null)
        {
            throw new RaceConditionException();
        }
        var doc = new IssueDocument
        {
            Status = Status.Pending,
            Id = message.Id,
            Description = message.Description,
            CreatedAt = message.CreatedAt,
            Version = 1,
            Software = new Software
            {
                Id = sw.Id,
                Title = sw.Name
            },
            User = new User
            {
                Id = "x0042069",
                Name = "Jeff Gonzalez",
                Phone = "555-1212",
                Email = "jeff@aol.com"
            }

        };
        await bus.PublishAsync(doc);
        await session.SaveChangesAsync();

    }
}
