using System.Net;
using System.Net.Http.Json;
using Api;
using Api.Actions.Products;
using Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Tests.Infrastructure;

namespace Tests.Products;

public class DeleteProductTests(ApiFixture fixture)
{
    [Fact]
    public async Task DeleteProduct_Success()
    {
        var ct = TestContext.Current.CancellationToken;
        var client = fixture.Api.CreateClient();

        var createReq = Sample.CreateProductRequest();
        var createRes = await client.PostAsJsonAsync("products", createReq, ApiFixture.JsonOptions, ct);
        createRes.EnsureSuccessStatusCode();
        var model = await createRes.Content.ReadFromJsonAsync<ProductModel>(ApiFixture.JsonOptions, ct);
        
        var uri = "products/" + model!.Id;
        var deleteRes = await client.DeleteAsync(uri, ct);
        deleteRes.EnsureSuccessStatusCode();
        
        var duplicateRes = await client.DeleteAsync(uri, ct);
        Assert.Equal(HttpStatusCode.NotFound, duplicateRes.StatusCode);
    }
    
    [Fact]
    public async Task EditProduct_Success_List()
    {
        var ct = TestContext.Current.CancellationToken;
        var client = fixture.Api.CreateClient();
        
        var createRequests = Enumerable.Range(0, 5)
            .Select(_ => Sample.CreateProductRequest() with { Name = "[DELETE] " + Guid.NewGuid() })
            .ToArray();
        foreach (var createRequest in createRequests)
        {
            var createRes = await client.PostAsJsonAsync("products", createRequest, ApiFixture.JsonOptions, ct);
            createRes.EnsureSuccessStatusCode();
        }

        var queryUri = UriHelper.BuildRelative("/products", 
            query: QueryString.Create(new Dictionary<string, string?>
            {
                [nameof(ListProducts.Request.Skip)] = "0",
                [nameof(ListProducts.Request.Take)] = "20",
                [nameof(ListProducts.Request.Query)] = "[DELETE]"
            }));
        var products = await client.GetFromJsonAsync<Paginated.Response<ProductModel>>(queryUri, ApiFixture.JsonOptions, ct);
        Assert.Equal(createRequests.Length, products!.Total);
        
        Queue<ProductModel> productsToDelete = new(products.Items);

        while (productsToDelete.Count != 0)
        {
            var deleteUri = "products/" + productsToDelete.Dequeue().Id;
            var deleteRes = await client.DeleteAsync(deleteUri, ct);
            deleteRes.EnsureSuccessStatusCode();
            
            products = await client.GetFromJsonAsync<Paginated.Response<ProductModel>>(queryUri, ApiFixture.JsonOptions, ct);
            Assert.Equal(productsToDelete.Count, products!.Total);
        }
    }
    
    
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MaxValue)]
    public async Task DeleteProduct_Error_Id(int id)
    {
        var ct = TestContext.Current.CancellationToken;
        var client = fixture.Api.CreateClient();
        
        var uri = "products/" + id;
        
        var res = await client.DeleteAsync(uri, ct);
        Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
    }
}