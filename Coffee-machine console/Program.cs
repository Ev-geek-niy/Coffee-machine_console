using Coffee_machine_console.Factory;
using Coffee_machine_console.Resources;

namespace Coffee_machine_console;

class Program
{
    static void Main(string[] args)
    {
        MachineAPI machine = new MachineAPI();
        machine.Run();
    }
}