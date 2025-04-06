using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TouristServer.DAL;
using TouristServer.Presentation.Dto;

namespace TouristServer.Presentation.Controllers;

[ApiController]
[Authorize]
[Route("v1/places")]
public class PlacesController(ApplicationDbContext context) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> GetFilteredPlaces([FromBody] FilterDto? filter, [FromQuery] int? page, [FromQuery] int? pageSize)
    {
        
        var query = context.Places.AsQueryable();
        if (filter == null) return Ok(await query.ToListAsync());
        if (filter.CategoryIds != null)
        {
            query = query.Where(p => filter.CategoryIds.Contains(p.CategoryId));
        }

        if (filter.DistrictIds != null)
        {
            query = query.Where(p => filter.DistrictIds.Contains(p.Id));
        }

        if (filter.ConstMin != null)
        {
            query = query.Where(p => p.Cost >= filter.ConstMin);
        }

        if (filter.ConstMax != null)
        {
            query = query.Where(p => p.Cost <= filter.ConstMax);
        }

        if (page != null && pageSize != null)
        {
            query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
        }
        
        return Ok(await query.ToListAsync());
    }
    
    [HttpGet("photosOfPlace/{placeId:guid}")]
    public async Task<IActionResult> GetPhotosOfPlace(
        [FromRoute] Guid placeId,
        [FromQuery] int? page,
        [FromQuery] int? pageSize)
    {
        var query = context.Photos.Where(p => p.PlaceId == placeId).AsQueryable();
        if (page != null && pageSize != null)
        {
            query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
        }
        return Ok(await query.ToListAsync());
    }
    
}