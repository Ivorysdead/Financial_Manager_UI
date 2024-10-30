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

public class GetLoanTests
{
    private readonly Mock<ILogger<GetLoans>> _logger;
    private readonly Mock<ILoan> _loan;
    private readonly GetLoans _sut; // System Under Test

    public GetLoanTests()
    {
        _logger = new Mock<ILogger<GetLoans>>();
        _loan = new Mock<ILoan>();
        _sut = new GetLoans(_loan.Object, _logger.Object);
    }

    [Test]
        public async Task Given_LoanServiceReturnsLoans_Then_ReturnsOkResponse()
        {
            // Arrange
             var loans = new List<LoanRequestModel>
            {
                new LoanRequestModel { LoanId = 1, UserId = 1, LoanName = "Car", LoanAmount = 10000 },
                new LoanRequestModel { LoanId = 2, UserId = 1, LoanName = "House", LoanAmount = 200000 }
            };
            _loan.Setup(service => service.GetAllLoans()).Returns(loans);

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddScoped<ILoggerFactory, LoggerFactory>();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var context = new Mock<FunctionContext>();
            context.SetupProperty(c => c.InstanceServices, serviceProvider);

            // Mock HttpRequestData
            var request = new Mock<HttpRequestData>(context.Object);

            // Step 1: Mock HttpResponseData and ensure the Body is set up correctly
            request.Setup(req => req.CreateResponse()).Returns(() =>
            {
                var response = new Mock<HttpResponseData>(context.Object);
                response.SetupProperty(r => r.Headers, new HttpHeadersCollection());
                response.SetupProperty(r => r.StatusCode, HttpStatusCode.OK);
                
                // Set up response body as a memory stream (this is step one)
                var memoryStream = new MemoryStream();
                response.SetupProperty(r => r.Body, memoryStream);
                return response.Object;
            });

            // Act
            var result = await _sut.RunAsync(request.Object);

            // Step 2: Read the response body and assert the content (this is step two)
            result.Body.Position = 0;  // Reset the stream position to read it
            var responseContent = await new StreamReader(result.Body).ReadToEndAsync();

            // Expected response (serialized loans)
            var expectedResponseContent = JsonConvert.SerializeObject(loans);

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responseContent, Is.EqualTo(expectedResponseContent));
            _loan.Verify(service => service.GetAllLoans(), Times.Once);
        }
    [Test]
    public async Task Given_LoanServiceThrowsException_Then_ReturnsBadRequestResponse()
    {
        // Arrange
        _loan.Setup(service => service.GetAllLoans()).Throws(new Exception("Service error"));

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped<ILoggerFactory, LoggerFactory>();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var context = new Mock<FunctionContext>();
        context.SetupProperty(c => c.InstanceServices, serviceProvider);

        // Mock HttpRequestData
        var request = new Mock<HttpRequestData>(context.Object);

        // Mock HttpResponseData
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
        var result = await _sut.RunAsync(request.Object);

        // Read the response body
        result.Body.Position = 0;
        var responseContent = await new StreamReader(result.Body).ReadToEndAsync();

        // Assert
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        Assert.That(responseContent, Is.EqualTo("Error: Service error"));
        _loan.Verify(service => service.GetAllLoans(), Times.Once);
    }
}
