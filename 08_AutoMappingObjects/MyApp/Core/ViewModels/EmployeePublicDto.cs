using System;

namespace MyApp.Core.ViewModels
{
    public class EmployeePublicDto : EmployeeDto
    {
        public DateTime? Birthday { get; set; }

        public string Address { get; set; }
    }
}
