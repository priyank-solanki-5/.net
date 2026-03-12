using Microsoft.EntityFrameworkCore;
using PdfGeneratorApp.Models;

namespace PdfGeneratorApp.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Employee> Employees => Set<Employee>();
}