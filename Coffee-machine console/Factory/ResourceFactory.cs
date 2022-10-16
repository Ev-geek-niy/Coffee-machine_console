using Coffee_machine_console.Resources;

namespace Coffee_machine_console.Factory;


/// <summary>
/// Класс фабрики ресурсов.
/// </summary>
public class ResourceFactory
{
    /// <summary>
    /// Создает экземпляр класса определенного типа ресурса.
    /// </summary>
    /// <param name="type">Тип ресурса.</param>
    /// <param name="value">Количество ресурса.</param>
    /// <returns>
    /// Экземпляр класса типа ресурса.
    /// Если такого нет - null.
    /// </returns>
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