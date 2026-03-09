PdfGeneratorApp
│
├── Controllers
│   └── ReportController.cs
│
├── Models
│   └── Customer.cs
│
├── Services
│   ├── IPdfService.cs
│   └── PdfService.cs
│
├── Data
│   └── DbConnectionFactory.cs
│
├── Repositories
│   ├── ICustomerRepository.cs
│   └── CustomerRepository.cs
│
├── Views
│   └── Report
│       └── Index.cshtml
│
├── wwwroot
│
├── Program.cs
└── appsettings.json










currently i am only configure folder stcture.
i am not configure any .net project.
i am went make one pdf genreter all data should store in database which is sql.
in database one primary key, employee name, employee address, employee dob, employee panding payment, payment ressive etc fields.
in frontend make one form accoding database fields.
all form datashould store in database.
one page downlod pdf page provide downlod button and droupdown box in dropdown box show all users primary key value when i select value those perticuler person data should fatch in pdf.
i can downlod those pdf.