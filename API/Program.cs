using Api;
using Data;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres"));
});
builder.Services.AddApiHandlers();
builder.Services.AddOpenApi();
builder.Services.ConfigureHttpJsonOptions(json => json.SerializerOptions.ConfigureDefaults());
builder.Services.AddDataProtection().PersistKeysToDbContext<DataContext>();
builder.Services.AddSerilog(logger =>
{
    logger.ReadFrom.Configuration(builder.Configuration);
});
builder.Services.AddHealthChecks();

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    await scope.ServiceProvider.GetRequiredService<DataContext>().Database.MigrateAsync();
}

if (app.Environment.IsProduction() is false)
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.MapHealthChecks("/healthz");
app.MapApiEndpoints();

await app.RunAsync();

public partial class Program; // for unit tests