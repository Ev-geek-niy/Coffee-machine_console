using Coffee_machine_console.Factory;
using Coffee_machine_console.PaymentMethods;
using Coffee_machine_console.Resources;

namespace Coffee_machine_console;

class MachineAPI
{
    private DB _db;
    private Order order;
    private PaymentCreator _paymentCreator;
    private ResourceFactory _resourceFactory;
    private Payment _paymentMethod;

    public MachineAPI()
    {
        this._db = new DB();
        this._paymentCreator = new PaymentCreator();
        this._resourceFactory = new ResourceFactory();
        this._paymentMethod = this._paymentCreator.CreatePaymentMethod("cash");
    }

    /// <summary>
    /// Запуск приложения.
    /// </summary>
    public void Run()
    {
        Console.WriteLine("Добро пожаловать!\nВведите help для показа всех команд\nВведите q, чтобы выйти");
        CommandListener();
        Console.WriteLine("Завершение работы");
    }

    /// <summary>
    /// Принимает строки пользователя и определяет какую команду необходимо выполнить.
    /// </summary>
    private void CommandListener()
    {
        Console.CancelKeyPress += CloseMachine;
        while (true)
        {
            string[] fullCommand = Console.ReadLine().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string command = GetCommand(fullCommand);
            string[] args = GetArgs(fullCommand);

            switch (command)
            {
                case "CoffeeList":
                    CoffeeList();
                    break;
                case "ChooseCoffee":
                    SelectDrink(args);
                    break;
                case "AddInCoffee":
                    AddInCoffee(args);
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
                case "ExecuteOrder":
                    ExecuteOrder(order);
                    break;
                default:
                    Console.WriteLine("Такой команды не существует");
                    break;
            }
        }
    }

    /// <summary>
    /// Отлавливает выход из приложения
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CloseMachine(object sender, ConsoleCancelEventArgs e)
    {
        Console.WriteLine("Закрытие приложения");
        Environment.Exit(-1);
    }

    /// <summary>
    /// Получить команду из строки.
    /// </summary>
    /// <param name="str">Массив строк введенных пользователем.</param>
    /// <returns>Первый элемент массива, так как он является командой.</returns>
    private string GetCommand(string[] str)
    {
        return str[0];
    }

    /// <summary>
    /// Получить аргументы из строки.
    /// Первый элемент пропускается, так как он является командой.
    /// </summary>
    /// <param name="str">Массив строк введенных пользователем.</param>
    /// <returns>Массив строк без первого элемента.</returns>
    private string[] GetArgs(string[] str)
    {
        return str.Skip(1).ToArray();
    }

    /// <summary>
    /// Вывод всех напитков из базы данных.
    /// </summary>
    private void CoffeeList()
    {
        List<Drink> drinks = _db.GetAllDrinks();
        foreach (var drink in drinks)
            drink.printInfo();
    }

    /// <summary>
    /// Выбор напитка и вывод заказа в консоль.
    /// </summary>
    /// <param name="args">Id напитка</param>
    private void SelectDrink(params string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Не хватает аргументов: ID напитка");
            return;
        }
        
        int.TryParse(args[0], out int id);
        Drink drink = _db.GetDrink(id);

        if (drink is null)
        {
            Console.WriteLine("Напитка с таким ID не существует");
            return;
        }
        
        this.order = new Order(drink);
        order.PrintOrder();
    }

    /// <summary>
    /// Выводит количество ресурса из БД.
    /// </summary>
    /// <param name="args">Название ресурса.</param>
    public void ResourceAmount(params string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Не хватает аргументов: название ресурса");
            return;
        }
        
        string title = args[0];
        int resValue = _db.GetResourceValue(title);

        if (resValue == -1)
        {
            Console.WriteLine("Такого ресурса не существует");
            return;
        }
        
        Console.WriteLine(resValue);
    }

    /// <summary>
    /// Заполнить определенный ресурс в базе данных на введенное значение.
    /// </summary>
    /// <param name="args">
    /// Первый элемент массива - название ресурса.
    /// Второй элемент массива - Количество ресурса.</param>
    public void FillResource(params string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Не хватает аргументов: название и количество ресурса");
            return;
        }
        
        string title = args[0];
        int.TryParse(args[1], out int value);
        
        if (value <= 0)
        {
            Console.WriteLine("Количество не может быть отрицательным или равно нулю");
            return;
        }

        //Получаем количество ресурса из БД.
        int resDBValue = _db.GetResourceValue(title);
        //Создаем объект ресурса, где значение будет равно:
        //значение из БД + аргумент из строки.
        Resource resource = _resourceFactory.CreateResource(title, value + resDBValue);
        int affected = _db.FillResource(resource);
        
        if (affected == 0)
            Console.WriteLine("Такого ресурса нет или что-то пошло не так");
        else
            Console.WriteLine($"{title} успешно восполнен");
    }

    /// <summary>
    /// Добавляет определенный тип ресурса в добавки (Addition).
    /// </summary>
    /// <param name="args">
    /// Первый элемент массива - название ресурса.
    /// Второй элемент массива - количество ресурса.
    /// </param>
    public void AddInCoffee(params string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Не хватает аргументов: название и количество ресурса");
            return;
        }
        
        string resourceType = args[0];
        int.TryParse(args[1], out int value);

        if (value <= 0)
        {
            Console.WriteLine("Количество не может быть отрицательным или равно нулю");
            return;
        }
        
        order.AddToOrder(resourceType, value);
        order.PrintOrder();
    }

    /// <summary>
    /// Добавить определенное количество средств.
    /// </summary>
    /// <param name="args">Количество средств.</param>
    public void AddMoney(params  string[] args)
    {
        int.TryParse(args[0], out int value);
        this._paymentMethod.Add(value);
        this._paymentMethod.printFounds();
    }

    /// <summary>
    /// Выполнение заказа.
    /// После выполнения заказа выводит в консоль заказ и остаток на счету.
    /// </summary>
    /// <param name="order">Объект заказа.</param>
    public void ExecuteOrder(Order order)
    {
        //TODO: очень плохо, переписать.
        int[] orderRaw = order.createRawData();
        int[] dbRaw = new[]
        {
            _db.GetResourceValue("cup"),
            _db.GetResourceValue("coffee"),
            _db.GetResourceValue("water"),
            _db.GetResourceValue("milk"),
            _db.GetResourceValue("sugar"),
        };

        for (int i = 0; i < dbRaw.Length; i++)
        {
            dbRaw[i] -= orderRaw[i];
            if (dbRaw[i] < 0)
            {
                Console.WriteLine("Не хватает ресурсов");
                return;
            }
        }

        if (!_paymentMethod.CanPay(order))
        {
            Console.WriteLine("Недостаточно средств");
            return;
        };

        _paymentMethod.Pay(order);
        _db.ExecuteOrder(dbRaw);
        _db.CreateLog(order);
        
        Console.WriteLine("Заказ успешно выполнен");
        order.PrintOrder();
        _paymentMethod.printFounds();
    }
}

