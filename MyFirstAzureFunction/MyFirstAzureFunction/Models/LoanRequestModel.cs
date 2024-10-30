namespace MyFirstAzureFunction.Models;

public class LoanRequestModel
{
    public int LoanId { get; set; }
    public int UserId { get; set; }
    public string LoanName { get; set; }
    public double LoanAmount { get; set; }
}