namespace Coffee_machine_console.PaymentMethods;

/// <summary>
/// Класс оплаты наличными.
/// </summary>
public class CashPayment : Payment
{
    // доступные номиналы валюты.
    private readonly int[] values = new[] { 1, 2, 5, 10, 50, 100, 200 };

    public CashPayment()
    {
        this.totalFunds = 0;
    }

    /// <summary>
    /// Добавляет денежные средства.
    /// Перед добавлением происводится проверка существование такого номинала.
    /// </summary>
    /// <param name="value">Количество денежных сроедств.</param>
    public override void Add(int value)
    {
        if (CheckValue(value))
            this.totalFunds += value;
    }

    /// <summary>
    /// Проверяет номинал валюты.
    /// </summary>
    /// <param name="value">количество валюты.</param>
    /// <returns>
    /// true - если номинал валюты существует в массиве values.
    /// false - если отсутствует.
    /// </returns>
    private bool CheckValue(int value)
    {
        if (!values.Contains(value))
        {
            Console.WriteLine("Валюты такого номинала не существует");
            return false;
        }

        return true;
    }
}