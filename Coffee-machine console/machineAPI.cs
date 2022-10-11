using System.Runtime.CompilerServices;

//TODO: Переписать все ифы на throw Exception;

namespace Coffee_machine_console;
public static class machineAPI
{
    private static DB _db = new DB();
    private static int _money = 0;
    private static int _choosedCoffee;
    private static string _order = "";

    private static Dictionary<string, int> _supplements = new Dictionary<string, int>()
    {
        ["milk"] = 1,
        ["sugar"] = 0,
    };

    public static void CreateOrder(string coffeeName)
    {
        if (_choosedCoffee <= 0)
            Console.WriteLine("Пожалуйста, выберите напиток\n");
        _order = $"{coffeeName}";
    }

    public static void AddToOrder()
    {
        foreach (KeyValuePair<string,int> pair in _supplements)
        {
            if (pair.Key == "milk" && pair.Value != 0)
                _order += $" плюс {pair.Value} молоко";
            if (pair.Key == "sugar" && pair.Value != 0)
                _order += $" плюс {pair.Value} сахар";
        }
    }

    public static void Run()
    {
        ExecuteOrder(1, _supplements, _money);
        // Console.WriteLine("Добро пожаловать!\nВведите help для показа всех команд\nВведите q, чтобы выйти");
        // CommandListener();
        // Console.WriteLine("Завершение работы");
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
                    ExecuteOrder(_choosedCoffee, _supplements, _money);
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

    public static void GetSupplyKeys()
    {
        foreach (var x in _supplements)
        {
            Console.WriteLine(x.Key);
        }
    }
    
    public static void GetSupply()
    {
        foreach (var x in _supplements)
        {
            Console.WriteLine($"{x.Key} - {x.Value}");
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
        var list = _db.GetAllDrinks();
        foreach (var item in list)
        {
            Console.WriteLine($"{item.Value[0]} - {item.Value[1]}, цена {item.Value[2]}");
        }
    }

    private static void ChooseCoffee(params string[] args)
    {
        try
        {
            int.TryParse(args[0], out int choice);
            int count = _db.GetDrinkCount();
            if (choice <= 0 || choice > count)
                Console.WriteLine(
                    "Выбранного напитка не существует\nПосмотреть список напитков можно по команде CoffeeList");
            else
            {
                _choosedCoffee = choice;
                string coffeeName = _db.GetDrink(_choosedCoffee)["drink_name"].ToString();
                CreateOrder(coffeeName);
                Console.WriteLine($"Ваш заказ: _order");
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

    private static void AddInCoffee(params string[] args)
    {
        try
        {
            string supply = args[0];
            int.TryParse(args[1], out int value);
            if (value <= 0)
            {
                Console.WriteLine("Количество внесенных средств не может быть меньше нуля или равно нулю");
            }
            else if (!_supplements.ContainsKey(supply))
            {
                Console.WriteLine("Добавки с таким именем не сущесвует, сейчас доступны: ");
                GetSupplyKeys();
            }
            else
            {
                _supplements[supply] = value;
            }
            AddToOrder();
            Console.WriteLine($"Ваш заказ: {_order}");
        }
        catch (IndexOutOfRangeException e)
        {
            Console.WriteLine("Не указаны параметры для добавок");
        }
    }

    private static void ExecuteOrder(int coffeeType, Dictionary<string, int> supplyment, int money)
    {
        int change = _db.ExecuteOrder(coffeeType, supplyment, money);
        if (change >= 0)
        {
            Console.WriteLine($"Выдал: {_order}");
            Console.WriteLine($"На счету: {change} рублей");
        }
        else
        {
            Console.WriteLine($"Не хватает {-change} рублей");
        }
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