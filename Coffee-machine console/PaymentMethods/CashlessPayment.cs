namespace Coffee_machine_console.PaymentMethods;

/// <summary>
/// Класс безналичной оплаты без реализации.
/// </summary>
public class CashlessPayment : Payment
{
    public void Pay()
    {
        Console.WriteLine("Приложите карту");
        Console.WriteLine("Обработка...");
        Console.WriteLine("Оплата успешно прошла");
    }
}