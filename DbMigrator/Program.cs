using Db;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(
        (ctx, config) =>
        {
            config.AddJsonFile("appsettings.json", optional: false).AddEnvironmentVariables();
        }
    )
    .ConfigureServices(
        (ctx, services) =>
        {
            services.AddSingleton<DbLocatorWrapper>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var baseConnectionString =
                    config["ConnectionStrings:Base"]
                    ?? throw new InvalidOperationException(
                        "Base connection string is not configured."
                    );
                return new DbLocatorWrapper(baseConnectionString);
            });
            services.AddTransient<DatabaseMigration>();
        }
    )
    .Build();

using var scope = host.Services.CreateScope();
var migrator = scope.ServiceProvider.GetRequiredService<DatabaseMigration>();
await migrator.MigrateAllDatabasesAsync();

public class DatabaseMigration(DbLocatorWrapper dbLocatorWrapper)
{
    public async Task MigrateAllDatabasesAsync()
    {
        var databases = await dbLocatorWrapper.GetDatabases();

        foreach (var database in databases)
        {
            try
            {
                var builder = new SqlConnectionStringBuilder(dbLocatorWrapper.ConnectionString())
                {
                    InitialCatalog = database.Name
                };

                var connectionString = builder.ToString();
                var optionsBuilder = new DbContextOptionsBuilder<ClientDbContext>();
                optionsBuilder.UseSqlServer(connectionString);

                using var context = new ClientDbContext(optionsBuilder.Options);

                Console.WriteLine($"Migrating: {database.Name}");
                await context.Database.MigrateAsync();
                Console.WriteLine($"✅ Done: {database.Name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Migration failed for {database.Name}: {ex.Message}");
            }
        }
    }
}
