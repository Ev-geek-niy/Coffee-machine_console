namespace Coffee_machine_console.Resources;

/// <summary>
/// Класс сахара.
/// </summary>
public class Sugar : Resource
{
    protected override string valueType
    {
        get => "кг";
    }

    public Sugar() : base()
    {
    }

    public Sugar(int value) : base(value)
    {
    }
}