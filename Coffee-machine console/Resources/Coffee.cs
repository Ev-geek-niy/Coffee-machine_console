namespace Coffee_machine_console.Resources;

public class Coffee : Resource
{
    public string valueType = "кг";

    public Coffee() : base()
    {
    }

    public Coffee(int value) : base(value)
    {
    }
}