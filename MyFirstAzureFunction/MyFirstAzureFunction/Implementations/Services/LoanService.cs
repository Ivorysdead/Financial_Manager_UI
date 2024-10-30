using MyFirstAzureFunction.Interfaces;
using MyFirstAzureFunction.Models;
using Newtonsoft.Json;

namespace MyFirstAzureFunction.Implementations.Services;

public class LoanService : ILoan
{
    private readonly string _filePath =  "loans.txt";
    

    public List<LoanRequestModel> GetAllLoans()
    {
        try
        {
            if (!File.Exists(_filePath))
            {
                return new List<LoanRequestModel>();
            }

            var loans = File.ReadAllLines(_filePath)
                .Select(JsonConvert.DeserializeObject<LoanRequestModel>)
                .ToList();
                
            return loans;
        }
        catch (Exception ex)
        {
            throw new Exception("Error reading loans from file.", ex);
        }
    }

    public void AddLoan(LoanRequestModel loan)
    {
        try
        {
            var serializedLoan = JsonConvert.SerializeObject(loan);
            File.AppendAllText(_filePath, serializedLoan + Environment.NewLine);
        }
        catch (Exception ex)
        {
            
            throw new Exception("Error writing loan to file.", ex);
        }
    }

    public double GetTotalLoanAmount()
    {
        try
        {
            var loans = GetAllLoans();
            return loans.Sum(loan => loan.LoanAmount);
        }
        catch (Exception ex)
        {
            throw new Exception("Error calculating total loan amount.", ex);
        }

    }
}