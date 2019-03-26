using AutoMapper;
using MyApp.Core.Contracts;
using MyApp.Core.ViewModels;
using MyApp.Data;
using System;

namespace MyApp.Core.Commands
{
    public class EmployeeInfoCommand : IExecutable
    {
        private readonly CompanyContext context;
        private readonly Mapper mapper;

        public EmployeeInfoCommand(CompanyContext context, Mapper mapper)
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

            var employeeDto = mapper.CreateMappedObject<EmployeeDto>(employee);

            string result = $"ID: {employeeDto.Id} - Full Name: {employeeDto.FirstName} {employeeDto.LastName} - Salary: ${employeeDto.Salary}";

            return result;
        }
    }
}
