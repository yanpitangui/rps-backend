using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RPS.Context;

namespace RPS.Extensions
{
    public static class DatabaseExtension
    {
        public static IServiceCollection AddApplicationDbContext(this IServiceCollection services)
        {

            services.AddDbContextPool<ApplicationDbContext>(o =>
            {
                //o.UseSqlServer("");
                o.UseInMemoryDatabase(databaseName: "rpsdb");
            });

            return services;
        }
    }
}
