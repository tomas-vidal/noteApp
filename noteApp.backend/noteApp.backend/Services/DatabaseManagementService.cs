using Microsoft.EntityFrameworkCore;
using noteApp.backend.Data;

public static class DatabaseManagementService
{
    public static void MigrationInitialisation(IApplicationBuilder app)
    {
        using (var serviceScope = app.ApplicationServices.CreateScope())
        {
            serviceScope.ServiceProvider.GetService<dbContext>().Database.Migrate();
        }
    }
}