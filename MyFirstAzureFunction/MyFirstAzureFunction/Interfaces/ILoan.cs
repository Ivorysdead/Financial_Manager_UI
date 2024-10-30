using MyFirstAzureFunction.Models;

namespace MyFirstAzureFunction.Interfaces;

public interface ILoan
{
    List<LoanRequestModel> GetAllLoans();
    void AddLoan(LoanRequestModel loan);
    double GetTotalLoanAmount();
}