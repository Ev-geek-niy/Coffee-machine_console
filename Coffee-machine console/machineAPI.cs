using System.Runtime.CompilerServices;
using Coffee_machine_console.Wrappers;

//TODO: Переписать все ифы на throw Exception;

namespace Coffee_machine_console;

public static class machineAPI
{
    private static readonly int[] _currencyValues = new[] { 1, 2, 5, 10, 50, 100, 200 };
    private static readonly string[] _resourceNames = new[] { "cup", "coffee", "water", "milk", "sugar" };
    private static readonly DB _db = new DB();

    private static int _money = 0;
    private static int _choosedCoffee = 0;
    private static string _order = "";

    private static readonly Dictionary<string, string[]> _resourceTypes = new Dictionary<string, string[]>()
    {
        ["литров"] = new []{"milk", "water"},
        ["кг"] = new []{"coffee", "sugar"},
        ["штук"] = new []{"cup"},
    };
    private static Dictionary<string, int> _supplements = new Dictionary<string, int>()
    {
        ["milk"] = 0,
        ["sugar"] = 0,
    };

    public static void CreateOrder(int drinkNumber)
    {
        string coffeeName = _db.GetDrink(drinkNumber)["drink_name"].ToString();
        _order = $"{coffeeName}";
    }

    public static void AddToOrder()
    {
        CreateOrder(_choosedCoffee);
        foreach (KeyValuePair<string, int> pair in _supplements)
        {
            if (pair.Key == "milk" && pair.Value != 0)
                _order += $" плюс {pair.Value / 100} молоко";
            if (pair.Key == "sugar" && pair.Value != 0)
                _order += $" плюс {pair.Value / 100} сахар";
        }
    }

    public static string GetResType(string resourceName)
    {
        foreach (var resourceType in _resourceTypes)
        {
            if (resourceType.Value.Contains(resourceName))
                return resourceType.Key;
        }

        return "";
    }

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

    private static string GetCommand(string[] str)
    {
        return str[0];
    }

    private static string[] GetArgs(string[] str)
    {
        return str.Skip(1).ToArray();
    }

    private static void Help()
    {
        Console.WriteLine("CoffeeList\nAddMoney [money]\nChooseCoffee [coffee number]\n" +
                          "AddInCoffee [sugar/milk] [count]\nExecuteOrder\nResourceAmount [resource name]\n" +
                          "FillResource [resource name] [value]");
    }

    private static void CoffeeList()
    {
        var list = _db.GetAllDrinks();
        foreach (var item in list)
        {
            Console.WriteLine($"{item.Value[0]} - {item.Value[1]}, цена {item.Value[2]} рублей");
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
                CreateOrder(_choosedCoffee);
                Console.WriteLine($"Ваш заказ: {_order}");
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

    //TODO: переписать на throw
    private static void AddInCoffee(params string[] args)
    {
        try
        {
            if (_choosedCoffee <= 0)
                throw new Exception("Не выбран кофе");
            
            string supply = args[0];
            if (!_supplements.ContainsKey(supply))
                throw new Exception("Добавки с таким именем не сущесвует, сейчас доступны: ");
            
            int.TryParse(args[1], out int value);
            if (value <= 0)
                throw new Exception("Нельзя добавить нулевое или отрицательное значение");
            
            int tempValue = value * 100 + _supplements[supply];
            int supplyValue = _db.GetResource(supply);
            
            
            if (supplyValue - tempValue < 0)
                throw new Exception($"{supply} недостаточно");
            
            
            _supplements[supply] = tempValue;
            AddToOrder();
            Console.WriteLine($"Ваш заказ: {_order}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private static void ExecuteOrder(int coffeeType, Dictionary<string, int> supplyment, int money)
    {
        try
        {
            if (coffeeType == 0)
                throw new Exception("Не выбран кофе");
            
            int bill = money - (int)_db.GetDrink(coffeeType)["drink_price"];
            if (bill < 0)
                throw new Exception($"Не хватает {-bill} денек");
            
            if (!_db.CheckResources(coffeeType, supplyment))
                throw new Exception("Не хватает ресов");

            _db.ExecuteOrder(coffeeType, supplyment, money);
            _money = bill;
            Console.WriteLine($"Выдано: {_order}");
            Console.WriteLine($"На счету: {_money} рублей");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private static void AddMoney(string[] args)
    {
        int.TryParse(args[0], out var money);
        if (_currencyValues.Contains(money))
        {
            _money += money;
            GetBill();
        }
        else
        {
            Console.WriteLine("Валюты такого номинала не существует");
        }
    }

    private static void GetBill()
    {
        Console.WriteLine($"На счету: {_money} рублей");
    }

    private static void ResourceAmount(params string[] args)
    {
        string resourceName = args[0];
        try
        {
            if (!_resourceNames.Contains(resourceName))
                throw new Exception("Такой ресурса нет в базе");

            int value = _db.GetResource(resourceName);
            string resType = GetResType(resourceName);
            double convertedValue = resourceName == "cup" ? value : value / 1000.0;
            if (convertedValue != 0)
                Console.WriteLine($"Количество {resourceName}: {convertedValue} {resType}");
            else
                Console.WriteLine($"{resourceName} закончился");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private static void FillResource(string[] args)
    {
        string resourceName = args[0];
        int.TryParse(args[1], out int value);
        try
        {
            if (!_resourceNames.Contains(resourceName))
                throw new Exception("Такого ресурса нет в таблице");

            if (value <= 0)
                throw new Exception("Нельзя восполнить 0 или отрицательное значение");

            _db.FillResource(resourceName, value);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}