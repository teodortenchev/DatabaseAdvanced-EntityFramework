using AutoMapper;
using MyApp.Core.Contracts;
using MyApp.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.Core.Commands
{
    public class SetManagerCommand : IExecutable
    {
        private readonly CompanyContext context;
        private readonly Mapper mapper;

        public SetManagerCommand(CompanyContext context, Mapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public string Execute(string[] inputArgs)
        {
            int employeeId = int.Parse(inputArgs[0]);
            int managerId = int.Parse(inputArgs[1]);

            var employee = context.Employees.Find(employeeId);
            var manager = context.Employees.Find(managerId);

            if (employee == null || manager == null)
            {
                throw new InvalidOperationException("Couldn't find manager or employee with these ids.");
            }

            employee.Manager = manager;

            context.SaveChanges();

            return "Successfully added a new manager for emlpoyee with Id " + employeeId; 
        }
    }
}
