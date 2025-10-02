using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Pluralsight.DotnetAzureFuncs;

public class NewPurchaseWebhook
{
    private readonly ILogger<NewPurchaseWebhook> _logger;

    public NewPurchaseWebhook(ILogger<NewPurchaseWebhook> logger)
    {
        _logger = logger;
    }

    [Function(nameof(NewPurchaseWebhook))]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "purchase")] HttpRequestData req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

        var name = req.Query.Get("name") ?? "Anonymous";
        await response.WriteStringAsync($"Welcome {name}!");

        return response;
    }

    record NewOrderWebhook(string productId, int quantity, string customerName, string customerEmail, decimal purchasePrice);
    
    [Function(nameof(GetPurchase))]
    public async Task<HttpResponseData> GetPurchase([HttpTrigger(AuthorizationLevel.Function, "get", Route ="purchase")] HttpRequestData req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        var order = await req.ReadFromJsonAsync<NewOrderWebhook>();

        var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

        var userAgent = req.Headers.GetValues("User-Agent").FirstOrDefault() ?? "Unknown"; 
        var name = req.Query.Get("name") ?? "Anonymous";
        await response.WriteStringAsync($"{order.customerName} purchased product {order.productId}!");

        return response;
    }
}