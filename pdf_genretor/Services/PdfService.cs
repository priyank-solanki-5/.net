using PdfGeneratorApp.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace PdfGeneratorApp.Services;

public class PdfService : IPdfService
{
    public byte[] GenerateEmployeePdf(Employee employee)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Size(PageSizes.A4);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header()
                    .Text("Employee Report")
                    .SemiBold()
                    .FontSize(22)
                    .FontColor(Colors.Blue.Darken2);

                page.Content().PaddingVertical(20).Column(column =>
                {
                    column.Spacing(10);

                    column.Item().Text($"Employee ID: {employee.Id}");
                    column.Item().Text($"Employee Name: {employee.Name}");
                    column.Item().Text($"Employee Address: {employee.Address}");
                    column.Item().Text($"Employee DOB: {employee.DateOfBirth:dd MMM yyyy}");
                    column.Item().Text($"Pending Payment: {employee.PendingPayment:C}");
                    column.Item().Text($"Payment Received: {employee.PaymentReceived:C}");

                    column.Item().PaddingTop(10).LineHorizontal(1);
                    column.Item().Text("This PDF was generated from the stored SQL Server data.");
                });

                page.Footer()
                    .AlignCenter()
                    .Text($"Generated on {DateTime.Now:dd MMM yyyy hh:mm tt}");
            });
        }).GeneratePdf();
    }
}