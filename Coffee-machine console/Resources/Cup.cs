namespace Coffee_machine_console.Resources;

/// <summary>
/// Класс кружки.
/// </summary>
public class Cup : Resource
{
    protected override string valueType
    {
        get => "штук";
    }

    protected override int delimeter
    {
        get => 1;
    }

    public Cup() : base()
    {
    }

    public Cup(int value) : base(value)
    {
    }
}