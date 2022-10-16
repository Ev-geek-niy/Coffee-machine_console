namespace Coffee_machine_console.PaymentMethods;

public class CashPayment : Payment
{

    private readonly int[] values = new[] { 1, 2, 5, 10, 50, 100, 200 };

    public CashPayment()
    {
        this.totalFunds = 0;
    }

    public void AddMoney(int value)
    {
        if (CheckValue(value))
            this.totalFunds += value;
    }

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