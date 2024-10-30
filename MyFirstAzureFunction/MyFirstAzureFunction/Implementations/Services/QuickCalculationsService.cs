using Microsoft.Extensions.Logging;
using MyFirstAzureFunction.Interfaces;

namespace MyFirstAzureFunction.Implementations.Services;

public class QuickCalculationsService: IQuickCalculations
{
    private readonly ILogger<QuickCalculationsService> _logger;
    public QuickCalculationsService(ILogger<QuickCalculationsService> logger)
    {
        _logger = logger;
    }   public int Add(string a, int b)
    {
        try
        {
            _logger.LogInformation($"QuickCalculations Add function accessed at: {DateTime.Now}"); 
            _logger.LogInformation($"Variable A value: {a} \n Variable B value: {b}"); 
            var result =Convert.ToInt32(a) + b;
            _logger.LogInformation($"Calculation completed: {DateTime.Now} \n Result: {result}");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error: {ex.Message}");
            throw;
        }
    }

    public int Subtract(string a, int b)
    {
        try
        {
            _logger.LogInformation($"QuickCalculations Subtract function accessed at: {DateTime.Now}"); 
            _logger.LogInformation($"Variable A value: {a} \n Variable B value: {b}"); 
            var result =Convert.ToInt32(a) - b;
            _logger.LogInformation($"Calculation completed: {DateTime.Now} \n Result: {result}");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error: {ex.Message}");
            throw;
        }
    }

    public int Multiply(string a, int b)
    {
        try
        {
            _logger.LogInformation($"QuickCalculations Multiply function accessed at: {DateTime.Now}"); 
            _logger.LogInformation($"Variable A value: {a} \n Variable B value: {b}"); 
            var result =Convert.ToInt32(a) * b;
            _logger.LogInformation($"Calculation completed: {DateTime.Now} \n Result: {result}");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error: {ex.Message}");
            throw;
        }
    }

    public int Divide(string a, int b)
    {
        try
        {
            _logger.LogInformation($"QuickCalculations Divide function accessed at: {DateTime.Now}"); 
            _logger.LogInformation($"Variable A value: {a} \n Variable B value: {b}"); 
            var result =Convert.ToInt32(a) / b;
            _logger.LogInformation($"Calculation completed: {DateTime.Now} \n Result: {result}");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error: {ex.Message}");
            throw;
        }
    }
}
