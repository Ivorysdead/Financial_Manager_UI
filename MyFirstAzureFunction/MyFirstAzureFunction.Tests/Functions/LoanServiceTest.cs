using MyFirstAzureFunction.Models;
using MyFirstAzureFunction.Implementations.Services;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MyFirstAzureFunction.Tests.Services
{
    public class LoanServiceTests
    {
        private LoanService _loanService;
        private readonly string _filePath = "loans.txt";
        
        [SetUp]
        public void Setup()
        {
            if (File.Exists(_filePath))
                File.Delete(_filePath); 
            _loanService = new LoanService();
        }

        [Test]
        public void GetAllLoans_WhenFileDoesNotExist_ReturnsEmptyList()
        {
            // Act
            var result = _loanService.GetAllLoans();

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void GetAllLoans_WhenFileHasValidLoans_ReturnsListOfLoans()
        {
            // Arrange
            var loans = new List<LoanRequestModel>
            {
                new LoanRequestModel { LoanId = 1, UserId = 1, LoanName = "Car", LoanAmount = 5000 },
                new LoanRequestModel { LoanId = 2, UserId = 2, LoanName = "House", LoanAmount = 200000 }
            };
            File.WriteAllLines(_filePath, loans.Select(JsonConvert.SerializeObject));

            // Act
            var result = _loanService.GetAllLoans();

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].LoanName, Is.EqualTo("Car"));
            Assert.That(result[1].LoanAmount, Is.EqualTo(200000));
        }

        
        [Test]
        public void AddLoan_WhenCalled_AppendsLoanToFile()
        {
            // Arrange
            var loan = new LoanRequestModel { LoanId = 1, UserId = 1, LoanName = "Car", LoanAmount = 5000 };

            // Act
            _loanService.AddLoan(loan);

            // Assert
            var result = File.ReadAllLines(_filePath).Select(JsonConvert.DeserializeObject<LoanRequestModel>).ToList();
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].LoanName, Is.EqualTo("Car"));
        }
        

        [Test]
        public void GetTotalLoanAmount_WhenCalled_ReturnsSumOfLoanAmounts()
        {
            // Arrange
            var loans = new List<LoanRequestModel>
            {
                new LoanRequestModel { LoanId = 1, UserId = 1, LoanName = "Car", LoanAmount = 5000 },
                new LoanRequestModel { LoanId = 2, UserId = 2, LoanName = "House", LoanAmount = 200000 }
            };
            File.WriteAllLines(_filePath, loans.Select(JsonConvert.SerializeObject));

            // Act
            var total = _loanService.GetTotalLoanAmount();

            // Assert
            Assert.That(total, Is.EqualTo(205000));
        }
        
    }
}