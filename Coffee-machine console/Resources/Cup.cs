namespace Coffee_machine_console.Resources;

/// <summary>
/// Класс кружки.
/// </summary>
public class Cup : Resource
{
    private int delimeter = 1;
    public Cup() : base()
    {
    }

    public Cup(int value) : base(value)
    {
    }
}