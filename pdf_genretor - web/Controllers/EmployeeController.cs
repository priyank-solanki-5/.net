using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PdfGeneratorApp.Models;
using PdfGeneratorApp.Repositories;
using PdfGeneratorApp.Services;

namespace PdfGeneratorApp.Controllers;

public class EmployeeController(IEmployeeRepository employeeRepository, IPdfService pdfService) : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View(new Employee { DateOfBirth = DateTime.Today.AddYears(-18) });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(Employee employee)
    {
        if (!ModelState.IsValid)
        {
            return View(employee);
        }

        await employeeRepository.AddAsync(employee);
        TempData["SuccessMessage"] = $"Employee record saved successfully. Generated ID: {employee.Id}";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Download()
    {
        return View(await BuildSelectionModelAsync());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Download(EmployeePdfSelectionViewModel model)
    {
        if (model.SelectedEmployeeId is null)
        {
            ModelState.AddModelError(nameof(model.SelectedEmployeeId), "Please select an employee ID.");
            return View(await BuildSelectionModelAsync());
        }

        var employee = await employeeRepository.GetByIdAsync(model.SelectedEmployeeId.Value);
        if (employee is null)
        {
            ModelState.AddModelError(nameof(model.SelectedEmployeeId), "Selected employee was not found.");
            return View(await BuildSelectionModelAsync(model.SelectedEmployeeId));
        }

        var pdfBytes = pdfService.GenerateEmployeePdf(employee);
        return File(pdfBytes, "application/pdf", $"employee-{employee.Id}.pdf");
    }

    private async Task<EmployeePdfSelectionViewModel> BuildSelectionModelAsync(int? selectedEmployeeId = null)
    {
        var employees = await employeeRepository.GetAllAsync();

        return new EmployeePdfSelectionViewModel
        {
            SelectedEmployeeId = selectedEmployeeId,
            EmployeeOptions = employees
                .Select(employee => new SelectListItem
                {
                    Value = employee.Id.ToString(),
                    Text = $"{employee.Id} - {employee.Name}"
                })
                .ToList()
        };
    }
}