namespace Coffee_machine_console.Resources;

public abstract class Resource
{
    public int value;
    protected int maxValue = 1000;
    protected int delimeter = 100;

    public Resource()
    {
        this.value = 0;
    }
    
    public Resource(int value)
    {
        if (value > maxValue)
        {
            this.value = maxValue;
            return;
        }
        
        this.value = value;
    }
    
    public virtual void Add(int value)
    {
        this.value += value;
    }
}