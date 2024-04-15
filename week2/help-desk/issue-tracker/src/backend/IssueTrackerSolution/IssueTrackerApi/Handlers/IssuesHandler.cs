using Google.Protobuf.WellKnownTypes;
using IssueTrackerApi.Controllers;
using IssueTrackerApi.Outgoing;
using Wolverine;
namespace IssueTrackerApi.Handlers;

public class IssuesHandler(IMessageBus bus)
{
    public async Task Handle(PublishIssueCommand command)
    {

        var @event = new IssueCreated
        {
            CreatedAt = Timestamp.FromDateTimeOffset(command.CreatedAt),
            Description = command.Description,
            Id = command.IssueId.ToString(),
            SoftwareId = command.SoftwareId.ToString(),
            UserId = command.UserId

        };

        await bus.SendAsync(@event);
    }
}


