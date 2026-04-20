using DAL.Context;
using DAL.Interfaces;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DAL.Extensions;

public static class DalServiceCollectionExtensions
{
    public static IServiceCollection AddDal(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ChessTournamentContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IPlayerRepository, PlayerRepository>();
        services.AddScoped<ITournamentRepository, TournamentRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();

        return services;
    }
}
