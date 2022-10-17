using Coffee_machine_console.Resources;

namespace Coffee_machine_console.PaymentMethods;

/// <summary>
/// Абстрактный класс типа типа оплаты.
/// </summary>
public abstract class Payment
{
    public int totalFunds;

    /// <summary>
    /// Добавляет денежные средства.
    /// </summary>
    /// <param name="value">количество средств.</param>
    public virtual void Add(int value)
    {
        this.totalFunds += value;
    }

    /// <summary>
    /// Выводит в консоль информацию о количестве денежных средств.
    /// </summary>
    public virtual void printFounds()
    {
        Console.WriteLine($"На счету: {this.totalFunds} рублей");
    }

    /// <summary>
    /// Произведение оплаты.
    /// Отнимает от внесенных средств стоимость заказа.
    /// </summary>
    /// <param name="order">Объект заказа</param>
    public virtual int Pay(Order order)
    {
        totalFunds -= order.drink.price;
        return totalFunds;
    }

    public bool CanPay(Order order)
    {
        return totalFunds - order.drink.price >= 0;
    }
}