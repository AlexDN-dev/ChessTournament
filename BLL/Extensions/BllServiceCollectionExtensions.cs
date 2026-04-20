using BLL.Interfaces;
using BLL.Service;
using DAL.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace BLL.Extensions;

public static class BllServiceCollectionExtensions
{
    public static IServiceCollection AddBll(this IServiceCollection services, string connectionString)
    {
        services.AddDal(connectionString);

        services.AddScoped<IPlayerService, PlayerService>();
        services.AddScoped<ITournamentService, TournamentService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IScoreService, ScoreService>();

        return services;
    }
}
