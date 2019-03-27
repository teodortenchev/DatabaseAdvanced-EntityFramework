using MyApp.Models;

namespace MyApp.Core.ViewModels
{
    class EmployeeManagerDto : EmployeeDto
    {
        public EmployeeDto Manager { get; set; }
    }
}
