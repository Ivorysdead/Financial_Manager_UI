using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using MyFirstAzureFunction.Implementations.Services;
using MyFirstAzureFunction.Interfaces;
using Newtonsoft.Json;


namespace MyFirstAzureFunction.Functions;

public class GetLoans
{
    private readonly ILoan _loanService;
    private readonly ILogger<GetLoans> _logger;

    public GetLoans(ILoan loanService, ILogger<GetLoans> logger)
    {
        _loanService = loanService;
        _logger = logger;
    }
    
    [Function("GetLoans")]
    public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
    {
        try
        {
            _logger.LogInformation($"Function Triggered: GetLoans at {DateTime.Now}");
            var loans = _loanService.GetAllLoans();
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            await response.WriteStringAsync(JsonConvert.SerializeObject(loans));
            _logger.LogInformation($"Function completed: GetLoans at {DateTime.Now}");
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