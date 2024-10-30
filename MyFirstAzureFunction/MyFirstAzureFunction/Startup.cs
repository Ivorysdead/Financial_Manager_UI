using System.Diagnostics.CodeAnalysis;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyFirstAzureFunction.Implementations.Services;
using MyFirstAzureFunction.Interfaces;
using Serilog.Extensions.Logging;

[assembly: FunctionsStartup(typeof(MyFirstAzureFunction.Startup))]
namespace MyFirstAzureFunction
{
    [ExcludeFromCodeCoverage]
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<ILoggerFactory>(sc =>
            {
                var providerCollection = sc.GetService<LoggerProviderCollection>();
                var factory = new SerilogLoggerFactory(null, true, providerCollection);
                foreach (var provider in sc.GetServices<ILoggerProvider>())
                    factory.AddProvider(provider);
                return factory;
            });
            builder.Services.AddSingleton<IQuickCalculations, QuickCalculationsService>();
            builder.Services.AddSingleton<ILoan, LoanService>();
        }
    }
}