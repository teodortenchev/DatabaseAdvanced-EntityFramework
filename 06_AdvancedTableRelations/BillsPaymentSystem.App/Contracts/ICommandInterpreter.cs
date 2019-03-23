namespace BillsPaymentSystem.App.Contracts
{
    public interface ICommandInterpreter
    {
        void Interpret(string[] data);
    }
}
