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
        var results = await context.Catalog.Where(c => c.Retired == false).ProjectToModel().ToListAsync();

        return Ok(new CatalogResponseModel {  Catalog = results});
    }


}
