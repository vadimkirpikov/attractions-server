using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TouristServer.DAL;
using TouristServer.DAL.Dbo;

namespace TouristServer.Presentation.Controllers;

[Authorize]
[ApiController]
[Route("/v1")]
public class OthersController(ApplicationDbContext context) : ControllerBase
{
    private async Task<IEnumerable<T>> GetSubjects<T>(int? page, int? pageSize) where T : class
    {
        var query = context.Set<T>().AsQueryable();

        if (page != null && pageSize != null)
        {
            query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
        }
        return await query.ToListAsync();
    }

    [HttpGet("districts")]
    public async Task<ActionResult<IEnumerable<District>>> GetDistricts([FromQuery] int? page, [FromQuery] int? pageSize)
    {
        return Ok(await GetSubjects<District>(page, pageSize));
    }
    
    [HttpGet("categories")]
    public async Task<ActionResult<IEnumerable<Category>>> GetCategories([FromQuery] int? page, [FromQuery] int? pageSize)
    {
        return Ok(await GetSubjects<Category>(page, pageSize));
    }

    [HttpGet("users")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers([FromQuery] int? page, [FromQuery] int? pageSize)
    {
        return Ok(await GetSubjects<User>(page, pageSize));
    }
}