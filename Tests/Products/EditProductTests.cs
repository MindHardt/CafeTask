using System.Net.Http.Json;
using Api.Actions.Products;
using Api.Models;
using Tests.Infrastructure;

namespace Tests.Products;

public class EditProductTests(ApiFixture fixture)
{
    [Fact]
    public async Task EditProduct_Success()
    {
        var ct = TestContext.Current.CancellationToken;
        var client = fixture.Api.CreateClient();

        var createReq = Sample.CreateProductRequest();
        var createRes = await client.PostAsJsonAsync("products", createReq, ApiFixture.JsonOptions, ct);
        createRes.EnsureSuccessStatusCode();
        var model = await createRes.Content.ReadFromJsonAsync<ProductModel>(ApiFixture.JsonOptions, ct);

        var editReq = new EditProduct.Request.Body
        {
            Name = Sample.CreateProductRequest().Name,
            Price = 250
        };
        var uri = "products/" + model!.Id;
        var editRes = await client.PutAsJsonAsync(uri, editReq, ApiFixture.JsonOptions, ct);
        editRes.EnsureSuccessStatusCode();
        var updatedModel = await editRes.Content.ReadFromJsonAsync<ProductModel>(ApiFixture.JsonOptions, ct);
        
        Assert.Equal(editReq.Name, updatedModel!.Name);
        Assert.Equal(editReq.Price, updatedModel.Price);
    }

    public static TheoryData<EditProduct.Request.Body> ErrorBodies { get; } =
    [
        new EditProduct.Request.Body { Name = null!, Price = 100 },
        new EditProduct.Request.Body { Name = string.Empty, Price = 100 },
        new EditProduct.Request.Body { Name = Sample.CreateProductRequest().Name, Price = 0 },
        new EditProduct.Request.Body { Name = Sample.CreateProductRequest().Name, Price = -1 },
    ];
    
    [Theory]
    [MemberData(nameof(ErrorBodies))]
    public async Task EditProduct_Error_Body(EditProduct.Request.Body body)
    {
        var ct = TestContext.Current.CancellationToken;
        var client = fixture.Api.CreateClient();

        var createReq = Sample.CreateProductRequest();
        var createRes = await client.PostAsJsonAsync("products", createReq, ApiFixture.JsonOptions, ct);
        createRes.EnsureSuccessStatusCode();
        var model = await createRes.Content.ReadFromJsonAsync<ProductModel>(ApiFixture.JsonOptions, ct);
        var uri = "products/" + model!.Id;
        
        var editRes = await client.PutAsJsonAsync(uri, body, ApiFixture.JsonOptions, ct);
        Assert.False(editRes.IsSuccessStatusCode);
    }
    
    
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MaxValue)]
    public async Task EditProduct_Error_Id(int id)
    {
        var ct = TestContext.Current.CancellationToken;
        var client = fixture.Api.CreateClient();
        
        var uri = "products/" + id;
        
        var editReq = new EditProduct.Request.Body
        {
            Name = Sample.CreateProductRequest().Name,
            Price = 250
        };
        var editRes = await client.PutAsJsonAsync(uri, editReq, ApiFixture.JsonOptions, ct);
        Assert.False(editRes.IsSuccessStatusCode);
    }
}