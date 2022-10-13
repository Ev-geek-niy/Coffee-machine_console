namespace Coffee_machine_console;

class Program
{
    static void Main(string[] args)
    {
        MachineAPI machine = new MachineAPI();
        machine.Run();
    }
}