namespace Coffee_machine_console.Resources;

/// <summary>
/// Класс сахара.
/// </summary>
public class Sugar : Resource
{
    public string valueType = "кг";

    public Sugar() : base()
    {
    }

    public Sugar(int value) : base(value)
    {
    }
}