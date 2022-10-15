namespace Coffee_machine_console.Resources;

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
        this.recipe = db.getRecipe(drink.id);
    }

    public void AddToOrder(string resourceType, int value)
    {
        addition.Add(resourceType, value);
    }

    
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
}