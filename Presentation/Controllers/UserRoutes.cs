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
public class UserRoutesController(ApplicationDbContext context) : Controller
{
    
    [HttpGet("simpleInfo")]
    public async Task<IActionResult> GetRoutesOfUser()
    {
        var userId = Guid.Parse(ClaimTypes.NameIdentifier);
        var routes = await context.Routes
            .Where(r => r.UserId == userId)
            .ToListAsync();
        return Ok(routes);
    }

    [HttpGet("fullInfo")]
    public async Task<IActionResult> GetFullInfoOfUserRoutes()
    {
        var userId = Guid.Parse(ClaimTypes.NameIdentifier);
        var routes = await context.Routes
            .Where(r => r.UserId == userId)
            .Include(r => r.RoutePlaces!)
            .ThenInclude(rp => rp.Place)
            .ToListAsync();
        return Ok(routes);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUserRoute([FromBody] UserRouteDto userRouteDto)
    {
        var userId = Guid.Parse(ClaimTypes.NameIdentifier);
        var userRoute = new UserRoute()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = userRouteDto.Name!,
            RoutePlaces = []
        };
        var routePlaceList = userRouteDto.RoutePlaces!.Select(routePlaceDto => new RoutePlace()
            {
                Id = Guid.NewGuid(), PlacePosition = routePlaceDto.PlacePosition, UserRouteId = userRoute.Id, PlaceId = routePlaceDto.PlaceId,
            })
            .ToList();
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            await context.Routes.AddAsync(userRoute);
            await context.SaveChangesAsync();
            await context.RoutePlaces.AddRangeAsync(routePlaceList);
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
    public async Task<IActionResult> UpdateUserRoute([FromRoute] Guid id, [FromBody] UserRouteDto userRouteDto)
    {
        var existingUserRoute = await context.Routes.SingleOrDefaultAsync(r => r.Id == id);
        if (existingUserRoute == null)
        {
            return NotFound($"Route with id {id} not found");
        }
        var userId = Guid.Parse(ClaimTypes.NameIdentifier);
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
        var userId = Guid.Parse(ClaimTypes.NameIdentifier);
        if (existingUserRoute.UserId != userId)
        {
            return NotFound($"User with id {userId} is not owner of the route with id {id}");
        }
        context.Routes.Remove(existingUserRoute);
        await context.SaveChangesAsync();
        return Ok("Successfully deleted");
    }
}