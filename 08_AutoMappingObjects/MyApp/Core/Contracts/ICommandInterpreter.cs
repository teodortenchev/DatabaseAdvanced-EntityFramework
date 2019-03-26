namespace MyApp.Core.Contracts
{
    public interface ICommandInterpreter
    {
        string Interpret(string[] data);
    }
}
