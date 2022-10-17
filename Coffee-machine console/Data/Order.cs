namespace Coffee_machine_console.Resources;

/// <summary>
/// Класс заказа.
/// </summary>
public class Order
{
    public Drink drink;
    public Recipe recipe;
    public Addition addition;

    public Order(Drink drink)
    {
        DB db = new DB();
        
        this.drink = drink;
        this.addition = new Addition();
        this.recipe = db.GetRecipe(drink.id);
    }

    /// <summary>
    /// Добавляет определенную добавку к заказу.
    /// </summary>
    /// <param name="resourceType">Тип добавки.</param>
    /// <param name="value">Количество добавки.</param>
    public void AddToOrder(string resourceType, int value)
    {
        addition.Add(resourceType, value);
    }

    /// <summary>
    /// Выводит информацию о заказе в консоль.
    /// </summary>
    public void PrintOrder()
    {
        if (addition.milk.value > 0 && addition.sugar.value > 0)
        {
            Console.WriteLine($"Ваш заказ: {drink.title}, плюс {addition.milk.value} мл молока, плюс {addition.sugar.value} гр сахара");
            return;
        }
        else if (addition.milk.value > 0)
        {
            Console.WriteLine($"Ваш заказ: {drink.title}, плюс {addition.milk.value} мл молока");
            return;
        }
        else if (addition.sugar.value > 0)
        {
            Console.WriteLine($"Ваш заказ: {drink.title}, плюс {addition.sugar.value} гр сахара");
            return;
        }
        else
        {
            Console.WriteLine($"Ваш заказ: {drink.title}");
        }
    }

    public int[] createRawData()
    {
        return new[]
        {
            1,
            this.recipe.coffee.value,
            this.recipe.water.value,
            this.recipe.milk.value + this.addition.milk.value,
            this.recipe.sugar.value + this.addition.sugar.value
        };
    }
}