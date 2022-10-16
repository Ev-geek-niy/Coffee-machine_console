namespace Coffee_machine_console.PaymentMethods;

public abstract class Payment
{
    protected int totalFunds;

    public virtual void Add(int value)
    {
        this.totalFunds += value;
    }

    public virtual void printFounds()
    {
        Console.WriteLine($"На счету: {this.totalFunds} рублей");
    }
}