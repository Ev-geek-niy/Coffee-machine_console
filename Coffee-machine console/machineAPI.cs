namespace Coffee_machine_console;

public static class machineAPI
{
    //Константсы
    //Экземляр БД
    private static readonly DB _db = new DB();

    //Доступные номиналы валют
    private static readonly int[] _currencyValues = new[] { 1, 2, 5, 10, 50, 100, 200 };

    //Доступные ресурсы
    private static readonly string[] _resourceNames = new[] { "cup", "coffee", "water", "milk", "sugar" };

    //Определение в чем исчисляется ресурс
    private static readonly Dictionary<string, string[]> _resourceTypes = new Dictionary<string, string[]>()
    {
        ["литров"] = new[] { "milk", "water" },
        ["кг"] = new[] { "coffee", "sugar" },
        ["штук"] = new[] { "cup" },
    };

    //Общее количество денег
    private static int _money = 0;

    //ID выбранного напитка
    private static int _choosedCoffee = 0;

    //Строка заказа
    private static string _order = "";

    //Количество добавок
    private static Dictionary<string, int> _supplements = new Dictionary<string, int>()
    {
        ["milk"] = 0,
        ["sugar"] = 0,
    };

    //Принимает id напитка и формирует строку заказа, подставляя название из БД
    private static void CreateOrder(int drinkNumber)
    {
        string coffeeName = _db.GetDrink(drinkNumber)["drink_name"].ToString();
        _order = $"{coffeeName}";
    }

    //Добавление добавки для напитка
    //Проходит по словарю _supplements и добавляет к строке количество и тип добавки
    private static void AddToOrder()
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

    //Получение в котором исчисляется тип ресурса
    private static string GetResType(string resourceName)
    {
        foreach (KeyValuePair<string, string[]> resourceType in _resourceTypes)
        {
            if (resourceType.Value.Contains(resourceName))
                return resourceType.Key;
        }

        return "";
    }

    //Запуск приложения
    public static void Run()
    {
        Console.WriteLine("Добро пожаловать!\nВведите help для показа всех команд\nВведите q, чтобы выйти");
        CommandListener();
        Console.WriteLine("Завершение работы");
    }

    //Принимает строки пользователя и определяет какую команду необходимо выполнить
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

    //Возвращает команду, равную первому слову из введенной строки
    private static string GetCommand(string[] str)
    {
        return str[0];
    }

    //Возвращает массив аргументов, введенных после команды
    private static string[] GetArgs(string[] str)
    {
        return str.Skip(1).ToArray();
    }

    //Вывод всех доступных команд с необходимыми аргументами
    private static void Help()
    {
        Console.WriteLine("CoffeeList\nAddMoney [money]\nChooseCoffee [coffee number]\n" +
                          "AddInCoffee [sugar/milk] [count]\nExecuteOrder\nResourceAmount [resource name]\n" +
                          "FillResource [resource name] [value]");
    }

    //Вывод всех напитков из БД
    private static void CoffeeList()
    {
        var allDrinks = _db.GetAllDrinks();
        foreach (var item in allDrinks)
        {
            Console.WriteLine($"{item.Value[0]} - {item.Value[1]}, цена {item.Value[2]} рублей");
        }
    }

    //Выбор кофе
    //Аргументы: id напитка
    private static void ChooseCoffee(params string[] args)
    {
        try
        {
            int.TryParse(args[0], out int choice);
            int count = _db.GetDrinkCount();

            //Проверка диапазона введенного id
            if (choice <= 0 || choice > count)
                throw new Exception(
                    "Выбранного напитка не существует\nПосмотреть список напитков можно по команде CoffeeList");

            //Запись id в свойство
            _choosedCoffee = choice;
            //Генерация строки заказа
            CreateOrder(_choosedCoffee);

            Console.WriteLine($"Ваш заказ: {_order}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    //Добавка к кофе
    //Аргументы: название | количество
    private static void AddInCoffee(params string[] args)
    {
        try
        {
            //Проверка на наличие выбранного кофе
            if (_choosedCoffee <= 0)
                throw new Exception("Не выбран кофе");

            //Получение названия добавки
            string supply = args[0];

            //Проверка на наличие в словаре _supplements
            if (!_supplements.ContainsKey(supply))
                throw new Exception("Добавки с таким именем не сущесвует, сейчас доступны: ");

            //Получение количества добавки
            //Если вместо числа будет введено слово value = 0
            int.TryParse(args[1], out int value);

            //Проверка на нулевые и отрицательные значения
            if (value <= 0)
                throw new Exception("Нельзя добавить нулевое или отрицательное значение");

            //Получения итогового количества добавки
            //Получение доступного количества из БД
            int tempValue = value * 100 + _supplements[supply];
            int supplyValue = _db.GetResource(supply);

            //Проверка на доступность количества добавок
            if (supplyValue - tempValue < 0)
                throw new Exception($"{supply} недостаточно");

            //Запись количества в словарь _supplements по выбранной добавке
            _supplements[supply] = tempValue;

            //Пересоздание строки заказа с обновленными параметрами добавок
            AddToOrder();

            Console.WriteLine($"Ваш заказ: {_order}");
        }
        catch (IndexOutOfRangeException e)
        {
            Console.WriteLine("Недостаточно аргументов");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    //Выполнение заказа и выдача информации по заказу и счету
    private static void ExecuteOrder(int coffeeType, Dictionary<string, int> supplyment, int money)
    {
        try
        {
            //Проверка на наличие выбранного кофе 
            if (coffeeType == 0)
                throw new Exception("Не выбран кофе");

            //Получение будущей сдачи при выполнении заказа
            int bill = money - (int)_db.GetDrink(coffeeType)["drink_price"];

            //Проверка, что сдача не будет отрицательной
            if (bill < 0)
                throw new Exception($"Не хватает {-bill} денек");

            //Проверка, что ресурсов из БД хватит для изготовления заказа
            if (!_db.CheckResources(coffeeType, supplyment))
                throw new Exception("Не хватает ресов");

            _db.ExecuteOrder(coffeeType, supplyment);

            //Запись сдачи в доступные средства
            _money = bill;
            Console.WriteLine($"Выдано: {_order}");
            Console.WriteLine($"На счету: {_money} рублей");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    //Добавление средств и вывод информации о счете
    //Аргументы: номинал валюты
    private static void AddMoney(string[] args)
    {
        try
        {
            //Получение количества внесенных средств
            int.TryParse(args[0], out var money);

            //Проверка, что номинал существует
            if (!_currencyValues.Contains(money))
                throw new Exception("Валюты такого номинала не существует");

            //Добавление средств в свойство
            _money += money;
            Console.WriteLine($"На счету: {_money} рублей");
        }
        catch (IndexOutOfRangeException e)
        {
            Console.WriteLine("Недостаточно аргументов");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    //Вывод доступного количества ресурса
    //Аргументы: название ресурса
    private static void ResourceAmount(params string[] args)
    {
        try
        {
            //Получение названия ресурса
            string resourceName = args[0];
            
            //Проверка на существования ресурса
            if (!_resourceNames.Contains(resourceName))
                throw new Exception("Такой ресурса нет в базе");

            //Получение количества ресурса из базы данных
            int value = _db.GetResource(resourceName);
            
            //Определения в чем исчисляется ресурс
            string resType = GetResType(resourceName);
            
            //Конвертирование значения
            double convertedValue = resourceName == "cup" ? value : value / 1000.0;
            
            //Проверка что ресурс не нулевой
            if (convertedValue == 0)
                throw new Exception($"{resourceName} закончился");
            
            Console.WriteLine($"Количество {resourceName}: {convertedValue} {resType}");
        }
        catch (IndexOutOfRangeException e)
        {
            Console.WriteLine("Недостаточно аргументов");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    //Добавление определенного количества выбранному ресурсу
    //Аргументы: название ресурса | количество
    private static void FillResource(string[] args)
    {
        try
        {
            //Получение названия ресурса
            string resourceName = args[0];
            //Получение количества, если нет = 0
            int.TryParse(args[1], out int value);
            
            //Проверка на сущестсвование ресурса
            if (!_resourceNames.Contains(resourceName))
                throw new Exception("Такого ресурса нет в таблице");

            //Проверка, что значение не нулевое или отрицательное
            if (value <= 0)
                throw new Exception("Нельзя восполнить 0 или отрицательное значение");

            _db.FillResource(resourceName, value);
        }
        catch (IndexOutOfRangeException e)
        {
            Console.WriteLine("Недостаточно аргументов");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}