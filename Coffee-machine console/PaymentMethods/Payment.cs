namespace Coffee_machine_console.PaymentMethods;

/// <summary>
/// Абстрактный класс типа типа оплаты.
/// </summary>
public abstract class Payment
{
    protected int totalFunds;

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
}