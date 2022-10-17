namespace Coffee_machine_console.Resources;

/// <summary>
/// Класс молотого кофе.
/// </summary>
public class Coffee : Resource
{
    protected override string valueType
    {
        get => "кг";
    }

    public Coffee() : base()
    {
    }

    public Coffee(int value) : base(value)
    {
    }
}