using System.Net;
using System.Text;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using MyFirstAzureFunction.Functions;
using MyFirstAzureFunction.Interfaces;
using MyFirstAzureFunction.Models;
using Newtonsoft.Json;

namespace MyFirstAzureFunction.Tests.Functions;

public class AddLoanTest
{
    private readonly Mock<ILogger<AddLoan>> _logger;
    private readonly Mock<ILoan> _loan;
    private readonly AddLoan _sut; // System Under Test

    public AddLoanTest()
    {
        _logger = new Mock<ILogger<AddLoan>>();
        _loan = new Mock<ILoan>();
        _sut = new AddLoan(_loan.Object, _logger.Object);
    }
     [Test]
        public async Task Given_ValidLoanData_When_AddLoan_Then_ReturnsOkResponse()
        {
            var loanRequest = new LoanRequestModel { LoanId = 1, UserId = 1, LoanName = "Car", LoanAmount = 10000 };
            var requestBody = JsonConvert.SerializeObject(loanRequest);

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddScoped<ILoggerFactory, LoggerFactory>();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var context = new Mock<FunctionContext>();
            context.SetupProperty(c => c.InstanceServices, serviceProvider);

            // Step 1: Mock HttpRequestData and HttpResponseData
            var request = new Mock<HttpRequestData>(context.Object);
            request.Setup(req => req.Body).Returns(new MemoryStream(Encoding.UTF8.GetBytes(requestBody)));
            request.Setup(req => req.Headers).Returns(new HttpHeadersCollection());
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
            var result = await _sut.RunAsync(request.Object);

            // Step 2: Read the response and assert its content
            result.Body.Position = 0;
            var responseContent = await new StreamReader(result.Body).ReadToEndAsync();

            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responseContent, Is.EqualTo(requestBody));
            _logger.Verify(logger => logger.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()!), Times.Exactly(3));
            _loan.Verify(service => service.AddLoan(It.IsAny<LoanRequestModel>()), Times.Once);
        }
        
        [Test]
        public async Task Given_InvalidLoanData_When_AddLoan_Then_ReturnsBadRequestResponse()
        {
            // Arrange
            var requestBody = " ";

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddScoped<ILoggerFactory, LoggerFactory>();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var context = new Mock<FunctionContext>();
            context.SetupProperty(c => c.InstanceServices, serviceProvider);

            // Mock HttpRequestData and HttpResponseData
            var request = new Mock<HttpRequestData>(context.Object);
            request.Setup(req => req.Body).Returns(new MemoryStream(Encoding.UTF8.GetBytes(requestBody)));
            request.Setup(req => req.Headers).Returns(new HttpHeadersCollection());
            request.Setup(req => req.CreateResponse()).Returns(() =>
            {
                var response = new Mock<HttpResponseData>(context.Object);
                response.SetupProperty(r => r.Headers, new HttpHeadersCollection());
                response.SetupProperty(r => r.StatusCode, HttpStatusCode.BadRequest); // Change to BadRequest
                var memoryStream = new MemoryStream();
                response.SetupProperty(r => r.Body, memoryStream);
                return response.Object;
            });

            // Act
            var result = await _sut.RunAsync(request.Object);

            // Assert
            result.Body.Position = 0;
            var responseContent = await new StreamReader(result.Body).ReadToEndAsync();

            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));  // This ensures it's BadRequest
            Assert.That(responseContent, Is.EqualTo("Error: Invalid loan data"));
            _loan.Verify(service => service.AddLoan(It.IsAny<LoanRequestModel>()), Times.Never);
        }
}