namespace Coffee_machine_console.Resources;

/// <summary>
/// Класс добавки.
/// </summary>
public class Addition
{
    public Milk milk;
    public Sugar sugar;

    public Addition()
    {
        milk = new Milk();
        sugar = new Sugar();
    }

    /// <summary>
    /// Добавляет определенное количество выбранного типа добавки.
    /// </summary>
    /// <param name="resourceType">Тип добавки.</param>
    /// <param name="value">Количество добавки.</param>
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