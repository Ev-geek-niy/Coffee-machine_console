using System.Text;

namespace Coffee_machine_console;

public class MachineAPI
{
    //Константы
    //Экземляр БД
    private readonly DB _db = new DB();

    //Доступные номиналы валют
    private readonly int[] _currencyValues = { 1, 2, 5, 10, 50, 100, 200 };

    //Доступные ресурсы
    private readonly string[] _resourceNames = { "cup", "coffee", "water", "milk", "sugar" };

    //Определение в чем исчисляется ресурс
    private readonly Dictionary<string, string[]> _resourceTypes = new Dictionary<string, string[]>()
    {
        ["литров"] = new[] { "milk", "water" },
        ["кг"] = new[] { "coffee", "sugar" },
        ["штук"] = new[] { "cup" },
    };

    //Общее количество денег
    private int _money = 0;

    //ID выбранного напитка
    private int _choosedCoffee = 0;

    //Строка заказа
    private StringBuilder _order = new StringBuilder();

    //Количество добавок
    private Dictionary<string, int> _supplements = new Dictionary<string, int>()
    {
        ["milk"] = 0,
        ["sugar"] = 0,
    };
    
    //Запуск приложения
    public void Run()
    {
        Console.WriteLine("Добро пожаловать!\nВведите help для показа всех команд\nВведите q, чтобы выйти");
        CommandListener();
        Console.WriteLine("Завершение работы");
    }

    //Принимает строки пользователя и определяет какую команду необходимо выполнить
    private bool CommandListener()
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
                    Console.WriteLine("Такой команды не существует");
                    break;
            }
        }
    }

    //Возвращает команду, равную первому слову из введенной строки
    private string GetCommand(string[] str)
    {
        return str[0];
    }

    //Возвращает массив аргументов, введенных после команды
    private string[] GetArgs(string[] str)
    {
        return str.Skip(1).ToArray();
    }
    
    //Принимает id напитка и формирует строку заказа, подставляя название из БД
    private void CreateOrder(int drinkNumber)
    {
        string coffeeName = _db.GetDrink(drinkNumber)["drink_name"].ToString();
        _order.Remove(0, _order.Length);
        _order.Append($"{coffeeName}");
    }

    //Добавление добавки для напитка
    //Проходит по словарю _supplements и добавляет к строке количество и тип добавки
    private void AddToOrder()
    {
        CreateOrder(_choosedCoffee);
        foreach (KeyValuePair<string, int> pair in _supplements)
        {
            if (pair.Key == "milk" && pair.Value != 0)
                _order.Append($" плюс {pair.Value / 100} молоко");
            if (pair.Key == "sugar" && pair.Value != 0)
                _order.Append($" плюс {pair.Value / 100} сахар");
        }
    }

    //Получение в котором исчисляется тип ресурса
    private string GetResType(string resourceName)
    {
        foreach (KeyValuePair<string, string[]> resourceType in _resourceTypes)
        {
            if (resourceType.Value.Contains(resourceName))
                return resourceType.Key;
        }

        return "";
    }

    //Вывод всех доступных команд с необходимыми аргументами
    private void Help()
    {
        Console.WriteLine("CoffeeList\nAddMoney [money]\nChooseCoffee [coffee number]\n" +
                          "AddInCoffee [sugar/milk] [count]\nExecuteOrder\nResourceAmount [resource name]\n" +
                          "FillResource [resource name] [value]");
    }

    //Вывод всех напитков из БД
    private void CoffeeList()
    {
        var allDrinks = _db.GetAllDrinks();
        foreach (var item in allDrinks)
        {
            Console.WriteLine($"{item.Value[0]} - {item.Value[1]}, цена {item.Value[2]} рублей");
        }
    }

    //Выбор кофе
    //Аргументы: id напитка
    private void ChooseCoffee(params string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Нет аргументов");
            return;
        }
        
        int.TryParse(args[0], out int choice);
        int count = _db.GetDrinkCount();

        //Проверка диапазона введенного id
        if (choice <= 0 || choice > count)
        {
            Console.WriteLine(
                "Выбранного напитка не существует\nПосмотреть список напитков можно по команде CoffeeList");
            return;
        }

        //Запись id в свойство
        _choosedCoffee = choice;
        //Генерация строки заказа
        CreateOrder(_choosedCoffee);

        Console.WriteLine($"Ваш заказ: {_order}");
    }

    //Добавка к кофе
    //Аргументы: название | количество
    private void AddInCoffee(params string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Нет аргументов");
            return;
        }
        
        //Проверка на наличие выбранного кофе
        if (_choosedCoffee <= 0)
        {
            Console.WriteLine("Не выбран кофе");
            return;
        }

        //Получение названия добавки
        string supply = args[0];

        //Проверка на наличие в словаре _supplements
        if (!_supplements.ContainsKey(supply))
        {
            Console.WriteLine("Добавки с таким именем не сущесвует, сейчас доступны: ");
            return;
        }

        //Получение количества добавки
        //Если вместо числа будет введено слово value = 0
        int.TryParse(args[1], out int value);

        //Проверка на нулевые и отрицательные значения
        if (value <= 0)
        {
            Console.WriteLine("Нельзя добавить нулевое или отрицательное значение");
            return;
        }

        //Получения итогового количества добавки
        //Получение доступного количества из БД
        int tempValue = value * 100 + _supplements[supply];
        int supplyValue = _db.GetResource(supply);

        //Проверка на доступность количества добавок
        if (supplyValue - tempValue < 0)
        {
            Console.WriteLine($"{supply} недостаточно");
            return;
        }

        //Запись количества в словарь _supplements по выбранной добавке
        _supplements[supply] = tempValue;

        //Пересоздание строки заказа с обновленными параметрами добавок
        AddToOrder();

        Console.WriteLine($"Ваш заказ: {_order}");
    }

    //Выполнение заказа и выдача информации по заказу и счету
    private void ExecuteOrder(int coffeeType, Dictionary<string, int> supplyment, int money)
    {
        //Проверка на наличие выбранного кофе 
        if (coffeeType == 0)
        {
            Console.WriteLine("Не выбран кофе");
            return;
        }

        //Получение будущей сдачи при выполнении заказа
        int bill = money - (int)_db.GetDrink(coffeeType)["drink_price"];

        //Проверка, что сдача не будет отрицательной
        if (bill < 0)
        {
            Console.WriteLine($"Не хватает на счету: {Math.Abs(bill)} рублей");
            return;
        }

        //Проверка, что ресурсов из БД хватит для изготовления заказа
        if (!_db.CheckResources(coffeeType, supplyment))
        {
            Console.WriteLine("Не хватает ресурсов");
            return;
        }

        _db.ExecuteOrder(coffeeType, supplyment);

        //Запись сдачи в доступные средства
        _money = bill;
        Console.WriteLine($"Выдано: {_order}");
        Console.WriteLine($"На счету: {_money} рублей");
    }

    //Добавление средств и вывод информации о счете
    //Аргументы: номинал валюты
    private void AddMoney(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Нет аргументов");
            return;
        }
        
        //Получение количества внесенных средств
        int.TryParse(args[0], out var money);

        //Проверка, что номинал существует
        if (!_currencyValues.Contains(money))
        {
            Console.WriteLine("Валюты такого номинала не существует");
            return;
        }

        //Добавление средств в свойство
        _money += money;
        Console.WriteLine($"На счету: {_money} рублей");
    }

    //Вывод доступного количества ресурса
    //Аргументы: название ресурса
    private void ResourceAmount(params string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Нет аргументов");
            return;
        }
        
        //Получение названия ресурса
        string resourceName = args[0];
        
        //Проверка на существования ресурса
        if (!_resourceNames.Contains(resourceName))
        {
            Console.WriteLine("Такой ресурса нет в базе");
            return;
        }

        //Получение количества ресурса из базы данных
        int value = _db.GetResource(resourceName);
        
        //Определения в чем исчисляется ресурс
        string resType = GetResType(resourceName);
        
        //Конвертирование значения
        double convertedValue = resourceName == "cup" ? value : value / 1000.0;
        
        //Проверка что ресурс не нулевой
        if (convertedValue == 0)
        {
            Console.WriteLine($"{resourceName} закончился");
            return;
        }
        
        Console.WriteLine($"Количество {resourceName}: {convertedValue} {resType}");
    }

    //Добавление определенного количества выбранному ресурсу
    //Аргументы: название ресурса | количество
    private void FillResource(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Нет аргументов");
            return;
        }
        
        //Получение названия ресурса
        string resourceName = args[0];
        //Получение количества, если нет = 0
        int.TryParse(args[1], out int value);
        
        //Проверка на сущестсвование ресурса
        if (!_resourceNames.Contains(resourceName))
        {
            Console.WriteLine("Такого ресурса нет в таблице");
            return;
        }

        //Проверка, что значение не нулевое или отрицательное
        if (value <= 0)
        {
            Console.WriteLine("Нельзя восполнить 0 или отрицательное значение");
            return;
        }

        _db.FillResource(resourceName, value);
    }
}