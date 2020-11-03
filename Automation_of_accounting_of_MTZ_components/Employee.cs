using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automation_of_accounting_of_MTZ_components
{
    public class Employee
    {
        [Key]
        public int EmployeeCode { get; set; }
        public string EmployeeLogin { get; set; }
        public string EmployeePassword { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeSurname { get; set; }
        public string EmployeePatronymic { get; set; }
        public int PostCode { get; set; }

        public Employee() { }

        public Employee(string employeeLogin, string employeePassword, string employeeName, string employeeSurname, string employeePatronymic, int postCode)
        {
            this.EmployeeLogin = employeeLogin;
            this.EmployeePassword = employeePassword;
            this.EmployeeName = employeeName;
            this.EmployeeSurname = employeeSurname;
            this.EmployeePatronymic = employeePatronymic;
            this.PostCode = postCode;
        }
    }
}
