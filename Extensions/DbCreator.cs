using TouristServer.DAL;

namespace TouristServer.Extensions;

public static class DbCreator
{
    public static WebApplication CreateDbIfNotExists(this WebApplication webApplication)
    {
        using var scope = webApplication.Services.CreateScope();
        var services = scope.ServiceProvider;
        var database= services.GetRequiredService<ApplicationDbContext>();
        var created = false;
        while (!created)
        {
            try
            {
                // database.Database.EnsureDeleted();
                database.Database.EnsureCreated();
                created = true;
            }
            catch (Exception ex)
            {
                // ignore
            }
        }
        return webApplication;
    }
}