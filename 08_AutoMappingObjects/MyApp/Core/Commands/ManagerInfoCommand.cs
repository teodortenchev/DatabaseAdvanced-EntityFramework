using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyApp.Core.Contracts;
using MyApp.Core.ViewModels;
using MyApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyApp.Core.Commands
{
    public class ManagerInfoCommand : IExecutable
    {
        private readonly CompanyContext context;
        private readonly Mapper mapper;

        public ManagerInfoCommand(CompanyContext context, Mapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public string Execute(string[] inputArgs)
        {
            int employeeId = int.Parse(inputArgs[0]);

            var manager = context.Employees.Include(m => m.ManagedEmployees).FirstOrDefault(x => x.Id == employeeId);

            if (manager == null)
            {
                throw new InvalidOperationException($"Can't find manager with id {employeeId}");
            }

            var managerDto = mapper.CreateMappedObject<ManagerDto>(manager);

            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"{managerDto.FirstName} {managerDto.LastName} | Emplyees: {managerDto.ManagedEmployees.Count}");

            foreach (EmployeeDto employee in managerDto.ManagedEmployees)
            {
                sb.AppendLine($"\t - {employee.FirstName} {employee.LastName} - ${employee.Salary:F2}");
            }



            return sb.ToString().TrimEnd();
        }
    }
}
