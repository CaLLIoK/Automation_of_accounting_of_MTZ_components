using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automation_of_accounting_of_MTZ_components.EmployeeFolder
{
    public class EmployeeContext: DbContext
    {
        public EmployeeContext() : base("Automation_of_accounting_of_MTZ_components") 
        {
            //if (!Database.Exists("Automation_of_accounting_of_MTZ_components"))
            //    Database.SetInitializer(new DropCreateDatabaseAlways<DataContext>());
        }
        public DbSet<Employee> Employee { get; set; }
    }
}
