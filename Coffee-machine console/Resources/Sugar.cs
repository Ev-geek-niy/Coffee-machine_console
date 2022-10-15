namespace Coffee_machine_console.Resources;

public class Sugar : Resource
{
    public string valueType = "кг";

    public Sugar() : base()
    {
    }

    public Sugar(int value) : base(value)
    {
    }
    
    public void Add(int value)
    {
        this.value += value;
    }
}