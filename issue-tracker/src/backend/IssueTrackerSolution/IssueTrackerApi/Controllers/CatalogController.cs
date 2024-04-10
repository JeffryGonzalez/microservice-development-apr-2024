using IssueTrackerApi.Data;
using IssueTrackerApi.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IssueTrackerApi.Controllers;

public class CatalogController(IssuesDataContext context) : ControllerBase
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
        var softwareItem = await context.ActiveCatalogItems.Select(c =>
         new IssueResponseSoftwareInfoModel { Id = c.Id, Name = c.Title }).SingleOrDefaultAsync(c => c.Id == id);

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
            Status = IssueStatus.Pending
        };
        context.Issues.Add(issue);
        await context.SaveChangesAsync();


        // If we do, write the issue to our database.
        // send them back a response
        // send a message to a topic so some other process can handle it.
        var response = new IssueResponseModel
        {
            CreatedAt = issue.CreatedAt,
            Description = request.Description,
            Id = issue.Id,
            Software = softwareItem,
            Status = issue.Status
        };

        return Ok(response);
    }



}
