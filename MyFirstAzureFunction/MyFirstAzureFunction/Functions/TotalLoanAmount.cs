using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using MyFirstAzureFunction.Interfaces;
using Newtonsoft.Json;

namespace MyFirstAzureFunction.Functions;

public class TotalLoanAmount
{
    private readonly ILoan _loanService;
    private readonly ILogger<TotalLoanAmount> _logger;

    public TotalLoanAmount(ILoan loanService, ILogger<TotalLoanAmount> logger)
    {
        _loanService = loanService;
        _logger = logger;
    }
    [Function("GetTotalLoanAmount")]
    public async Task<HttpResponseData> GetTotalLoanAmount([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
    {
        try
        {
            _logger.LogInformation($"Function Triggered: GetTotalLoanAmount at {DateTime.Now}");
            var totalAmount = _loanService.GetTotalLoanAmount();
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            await response.WriteStringAsync(JsonConvert.SerializeObject(new { TotalLoanAmount = totalAmount }));
            _logger.LogInformation($"Function completed: GetTotalLoanAmount at {DateTime.Now}");
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