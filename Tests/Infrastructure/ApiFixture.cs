using System.Text.Json;
using Api;
using Testcontainers.PostgreSql;

namespace Tests.Infrastructure;

public class ApiFixture : IAsyncLifetime
{
    public PostgreSqlContainer Postgres { get; }
    public ApiFactory Api { get; }

    public static JsonSerializerOptions JsonOptions { get; } = new JsonSerializerOptions().WithDefaults();

    public ApiFixture()
    {
        Postgres = new PostgreSqlBuilder()
            .WithDatabase("cafe_task")
            .WithImage("postgres:17.4")
            .Build();
        Api = new ApiFactory(Postgres);
    }

    public async ValueTask InitializeAsync()
    {
        await Postgres.StartAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await Api.DisposeAsync();
        await Postgres.DisposeAsync();
    }
}