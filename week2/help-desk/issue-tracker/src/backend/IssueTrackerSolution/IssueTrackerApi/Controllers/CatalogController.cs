using IssueTrackerApi.Data;
using IssueTrackerApi.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wolverine;

namespace IssueTrackerApi.Controllers;

public class CatalogController(IssuesDataContext context, IMessageBus bus) : ControllerBase
{
    [HttpGet("/catalog")]
    public async Task<ActionResult<CatalogResponseModel>> GetSupportedSoftware()
    {
        var results = await context.ActiveCatalogItems
            .ProjectToModel().ToListAsync();

        return Ok(new CatalogResponseModel { Catalog = results });
    }

    [HttpPost("/catalog/{id:guid}/issues")]
    public async Task<ActionResult> AddAnIssueAsync([FromBody] IssueRequestModel request, Guid id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        // Todo List.
        // Find if we support that software (look it up.)
        var softwareItem = await context.ActiveCatalogItems.SingleOrDefaultAsync(c => c.Id == id);

        if (softwareItem is null)
        {
            return NotFound("That software item is not supported");
        }

        var issue = new Issue
        {
            Id = Guid.NewGuid(),
            SoftwareId = softwareItem.Id,
            CreatedAt = DateTime.UtcNow,
            Description = request.Description,
            Status = IssueStatus.Pending,
            CatalogItem = softwareItem
        };
        context.Issues.Add(issue);
        await context.SaveChangesAsync();

        // publish a message to the topic.
        await bus.InvokeAsync(new PublishIssueCommand(issue.Id, softwareItem.Id, request.Description, issue.CreatedAt, User?.Identity?.Name ?? "Jeff"));

        var response = new IssueResponseModel
        {
            CreatedAt = issue.CreatedAt,
            Description = request.Description,
            Id = issue.Id,
            Software = new IssueResponseSoftwareInfoModel()
            {
                Id = softwareItem.Id,
                Name = softwareItem.Title
            },
            Status = issue.Status
        };

        return Ok(response);
    }

    [HttpGet("/catalog/{id:guid}/issues")]
    public async Task<ActionResult> GetIssuesForCatalogAsync(Guid id)
    {
        var sw = await context.Catalog.Include(c => c.Issues).Where(c => c.Id == id).SingleOrDefaultAsync();
        if (sw is null)
        {
            return NotFound();
        }
        return Ok(sw.Issues.Select(i => new { Id = i.Id, Description = i.Description, Status = i.Status, Created = i.CreatedAt }));
    }

    [HttpGet("/issues/{id:guid}")]
    public async Task<ActionResult> GetIssueById(Guid id)
    {
        var issue = await context.Issues.Include(i => i.CatalogItem).SingleOrDefaultAsync(i => i.Id == id);

        if (issue is null)
        {
            return NotFound();
        }
        else
        {
            return Ok(new
            {
                IssueId = issue.Id,
                issue.Status,
                issue.CreatedAt,
                issue.CatalogItem.Title
            });
        }
    }
    // TODO: Cancelling Issues
    // Issues can only be cancelled when they are Pending
    // 
}
public record PublishIssueCommand(Guid IssueId, Guid SoftwareId, string Description, DateTimeOffset CreatedAt, string UserId);
