using Coffee_machine_console.PaymentMethods;

namespace Coffee_machine_console.Factory;

/// <summary>
/// Класс фабрики метода оплаты.
/// </summary>
public class PaymentCreator
{
    /// <summary>
    /// Создает экземпляр класса определенного типа оплаты.
    /// </summary>
    /// <param name="method">Строка с типом оплаты.</param>
    /// <returns>
    /// Экземпляр класса типа оплаты.
    /// Если такого нет - null.
    /// </returns>
    public Payment CreatePaymentMethod(string method)
    {
        switch (method)
        {
            case "cash":
                return new CashPayment();
            case "cashless":
                return new CashlessPayment();
        }

        return null;
    }
}