using PdfGeneratorApp.Models;

namespace PdfGeneratorApp.Services;

public interface IPdfService
{
    byte[] GenerateEmployeePdf(Employee employee);
}