using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TouristServer.DAL;
using TouristServer.DAL.Dbo;
using TouristServer.Presentation.Dto;

namespace TouristServer.Presentation.Controllers;

[ApiController]
[Authorize]
[Route("v1/places")]
public class PlacesController(ApplicationDbContext context) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<IEnumerable<PlaceDto>>> GetFilteredPlaces([FromBody] FilterDto? filter, [FromQuery] int? page, [FromQuery] int? pageSize)
    {
        
        var query = context.Places.AsQueryable();
        if (filter?.CategoryIds is { Count: 0 })
        {
            query = query.Where(p => filter.CategoryIds.Contains(p.CategoryId));
        }

        if (filter?.DistrictIds is { Count: 0 })
        {
            query = query.Where(p => filter.DistrictIds.Contains(p.DistrictId));
        }

        if (filter?.ConstMin != null)
        {
            query = query.Where(p => p.Cost >= filter.ConstMin);
        }

        if (filter?.ConstMax != null)
        {
            query = query.Where(p => p.Cost <= filter.ConstMax);
        }

        if (page != null && pageSize != null)
        {
            query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
        }
        
        return Ok(await query.Select(p => new PlaceDto()
        {
            Id = p.Id,
            Name = p.Name,
            Latitude = p.Latitude,
            Longitude = p.Longitude,
            DistrictName = p.District!.Name,
            PhotosUrl = p.Photos!.Select(ph => ph.Url).Take(1).ToList(),
            CategoryName = p.Category!.Name,
            Cost = p.Cost,
        }).ToListAsync());
    }
    
    [HttpGet("{placeId:guid}")]
    public async Task<ActionResult<PlaceDto>> GetPlace(
        [FromRoute] Guid placeId )
    {
        var place = await context.Places.Where(p => p.Id == placeId)
            .Include(p => p.Photos)
            .Include(p => p.District)
            .Include(p => p.Category)
            .SingleOrDefaultAsync();

        if (place == null) return NotFound();
        var placeDto = new PlaceDto()
        {
            Id = place.Id,
            Name = place.Name,
            Description = place.Description,
            DistrictName = place.District!.Name,
            CategoryName = place.Category!.Name,
            Latitude = place.Latitude,
            Longitude = place.Longitude,
            Cost = place.Cost,
            PhotosUrl = place.Photos!.Select(p => p.Url).ToList()
        };
        return Ok(placeDto);
    }
    
}