using Microsoft.Extensions.DependencyInjection;
using SalesTaxes.Configuration;
using SalesTaxes.Contracts.Services;

var serviceCollection = new ServiceCollection();
serviceCollection.ConfigureServices();
var serviceProvider = serviceCollection.BuildServiceProvider();
var app = serviceProvider.GetService<IApp>() ?? throw new ApplicationException("Unable to start application.");
app.Run();
