using IssueTrackerApi.Controllers;
using IssueTrackerApi.Data;
using Wolverine;

namespace IssueTrackerApi.Handlers;

public class IssuesHandler(IMessageBus bus, IssuesDataContext context)
{
    public async Task Handle(PublishIssueCommand command)
    {


    }
}


