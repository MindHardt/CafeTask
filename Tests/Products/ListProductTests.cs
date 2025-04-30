using System.Net.Http.Json;
using Api;
using Api.Actions.Products;
using Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Tests.Infrastructure;

namespace Tests.Products;

public class ListProductTests(ApiFixture fixture)
{
    [Fact]
    public async Task ListProducts_Success_Filter()
    {
        var ct = TestContext.Current.CancellationToken;
        var client = fixture.Api.CreateClient();

        var namePrefix = nameof(ListProducts_Success_Filter) + " ";
        string[] names = ["Хлеб", "Ржаной хлеб", "Чесночный хлеб", "Борщ", "Ролл калифорния"];
        foreach (var name in names)
        {
            var createRequest = Sample.CreateProductRequest() with { Name = namePrefix + name };
            var createRes = await client.PostAsJsonAsync("products", createRequest, ApiFixture.JsonOptions, ct);
            createRes.EnsureSuccessStatusCode();
        }

        foreach (var name in names)
        {
            var queryUri = UriHelper.BuildRelative("/products", 
                query: QueryString.Create(new Dictionary<string, string?>
                {
                    [nameof(ListProducts.Request.Skip)] = "0",
                    [nameof(ListProducts.Request.Take)] = "20",
                    [nameof(ListProducts.Request.Query)] = name
                }));
            var response = await client.GetFromJsonAsync<Paginated.Response<ProductModel>>(queryUri, ct);
            var matchingNamesCount = names.Count(x => x.Contains(name, StringComparison.InvariantCultureIgnoreCase));
            
            Assert.Equal(matchingNamesCount, response!.Total);
        }
    }
}