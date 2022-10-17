namespace Coffee_machine_console.Resources;

/// <summary>
/// Класс молока.
/// </summary>
public class Milk : Resource
{
    protected override string valueType
    {
        get => "литров";
    }

    public Milk() : base()
    {
    }

    public Milk(int value) : base(value)
    {
    }
}