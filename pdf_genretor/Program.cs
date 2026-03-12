using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PdfGeneratorApp.Data;
using PdfGeneratorApp.Forms;
using PdfGeneratorApp.Repositories;
using PdfGeneratorApp.Services;
using QuestPDF.Infrastructure;

namespace PdfGeneratorApp;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        QuestPDF.Settings.License = LicenseType.Community;

        var builder = Host.CreateApplicationBuilder();
        var connectionString = builder.Configuration["ConnectionStrings:DefaultConnection"]
            ?? "Data Source=pdf-generator.db";

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(connectionString));
        builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        builder.Services.AddScoped<IPdfService, PdfService>();
        builder.Services.AddScoped<MainForm>();

        using var host = builder.Build();
        using var scope = host.Services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.EnsureCreated();

        var mainForm = scope.ServiceProvider.GetRequiredService<MainForm>();
        Application.Run(mainForm);
    }
}