using AutoMapper;
using MyApp.Core.Contracts;
using MyApp.Core.ViewModels;
using MyApp.Data;
using System;
using System.Globalization;

namespace MyApp.Core.Commands
{
    public class SetBirthdayCommand : IExecutable
    {
        private readonly CompanyContext context;
        private readonly Mapper mapper;

        public SetBirthdayCommand(CompanyContext context, Mapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public string Execute(string[] inputArgs)
        {
            int id = int.Parse(inputArgs[0]);
            DateTime birthdate = DateTime.ParseExact(inputArgs[1], "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var employee = context.Employees.Find(id);

            if (employee == null)
            {
                throw new InvalidOperationException($"Employee with Id {id} not found!");
            }

            employee.Birthday = birthdate;

            context.SaveChanges();

            var employeeDto = mapper.CreateMappedObject<EmployeeDto>(employee);

            string result = $"{employeeDto.FirstName} {employeeDto.LastName} - DoB set to: {employee.Birthday}";

            return result;

        }
    }
}
