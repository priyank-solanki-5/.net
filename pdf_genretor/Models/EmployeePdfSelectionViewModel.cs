using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PdfGeneratorApp.Models;

public class EmployeePdfSelectionViewModel
{
    [Display(Name = "Employee ID")]
    public int? SelectedEmployeeId { get; set; }

    public List<SelectListItem> EmployeeOptions { get; set; } = [];
}