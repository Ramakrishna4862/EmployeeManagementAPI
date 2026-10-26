using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagementAPI.Tests.MoqPractice   
{
    public interface IEmployeeService
    {
        string GetEmployeeName(int id);
    }
}
