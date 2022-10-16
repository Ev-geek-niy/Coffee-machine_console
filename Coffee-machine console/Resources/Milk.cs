namespace Coffee_machine_console.Resources;

public class Milk : Resource
{
    public string valueType = "литров";

    public Milk() : base()
    {
    }

    public Milk(int value) : base(value)
    {
    }
}