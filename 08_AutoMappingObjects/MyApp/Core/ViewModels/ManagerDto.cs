using System.Collections.Generic;

namespace MyApp.Core.ViewModels
{
    class ManagerDto : EmployeeDto
    {
        public ManagerDto()
        {
            ManagedEmployees = new List<EmployeeDto>();
        }

        public List<EmployeeDto> ManagedEmployees { get; set; }
    }
}
