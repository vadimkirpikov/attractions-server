using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TouristServer.DAL;
using TouristServer.DAL.Dbo;
using TouristServer.Presentation.Dto;

namespace TouristServer.Presentation.Controllers;

[ApiController]
[Authorize]
[Route("v1/routes")]
public class UserRoutesController(ApplicationDbContext context, ILogger<UserRoutesController> logger) : Controller
{
    [HttpGet("simpleInfo")]
    public async Task<ActionResult<IEnumerable<UserRoute>>> GetRoutesOfUser()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var routes = await context.Routes
            .Where(r => r.UserId == userId)
            .ToListAsync();
        return Ok(routes);
    }

    [HttpGet("fullInfo/{id:guid}")]
    public async Task<ActionResult<UserRouteDto>> GetFullInfoOfUserRoute([FromRoute] Guid id)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var route = await context.Routes.SingleOrDefaultAsync(r => r.Id == id);
        if (route == null)
        {
            return NotFound($"Rote with id {id} not found");
        }

        if (route.UserId != userId)
        {
            return NotFound($"User with id {userId} is not owner of the route with id {id}");
        }
        var routeFull = await context.Routes
            .Where(r => r.Id == route.Id)
            .Include(r => r.RoutePlaces!)
            .ThenInclude(rp => rp.Place)
            .SingleOrDefaultAsync();

        var userRouteDto = new UserRouteDto
        {
            Name = routeFull!.Name,
            RoutePlaces = routeFull.RoutePlaces!.Select(rp => new RoutePlaceDto
            {
                PlaceId = rp.PlaceId,
                Cost = rp.Place!.Cost,
                PlaceName = rp.Place.Name,
                PlacePosition = rp.PlacePosition,
                Latitude = rp.Place!.Latitude,
                Longitude = rp.Place!.Longitude
            }).ToList()
        };
        return Ok(userRouteDto);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUserRoute([FromBody] UserRouteDtoReq userRouteDto)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var userRoute = new UserRoute()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = userRouteDto.Name!,
            RoutePlaces = []
        };
        var routePlaceList = userRouteDto.RoutePlaces?.Select(routePlaceDto => new RoutePlace()
            {
                Id = Guid.NewGuid(), PlacePosition = routePlaceDto.PlacePosition, UserRouteId = userRoute.Id, PlaceId = routePlaceDto.PlaceId,
            })
            .ToList();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            await context.Routes.AddAsync(userRoute);
            await context.SaveChangesAsync();
            if (routePlaceList != null)
            {
                await context.RoutePlaces.AddRangeAsync(routePlaceList);

            }
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return Created();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, $"Fail to create with message {ex.Message}");
        }
    }
    
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateUserRoute([FromRoute] Guid id, [FromBody] UserRouteDtoReq userRouteDto)
    {
        var existingUserRoute = await context.Routes.SingleOrDefaultAsync(r => r.Id == id);
        if (existingUserRoute == null)
        {
            return NotFound($"Route with id {id} not found");
        }
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        if (existingUserRoute.UserId != userId)
        {
            return NotFound($"User with id {userId} is not owner of the route with id {id}");
        }
        existingUserRoute.Name = userRouteDto.Name!;
        var routePlaceList = userRouteDto.RoutePlaces!.Select(routePlaceDto => new RoutePlace()
            {
                Id = Guid.NewGuid(), PlacePosition = routePlaceDto.PlacePosition, UserRouteId = existingUserRoute.Id, PlaceId = routePlaceDto.PlaceId,
            })
            .ToList();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            context.Routes.Update(existingUserRoute);
    
            var currentRoutePlaces = await context.RoutePlaces
                .Where(rp => rp.UserRouteId == existingUserRoute.Id)
                .ToListAsync();

            context.RoutePlaces.RemoveRange(currentRoutePlaces);
            await context.SaveChangesAsync();

            await context.AddRangeAsync(routePlaceList);
            await context.SaveChangesAsync();

            await transaction.CommitAsync();

            return Ok();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, $"Fail to update with message {ex.Message}");
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUserRoute([FromRoute] Guid id)
    {
        var existingUserRoute = await context.Routes.SingleOrDefaultAsync(r => r.Id == id);
        if (existingUserRoute == null)
        {
            return NotFound($"Route with id {id} not found");
        }
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        if (existingUserRoute.UserId != userId)
        {
            return NotFound($"User with id {userId} is not owner of the route with id {id}");
        }
        context.Routes.Remove(existingUserRoute);
        await context.SaveChangesAsync();
        return Ok("Successfully deleted");
    }
}