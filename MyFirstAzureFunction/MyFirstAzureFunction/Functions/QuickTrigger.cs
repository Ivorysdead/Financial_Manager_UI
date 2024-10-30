using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker.Http;
using MyFirstAzureFunction.Implementations.Services;
using MyFirstAzureFunction.Interfaces;
using MyFirstAzureFunction.Models;
using Newtonsoft.Json;

namespace MyFirstAzureFunction.Functions;

public class QuickTrigger
{
    private readonly ILogger<QuickTrigger> _logger;
    private readonly IQuickCalculations _calculationService;
    
    public QuickTrigger(ILogger<QuickTrigger> logger, IQuickCalculations calculationService)
    {
        _logger = logger;
        _calculationService = calculationService;
    }

    [Function("QuickTrigger")]
    public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    {
        
        var requestBody = req.ReadAsStringAsync().Result;
        var requestObject = JsonConvert.DeserializeObject<CalculationRequestModel>(requestBody);
        try{
            _logger.LogInformation($"Function Triggered: QuickTrigger at {DateTime.Now}");
            var result = _calculationService.Add(requestObject.a, requestObject.b);
            _logger.LogInformation($"Function completed: QuickTrigger at {DateTime.Now} \n Result: {result}");
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            await response.WriteStringAsync($"Result: {result}");
            
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error: {ex.Message}");
            var response = req.CreateResponse(HttpStatusCode.BadRequest);
            await response.WriteStringAsync($"Error: {ex.Message}");
            return response;
        }
        
    }

}