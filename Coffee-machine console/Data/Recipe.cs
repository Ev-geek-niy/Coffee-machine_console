namespace Coffee_machine_console.Resources;

/// <summary>
/// Класс рецепта напитка.
/// </summary>
public class Recipe
{
    public Coffee coffee;
    public Water water;
    public Milk milk;
    public Sugar sugar;

    public Recipe(Coffee coffee, Water water, Milk milk)
    {
        this.coffee = coffee;
        this.water = water;
        this.milk = milk;
        this.sugar = new Sugar(0);
    }

    /// <summary>
    /// Выводит рецепт напитка в консоль.
    /// </summary>
    public void printRecipe()
    {
        Console.WriteLine($"coffee: {coffee.value}, water: {water.value}, milk: {milk.value}, sugar: {sugar.value}");
    }
}