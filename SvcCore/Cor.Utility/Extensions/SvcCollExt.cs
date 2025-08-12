using Cor.App.Interfaces;
using Cor.Utility.Persistence;
using Cor.Utility.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cor.Utility.Extensions
{
    public static class SvcCollExt
    {
        public static IServiceCollection AddUtilitySvc(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<CoreDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("CoreDbCon")));
            services.AddScoped(typeof(ICoreRepository<>), typeof(CoreRepository<>));

            return services;
        }
    }
}
