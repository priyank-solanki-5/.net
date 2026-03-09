using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PdfGeneratorApp.Models;

public class Employee
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "Employee Name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(250)]
    [Display(Name = "Employee Address")]
    public string Address { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    [Display(Name = "Employee DOB")]
    public DateTime DateOfBirth { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Range(typeof(decimal), "0", "999999999")]
    [Display(Name = "Pending Payment")]
    public decimal PendingPayment { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Range(typeof(decimal), "0", "999999999")]
    [Display(Name = "Payment Received")]
    public decimal PaymentReceived { get; set; }
}