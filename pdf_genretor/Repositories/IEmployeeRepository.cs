using PdfGeneratorApp.Models;

namespace PdfGeneratorApp.Repositories;

public interface IEmployeeRepository
{
    Task AddAsync(Employee employee);
    Task<List<Employee>> GetAllAsync();
    Task<Employee?> GetByIdAsync(int id);
}