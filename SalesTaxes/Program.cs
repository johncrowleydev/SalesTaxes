using Microsoft.Extensions.DependencyInjection;
using SalesTaxes;
using SalesTaxes.Configuration;
using SalesTaxes.Contracts.Services;

var serviceCollection = new ServiceCollection();
serviceCollection.AddSingleton<App>();
serviceCollection.ConfigureServices();
var serviceProvider = serviceCollection.BuildServiceProvider();
var app = serviceProvider.GetService<App>() ?? throw new ApplicationException("Unable to start application.");
app.Run();
