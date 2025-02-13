using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Power_BI_Service.SemanticModels;
using Power_BI_Service;

var builder = FunctionsApplication.CreateBuilder(args);
builder.ConfigureFunctionsWebApplication();


builder.Services.AddHttpClient();
builder.Services.AddSingleton<Token>();
builder.Services.AddSingleton<SemanticModelFunction>();

//var host = new HostBuilder()
//    .ConfigureFunctionsWorkerDefaults()
//    .ConfigureServices(services =>
//    {
//        services.AddHttpClient();            // Ensure HttpClient is registered globally
//        services.AddSingleton<Token>();       //  Ensure Token is registered
//        services.AddSingleton<SemanticModelFunction>(); // Ensure SemanticModelFunction is registered
//    })
//    .Build();


// Add HttpClient to the service collection


// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();

builder.Build().Run();