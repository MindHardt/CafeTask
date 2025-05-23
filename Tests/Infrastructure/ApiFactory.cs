using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Testcontainers.PostgreSql;

namespace Tests.Infrastructure;

public class ApiFactory(PostgreSqlContainer postgres) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureAppConfiguration(config =>
        {
            var configManager = new ConfigurationManager();
            configManager["ConnectionStrings:Postgres"] = postgres.GetConnectionString();
            config.AddConfiguration(configManager);
        });
    }
}