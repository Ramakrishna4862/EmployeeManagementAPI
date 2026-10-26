using Moq;
using Xunit;
using EmployeeManagementAPI.Tests.MoqPractice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagementAPI.Tests.MoqPractice
{
    public class EmployeeReportService
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeReportService(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public string GetEmployeeReport(int id)
        {
            var name = _employeeService.GetEmployeeName(id);
            return $"Employee Report: {name}";
        }

    }
}
