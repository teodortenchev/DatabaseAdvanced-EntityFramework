using AutoMapper;
using MyApp.Core.Contracts;
using MyApp.Core.ViewModels;
using MyApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyApp.Core.Commands
{
    public class ListEmployeesOlderThan : IExecutable
    {
        private readonly CompanyContext context;
        private readonly Mapper mapper;

        public ListEmployeesOlderThan(CompanyContext context, Mapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public string Execute(string[] inputArgs)
        {
            int age = int.Parse(inputArgs[0]);

            var employees = context.Employees.Where(a => (DateTime.Now - a.Birthday.Value).TotalDays > age * 365.2425).ToList();

            List<EmployeeManagerDto> employeesWithManagers = new List<EmployeeManagerDto>();


          
            foreach (var employee in employees)
            {
                employeesWithManagers.Add(mapper.CreateMappedObject<EmployeeManagerDto>(employee));
            }

            string result = String.Join(Environment.NewLine, employeesWithManagers.Select(x =>
            $"{x.FirstName} {x.LastName} - ${x.Salary:F2} Manager: {((x.Manager == null) ? "[No Manager]" : $"{x.Manager.FirstName} {x.Manager.LastName}")}"));

            return result;
        }
    }
}
