using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using MyFirstAzureFunction.Implementations.Services;
using MyFirstAzureFunction.Interfaces;
using MyFirstAzureFunction.Models;
using Newtonsoft.Json;

namespace MyFirstAzureFunction.Functions;

public class AddLoan
{
    private readonly ILoan _loanService;
    private readonly ILogger<AddLoan> _logger;

    public AddLoan(ILoan loanService, ILogger<AddLoan> logger)
    {
        _loanService = loanService;
        _logger = logger;
    }

    [Function("AddLoan")]
    public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    {
        try
        {
            _logger.LogInformation($"Function Triggered: AddLoan at {DateTime.Now}");
            var requestBody = await req.ReadAsStringAsync();
            var loan = JsonConvert.DeserializeObject<LoanRequestModel>(requestBody);

            if (loan == null)
            {
                throw new ArgumentException("Invalid loan data");
            }

            _loanService.AddLoan(loan);

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            await response.WriteStringAsync(JsonConvert.SerializeObject(loan));
            _logger.LogInformation($"Function completed: AddLoan at {DateTime.Now}");
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