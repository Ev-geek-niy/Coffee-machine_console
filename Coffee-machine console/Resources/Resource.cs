namespace Coffee_machine_console.Resources;

public abstract class Resource
{
    public int value;
    protected int delimeter = 100;

    public Resource()
    {
        this.value = 0;
    }
    
    public Resource(int value)
    {
        this.value = value;
    }
}