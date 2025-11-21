using ContractMonthlyClaimSystem.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUserStore, JsonUserStore>();
        services.AddScoped<IClaimStore, JsonClaimStore>();
        services.AddScoped<IFileGuard, FileGuard>();

        return services;
    }
}