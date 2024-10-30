using System.Net;
using System.Text;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using MyFirstAzureFunction.Functions;
using MyFirstAzureFunction.Interfaces;
using MyFirstAzureFunction.Models;
using NUnit.Framework;
namespace MyFirstAzureFunction.Tests.Functions;

public class TotalLoanAmountTests
{
    private readonly Mock<ILogger<TotalLoanAmount>> _logger;
    private readonly Mock<ILoan> _loan;
    private readonly TotalLoanAmount _sut; // System Under Test

    public TotalLoanAmountTests()
    {
        _logger = new Mock<ILogger<TotalLoanAmount>>();
        _loan = new Mock<ILoan>();
        _sut = new TotalLoanAmount(_loan.Object, _logger.Object);
    }

    [Test]
    public async Task Given_LoanServiceReturnsTotal_When_GetTotalLoanAmount_Then_ReturnsOkResponse()
    {
        // Arrange
        var totalAmount = 5000.0; // Example total loan amount
        _loan.Setup(service => service.GetTotalLoanAmount()).Returns(totalAmount);

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped<ILoggerFactory, LoggerFactory>();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var context = new Mock<FunctionContext>();
        context.SetupProperty(c => c.InstanceServices, serviceProvider);

        // Step 1: Mock HttpRequestData and HttpResponseData
        var request = new Mock<HttpRequestData>(context.Object);
        request.Setup(req => req.CreateResponse()).Returns(() =>
        {
            var response = new Mock<HttpResponseData>(context.Object);
            response.SetupProperty(r => r.Headers, new HttpHeadersCollection());
            response.SetupProperty(r => r.StatusCode, HttpStatusCode.OK);
            var memoryStream = new MemoryStream();
            response.SetupProperty(r => r.Body, memoryStream);
            return response.Object;
        });

        // Act
        var result = await _sut.GetTotalLoanAmount(request.Object);

        // Step 2: Read the response and assert its content
        result.Body.Position = 0;
        var responseContent = await new StreamReader(result.Body).ReadToEndAsync();

        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(responseContent, Is.EqualTo(JsonConvert.SerializeObject(new { TotalLoanAmount = totalAmount })));
        _logger.Verify(logger => logger.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()!), Times.Exactly(2));
        _loan.Verify(service => service.GetTotalLoanAmount(), Times.Once);
    }
    [Test]
    public async Task Given_LoanServiceThrowsException_When_GetTotalLoanAmount_Then_ReturnsBadRequestResponse()
    {
        // Arrange
        _loan.Setup(service => service.GetTotalLoanAmount()).Throws(new Exception("Service error"));

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped<ILoggerFactory, LoggerFactory>();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var context = new Mock<FunctionContext>();
        context.SetupProperty(c => c.InstanceServices, serviceProvider);

        // Mock HttpRequestData and HttpResponseData
        var request = new Mock<HttpRequestData>(context.Object);
        request.Setup(req => req.CreateResponse()).Returns(() =>
        {
            var response = new Mock<HttpResponseData>(context.Object);
            response.SetupProperty(r => r.Headers, new HttpHeadersCollection());
            response.SetupProperty(r => r.StatusCode, HttpStatusCode.BadRequest);
            var memoryStream = new MemoryStream();
            response.SetupProperty(r => r.Body, memoryStream);
            return response.Object;
        });

        // Act
        var result = await _sut.GetTotalLoanAmount(request.Object);

        // Step 2: Read the response and assert its content
        result.Body.Position = 0;
        var responseContent = await new StreamReader(result.Body).ReadToEndAsync();

        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        Assert.That(responseContent, Is.EqualTo("Error: Service error"));
        _loan.Verify(service => service.GetTotalLoanAmount(), Times.Once);
    }
}
