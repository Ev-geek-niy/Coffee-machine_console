namespace Coffee_machine_console.Resources;

/// <summary>
/// Класс воды.
/// </summary>
public class Water : Resource
{
    protected override string valueType
    {
        get => "литров";
    }

    public Water() : base()
    {
    }

    public Water(int value) : base(value)
    {
    }
}