using Microsoft.EntityFrameworkCore;
using VerticalSliceAPI.Model;

namespace VerticalSliceAPI.Extensions
{
    public static class AppExtensions
    {
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();
            using AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            context.Database.Migrate();
        }
    }
}
