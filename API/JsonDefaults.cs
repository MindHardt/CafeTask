using System.Text.Json;
using System.Text.Json.Serialization;

namespace Api;

public static class JsonDefaults
{
    public static void ConfigureDefaults(this JsonSerializerOptions options)
    {
        options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.PropertyNameCaseInsensitive = true;
        options.Converters.Add(new JsonStringEnumConverter());
    }

    public static JsonSerializerOptions WithDefaults(this JsonSerializerOptions options)
    {
        ConfigureDefaults(options);
        return options;
    }
}