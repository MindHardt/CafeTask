using System.Net.Http.Json;
using Api.Actions.Products;
using Api.Models;
using Tests.Infrastructure;

namespace Tests.Products;

public class CreateProductTests(ApiFixture fixture)
{
    [Fact]
    public async Task TestCreateProduct_Success()
    {
        var ct = TestContext.Current.CancellationToken;
        var client = fixture.Api.CreateClient();

        var req = Sample.CreateProductRequest();
        var res = await client.PostAsJsonAsync("products", req, ApiFixture.JsonOptions, ct);
        res.EnsureSuccessStatusCode();
        var result = await res.Content.ReadFromJsonAsync<ProductModel>(ct);
        
        Assert.NotEqual(0, result!.Id);
        Assert.Equal(req.Name, result.Name);
        Assert.Equal(req.Price, result.Price);
    }

    public static TheoryData<CreateProduct.Request> ErrorRequests { get; } =
    [
        Sample.CreateProductRequest() with { Name = null! },
        Sample.CreateProductRequest() with { Name = string.Empty },
        Sample.CreateProductRequest() with { Price = 0 },
        Sample.CreateProductRequest() with { Price = -1 }
    ];
    
    [Theory]
    [MemberData(nameof(ErrorRequests))]
    public async Task TestCreateProduct_Error(CreateProduct.Request request)
    {
        var ct = TestContext.Current.CancellationToken;
        var client = fixture.Api.CreateClient();
        
        var res = await client.PostAsJsonAsync("products", request, ApiFixture.JsonOptions, ct);
        Assert.False(res.IsSuccessStatusCode);
    }
}