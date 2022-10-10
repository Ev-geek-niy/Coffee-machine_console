using System.Runtime.CompilerServices;

namespace Coffee_machine_console;

public static class machineAPI
{
    private static DB _db = new DB();
    private static int _money = 0;
    private static int _choosedCoffee;

    public static void Run()
    {
        Console.WriteLine("Добро пожаловать!\nВведите help для показа всех команд\nВведите q, чтобы выйти");
        CommandListener();
        Console.WriteLine("Завершение работы");
    }

    private static bool CommandListener()
    {
        while (true)
        {
            string[] fullCommand = Console.ReadLine().Split(' ');
            string command = GetCommand(fullCommand);
            string[] args = GetArgs(fullCommand);

            switch (command)
            {
                case "help":
                    Help();
                    break;
                case "CoffeeList":
                    CoffeeList();
                    break;
                case "ChooseCoffee":
                    ChooseCoffee(args);
                    break;
                case "AddInCoffee":
                    AddInCoffee(args);
                    break;
                case "ExecuteOrder":
                    ExecuteOrder();
                    break;
                case "AddMoney":
                    AddMoney(args);
                    break;
                case "ResourceAmount":
                    ResourceAmount(args);
                    break;
                case "FillResource":
                    FillResource(args);
                    break;
                case "q":
                    return false;
                default:
                    Console.WriteLine("Такой команды не сущесвует");
                    break;
                
            }
        }
    }

    private static string GetCommand(string[] str)
    {
        return str[0];
    }

    private static string[] GetArgs(string[] str)
    {
        return str.Skip(1).ToArray();
    }

    private static void DegubCommand(string command, string[] args)
    {
        Console.WriteLine(command);
        if (args.Length > 0)
        {
            foreach (string arg in args)
            {
                Console.Write(arg + " ");
            }
            Console.WriteLine();
        }
        else
            Console.WriteLine("No args");
    }

    private static void Help()
    {
        Console.WriteLine("CoffeeList\nAddMoney [money]\nChooseCoffee [coffee number]\n" +
                          "AddInCoffee [sugar/milk] [count]\nExecuteOrder\nResourceAmount [resource name]\n" +
                          "FillResource [resource name] [value]\n");
    }

    private static void CoffeeList()
    {
        _db.getAllDrinks();
    }

    private static void ChooseCoffee(params string[] args)
    {
        try
        {
            int.TryParse(args[0], out int choice);
            int count = _db.getDrinkCount();
            if (choice <= 0 || choice > count)
                Console.WriteLine(
                    "Выбранного напитка не существует\nПосмотреть список напитков можно по команде CoffeeList");
            else
            {
                _choosedCoffee = choice;
                Console.WriteLine("Вы выбрали кофе под номером " + _choosedCoffee);
            }
        }
        catch (IndexOutOfRangeException e)
        {
            Console.WriteLine("Не указан номер напитка\nПосмотреть список напитков можно по команде CoffeeList");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private static void AddInCoffee(string[] args)
    {
        
    }

    private static void ExecuteOrder()
    {
        
    }

    private static void AddMoney(string[] args)
    {
        int.TryParse(args[0], out var money);
        _money += money;
        GetBill();
    }

    private static void GetBill()
    {
        Console.WriteLine($"На счету: {_money} рублей");
    }

    private static void ResourceAmount(string[] args)
    {
        
    }

    private static void FillResource(string[] args)
    {
        
    }
}