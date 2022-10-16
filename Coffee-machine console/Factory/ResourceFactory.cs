using Coffee_machine_console.Resources;

namespace Coffee_machine_console.Factory;

public class ResourceFactory
{
    public Resource CreateResource(string type, int value)
    {
        switch (type)
        {
            case "coffee":
                return new Coffee(value);
            case "water":
                return new Water(value);
            case "milk":
                return new Milk(value);
            case "sugar":
                return new Sugar(value);
            case "cup":
                return new Cup(value);
        }

        return null;
    }
}