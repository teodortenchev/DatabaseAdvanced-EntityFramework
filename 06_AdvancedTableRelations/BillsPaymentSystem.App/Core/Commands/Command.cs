using BillsPaymentSystem.App.Contracts;
using BillsPaymentSystem.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace BillsPaymentSystem.App.Core.Commands
{
    public abstract class Command : IExecutable
    {
        private readonly string[] data;

        public Command(string[] data, BillsPaymentSystemContext context)
        {
            this.data = data;
            Context = context;
        }

        public IReadOnlyList<string> Data => Array.AsReadOnly(data);
        public BillsPaymentSystemContext Context { get; }

        public abstract void Execute();
    }
}
