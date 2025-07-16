using System.Data;
using DbLocator;
using DbLocator.Domain;
using Microsoft.Extensions.Hosting;

namespace Db;

public class DbLocatorInitializer(DbLocatorWrapper dbLocatorWrapper) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        const int maxRetries = 10;
        const int delayMs = 3000;

        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                await dbLocatorWrapper.AddDatabaseType();
                await dbLocatorWrapper.AddDatabaseServer();
                return;
            }
            catch (Exception ex)
            {
                await Task.Delay(delayMs, cancellationToken);
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
