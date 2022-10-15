namespace Coffee_machine_console.Resources;

public class Addition
{
    public Milk milk;
    public Sugar sugar;

    public Addition()
    {
        milk = new Milk();
        sugar = new Sugar();
    }

    public void Add(string resourceType, int value)
    {
        switch (resourceType)
        {
            case "milk":
                this.milk.Add(value);
                break;
            case "sugar":
                this.sugar.Add(value);
                break;
        }
    }
}