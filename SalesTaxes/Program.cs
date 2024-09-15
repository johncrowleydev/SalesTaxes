using Microsoft.Extensions.DependencyInjection;
using SalesTaxes;
using SalesTaxes.Configuration;

var serviceCollection = new ServiceCollection();
serviceCollection.AddSingleton<App>();
serviceCollection.ConfigureServices();
var serviceProvider = serviceCollection.BuildServiceProvider();
var app = serviceProvider.GetService<App>() ?? throw new ApplicationException("Unable to start application.");
app.Run();
