using AutoMapper;
using MyApp.Core.Contracts;
using MyApp.Core.ViewModels;
using MyApp.Data;
using System;
using System.Linq;

namespace MyApp.Core.Commands
{
    public class SetAddressCommand : IExecutable
    {
        private readonly CompanyContext context;
        private readonly Mapper mapper;

        public SetAddressCommand(CompanyContext context, Mapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public string Execute(string[] inputArgs)
        {
            int id = int.Parse(inputArgs[0]);
            string address = String.Join(" ", inputArgs.Skip(1));


            var employee = context.Employees.Find(id);

            if (employee == null)
            {
                throw new InvalidOperationException($"Employee with Id {id} not found!");
            }

            if (employee.Address != null)
            {
                Console.WriteLine($"Overwriting address: {employee.Address}.");
            }

            employee.Address = address;

            context.SaveChanges();

            var employeeDto = this.mapper.CreateMappedObject<EmployeeDto>(employee);

            string result = $"Address for {employeeDto.FirstName} {employeeDto.LastName} set to {employee.Address}";

            return result;
        }
    }
}
