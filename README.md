# Employee Management API

A RESTful Web API built with **ASP.NET Core Web API, C#, Entity Framework Core, and SQL Server** to manage employee records.

## Features

- Create and retrieve employee records
- Update and delete employee details
- DTO-based request and response handling
- Input validation using Data Annotations
- Duplicate email detection with appropriate HTTP status codes
- Pagination for employee records
- Global exception handling middleware
- Structured error responses with trace IDs
- Entity Framework Core database integration
- Swagger UI for API testing
- SQL Server database migrations

  ## API Endpoints

Base route: `/api/Employees`

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/Employees` | Get employees with pagination, filtering, and sorting |
| GET | `/api/Employees/{id}` | Get an employee by ID |
| POST | `/api/Employees` | Create a new employee |
| PUT | `/api/Employees/{id}` | Update an existing employee |
| DELETE | `/api/Employees/{id}` | Delete an employee |

### Pagination, Filtering, and Sorting

Example request:

```http
GET /api/Employees?pageNumber=1&pageSize=10&empName=Rama&department=IT&sortBy=id&sortOrder=asc
```

Query parameters:

- `pageNumber` — Page to retrieve
- `pageSize` — Number of employees per page
- `empName` — Filter employees by name
- `department` — Filter employees by department
- `sortBy` — Field used for sorting
- `sortOrder` — Sort direction (`asc` or `desc`)

The paginated response includes:

- `PageNumber`
- `PageSize`
- `TotalEmployees`
- `TotalPages`
- `Data` — Employee records for the requested page

### Create an Employee

Example request body:

```json
{
  "name": "Rama Krishna",
  "email": "rama@example.com",
  "department": "IT",
  "salary": 50000
}
```

The API validates the input and prevents duplicate employee email addresses.

### Example Error Responses

- `400 Bad Request` — Invalid input
- `404 Not Found` — Employee does not exist
- `409 Conflict` — Duplicate email address
- `500 Internal Server Error` — Unexpected server error

## Technologies Used

- C#
- ASP.NET Core Web API (.NET 8)
- Entity Framework Core
- Microsoft SQL Server
- Data Transfer Objects (DTOs)
- LINQ and asynchronous programming
- Swagger / OpenAPI
- Git and GitHub

## Error Handling

The API uses appropriate HTTP status codes, including:

- `200 OK` — Request completed successfully
- `201 Created` — Employee created successfully
- `400 Bad Request` — Invalid request data
- `404 Not Found` — Employee not found
- `409 Conflict` — Duplicate employee email
- `500 Internal Server Error` — Unexpected server error

## Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server
- Visual Studio 2022 or another compatible IDE

### Setup

1. Clone this repository:

   ```bash
   git clone https://github.com/Ramakrishna4862/EmployeeManagementAPI.git
   ```

2. Open the solution or project in Visual Studio.

3. Configure the `DefaultConnection` connection string using User Secrets or another secure configuration method.

4. Apply the Entity Framework Core migrations:

   ```bash
   dotnet ef database update
   ```

5. Run the application:

   ```bash
   dotnet run
   ```

6. Open the Swagger URL shown in the terminal to explore and test the API.

## What I Learned

- Designing RESTful API endpoints
- Using DTOs to separate API contracts from database entities
- Validating incoming requests
- Handling database conflicts
- Implementing pagination
- Centralizing exception handling with middleware
- Working with Entity Framework Core migrations
- Testing API endpoints using Swagger

## Author

**Rama Krishna**

GitHub: [Ramakrishna4862](https://github.com/Ramakrishna4862)
