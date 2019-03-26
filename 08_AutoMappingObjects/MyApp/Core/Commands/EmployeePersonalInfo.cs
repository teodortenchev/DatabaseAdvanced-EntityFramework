using AutoMapper;
using MyApp.Core.Contracts;
using MyApp.Core.ViewModels;
using MyApp.Data;
using System;
using System.Globalization;
using System.Text;

namespace MyApp.Core.Commands
{
    public class EmployeePersonalInfo : IExecutable
    {
        private readonly CompanyContext context;
        private readonly Mapper mapper;

        public EmployeePersonalInfo(CompanyContext context, Mapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public string Execute(string[] inputArgs)
        {
            int id = int.Parse(inputArgs[0]);

            var employee = context.Employees.Find(id);

            if (employee == null)
            {
                throw new InvalidOperationException($"Employee with Id {id} not found!");
            }

            var employeeDto = mapper.CreateMappedObject<EmployeePublicDto>(employee);

            string birthdate = employeeDto.Birthday.Value.ToString("dd-MM-yyyy");

            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"ID: {employeeDto.Id} - {employeeDto.FirstName} - {employee.LastName} - ${employeeDto.Salary}");
            sb.AppendLine($"Birthday: {birthdate}");
            sb.AppendLine($"Address: {employee.Address}");

            return sb.ToString().TrimEnd();


        }
    }
}
