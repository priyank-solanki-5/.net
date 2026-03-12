using System.ComponentModel.DataAnnotations;
using PdfGeneratorApp.Models;
using PdfGeneratorApp.Repositories;
using PdfGeneratorApp.Services;

namespace PdfGeneratorApp.Forms;

public class MainForm : Form
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IPdfService _pdfService;

    private readonly TextBox _nameTextBox = new();
    private readonly TextBox _addressTextBox = new();
    private readonly DateTimePicker _dobPicker = new();
    private readonly NumericUpDown _pendingNumeric = new();
    private readonly NumericUpDown _receivedNumeric = new();
    private readonly ComboBox _employeeComboBox = new();
    private readonly Button _saveButton = new();
    private readonly Button _pdfButton = new();
    private readonly Label _statusLabel = new();

    public MainForm(IEmployeeRepository employeeRepository, IPdfService pdfService)
    {
        _employeeRepository = employeeRepository;
        _pdfService = pdfService;

        InitializeUi();
        Load += async (_, _) => await RefreshEmployeeListAsync();
    }

    private void InitializeUi()
    {
        Text = "PDF Generator - Desktop";
        StartPosition = FormStartPosition.CenterScreen;
        Width = 760;
        Height = 560;

        var table = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(16),
            ColumnCount = 2,
            RowCount = 9,
            AutoSize = true
        };

        table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        for (var i = 0; i < table.RowCount; i++)
        {
            table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        }

        _addressTextBox.Multiline = true;
        _addressTextBox.Height = 70;

        _dobPicker.Format = DateTimePickerFormat.Short;
        _dobPicker.Value = DateTime.Today.AddYears(-18);

        _pendingNumeric.DecimalPlaces = 2;
        _pendingNumeric.Maximum = 999999999;

        _receivedNumeric.DecimalPlaces = 2;
        _receivedNumeric.Maximum = 999999999;

        _employeeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

        _saveButton.Text = "Save Employee";
        _saveButton.Height = 34;
        _saveButton.Click += async (_, _) => await SaveEmployeeAsync();

        _pdfButton.Text = "Generate PDF";
        _pdfButton.Height = 34;
        _pdfButton.Click += async (_, _) => await GeneratePdfAsync();

        _statusLabel.AutoSize = true;
        _statusLabel.ForeColor = Color.DarkGreen;

        AddRow(table, 0, "Employee Name", _nameTextBox);
        AddRow(table, 1, "Employee Address", _addressTextBox);
        AddRow(table, 2, "Employee DOB", _dobPicker);
        AddRow(table, 3, "Pending Payment", _pendingNumeric);
        AddRow(table, 4, "Payment Received", _receivedNumeric);
        AddRow(table, 5, "Select Employee", _employeeComboBox);

        var buttonPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight
        };

        buttonPanel.Controls.Add(_saveButton);
        buttonPanel.Controls.Add(_pdfButton);

        AddRow(table, 6, "Actions", buttonPanel);
        AddRow(table, 7, "Status", _statusLabel);

        Controls.Add(table);
    }

    private static void AddRow(TableLayoutPanel table, int row, string labelText, Control control)
    {
        var label = new Label
        {
            Text = labelText,
            Anchor = AnchorStyles.Left,
            AutoSize = true,
            Padding = new Padding(0, 8, 0, 0)
        };

        control.Dock = DockStyle.Top;
        control.Margin = new Padding(0, 4, 0, 10);

        table.Controls.Add(label, 0, row);
        table.Controls.Add(control, 1, row);
    }

    private async Task SaveEmployeeAsync()
    {
        var employee = new Employee
        {
            Name = _nameTextBox.Text.Trim(),
            Address = _addressTextBox.Text.Trim(),
            DateOfBirth = _dobPicker.Value.Date,
            PendingPayment = _pendingNumeric.Value,
            PaymentReceived = _receivedNumeric.Value
        };

        var validationErrors = ValidateEmployee(employee);
        if (validationErrors.Count > 0)
        {
            MessageBox.Show(string.Join(Environment.NewLine, validationErrors), "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        await _employeeRepository.AddAsync(employee);
        _statusLabel.Text = $"Employee saved with ID: {employee.Id}";

        ClearForm();
        await RefreshEmployeeListAsync(employee.Id);
    }

    private async Task GeneratePdfAsync()
    {
        if (_employeeComboBox.SelectedItem is not EmployeeOption selectedEmployee)
        {
            MessageBox.Show("Please select an employee first.", "Missing Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var employee = await _employeeRepository.GetByIdAsync(selectedEmployee.Id);
        if (employee is null)
        {
            MessageBox.Show("Selected employee was not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            await RefreshEmployeeListAsync();
            return;
        }

        var pdfBytes = _pdfService.GenerateEmployeePdf(employee);

        using var dialog = new SaveFileDialog
        {
            Filter = "PDF files (*.pdf)|*.pdf",
            FileName = $"employee-{employee.Id}.pdf",
            Title = "Save Employee PDF"
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await File.WriteAllBytesAsync(dialog.FileName, pdfBytes);
        _statusLabel.Text = $"PDF saved: {dialog.FileName}";

        MessageBox.Show("PDF generated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private async Task RefreshEmployeeListAsync(int? selectedEmployeeId = null)
    {
        var employees = await _employeeRepository.GetAllAsync();

        var options = employees
            .Select(employee => new EmployeeOption(employee.Id, employee.Name))
            .ToList();

        _employeeComboBox.DataSource = options;

        if (selectedEmployeeId.HasValue)
        {
            var selected = options.FirstOrDefault(option => option.Id == selectedEmployeeId.Value);
            if (selected is not null)
            {
                _employeeComboBox.SelectedItem = selected;
            }
        }
    }

    private void ClearForm()
    {
        _nameTextBox.Clear();
        _addressTextBox.Clear();
        _dobPicker.Value = DateTime.Today.AddYears(-18);
        _pendingNumeric.Value = 0;
        _receivedNumeric.Value = 0;
    }

    private static List<string> ValidateEmployee(Employee employee)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(employee);

        Validator.TryValidateObject(employee, context, results, validateAllProperties: true);

        return results
            .Select(result => result.ErrorMessage)
            .Where(message => !string.IsNullOrWhiteSpace(message))
            .Cast<string>()
            .ToList();
    }

    private sealed record EmployeeOption(int Id, string Name)
    {
        public override string ToString() => $"{Id} - {Name}";
    }
}
