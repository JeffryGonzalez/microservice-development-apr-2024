using HelpDeskStreamAcl.Outgoing;
using IssueTrackerApi.Outgoing;
using Marten;
using SoftwareCatalogService.Outgoing;
using Wolverine;

namespace HelpDeskStreamAcl.Handlers;

public static class IssuesHandler
{
    public static async Task<Issue> HandleAsync(IssueCreated message, IDocumentSession session, IMessageBus bus)
    {
        var sw = await session.Query<SoftwareCatalogItemCreated>().SingleAsync(i => i.Id == message.SoftwareId);

       var issue = new Issue
        {
            CreatedAt = message.CreatedAt,
            Description = message.Description,
            Id = message.Id,
            Software = new SoftwareItem
            {
                Id = message.SoftwareId,
                Title = sw.Name,
                Description = sw.Description
            }
        };

        //await bus.PublishAsync(issue);
        return issue;
    }
}
