namespace MyFirstAzureFunction.Interfaces;

public interface IQuickCalculations
{
    int Add(string a, int b);
    int Subtract(string a, int b);
    int Multiply(string a, int b);
    int Divide(string a, int b);
}