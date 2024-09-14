using Microsoft.Extensions.DependencyInjection;
using SalesTaxes.Contracts.Services;
using SalesTaxes.Services;

namespace SalesTaxes.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.AddSingleton<ICartService, CartService>();
        services.AddSingleton<IInventoryService, InventoryService>();
        services.AddSingleton<ISalesTaxService, SalesTaxService>();

        return services;
    }
}
