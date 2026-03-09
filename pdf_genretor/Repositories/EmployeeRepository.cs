using Microsoft.EntityFrameworkCore;
using PdfGeneratorApp.Data;
using PdfGeneratorApp.Models;

namespace PdfGeneratorApp.Repositories;

public class EmployeeRepository(AppDbContext dbContext) : IEmployeeRepository
{
    public async Task AddAsync(Employee employee)
    {
        dbContext.Employees.Add(employee);
        await dbContext.SaveChangesAsync();
    }

    public async Task<List<Employee>> GetAllAsync()
    {
        return await dbContext.Employees
            .OrderBy(employee => employee.Id)
            .ToListAsync();
    }

    public async Task<Employee?> GetByIdAsync(int id)
    {
        return await dbContext.Employees.FirstOrDefaultAsync(employee => employee.Id == id);
    }
}