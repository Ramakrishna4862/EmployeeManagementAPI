# Employee Management API

A RESTful Web API built using ASP.NET Core Web API, C#, Entity Framework Core, and SQL Server to manage employee records. This project demonstrates backend development, data validation, error handling, and automated testing using .NET 8.

## Technologies Used

- C# and ASP.NET Core Web API (.NET 8)
- Entity Framework Core 8
- Microsoft SQL Server
- Data Transfer Objects (DTOs) and Data Annotations
- LINQ and asynchronous programming
- Swagger / OpenAPI
- xUnit and Moq
- EF Core InMemory for database-related tests
- Git and GitHub

## Features

- Create, retrieve, update, and delete employee records
- Validate employee data, including required fields, email addresses, string lengths, and salary ranges
- Prevent duplicate employee email addresses
- Return appropriate HTTP status codes
- Paginate employee records with response metadata
- Filter employees by name and department
- Sort employee records by supported fields and sort direction
- Handle unexpected exceptions through custom middleware
- Return structured error responses with trace IDs
- Test API behavior and exception handling with automated tests

## Employee Fields

- `Id`
- `Name`
- `Email`
- `Department`
- `Salary`
- `CreatedDate`

## API Endpoints

Base route: `/api/Employees`

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/Employees` | Retrieve employees with pagination, filtering, and sorting |
| GET | `/api/Employees/{id}` | Retrieve an employee by ID |
| POST | `/api/Employees` | Create an employee |
| PUT | `/api/Employees/{id}` | Update an employee |
| DELETE | `/api/Employees/{id}` | Delete an employee |

## Pagination, Filtering, and Sorting

Example request:

```http
GET /api/Employees?pageNumber=1&pageSize=10&empName=Rama&department=IT&sortBy=name&sortOrder=asc
```

Supported query parameters:

- `pageNumber`: Page to retrieve
- `pageSize`: Number of employees per page
- `empName`: Filter by employee name
- `department`: Filter by department
- `sortBy`: Supported field used for sorting
- `sortOrder`: Sort direction (`asc` or `desc`)

The paginated response includes page information, total employee count, total pages, and the employee data for the requested page.

## Example Create Employee Request

```json
{
  "name": "Rama Krishna",
  "email": "rama@example.com",
  "department": "IT",
  "salary": 25000
}
```

## HTTP Status Codes

- `200 OK`: Request completed successfully
- `201 Created`: Employee created successfully
- `204 No Content`: Update or delete completed successfully
- `400 Bad Request`: Invalid request data
- `404 Not Found`: Employee not found
- `409 Conflict`: Duplicate employee email
- `500 Internal Server Error`: Unexpected server error

## Exception Handling

Custom exception middleware logs unexpected exceptions and returns a standardized error response containing an error message and trace ID.

## Getting Started

### Prerequisites

- .NET 8 SDK
- Visual Studio 2022 or a compatible IDE
- SQL Server
- SQL Server Management Studio (SSMS), recommended

### Setup Instructions

1. Clone the repository:

   ```bash
   git clone https://github.com/Ramakrishna4862/EmployeeManagementAPI.git
   ```

2. Open the solution in Visual Studio.

3. Configure the `DefaultConnection` connection string for your local SQL Server instance. Use User Secrets or another secure local configuration method. Do not commit passwords or credentials.

4. If the project has EF Core migrations and the EF CLI is installed, apply them from the project directory:

   ```bash
   dotnet ef database update
   ```

5. Run the API:

   ```bash
   dotnet run
   ```

6. Open the Swagger URL displayed in the terminal to explore and test the endpoints.

## Running Automated Tests

Run the tests from the repository root:

```bash
dotnet test
```

The test project uses xUnit, EF Core InMemory, and Moq where appropriate to test validation, controller behavior, and exception handling.

## What I Learned

- Designing RESTful APIs with ASP.NET Core
- Implementing CRUD operations using Entity Framework Core
- Using DTOs and Data Annotations for input validation
- Handling duplicate records and appropriate HTTP responses
- Implementing pagination, filtering, and sorting
- Centralizing error handling with custom middleware
- Writing automated tests using xUnit and Moq

## Future Improvements

- JWT authentication and role-based authorization
- Additional service-layer tests
- Integration tests using a real test database
- Continuous integration using GitHub Actions
- Deployment to a cloud hosting platform

## Author

Rama Krishna

GitHub: https://github.com/Ramakrishna4862
