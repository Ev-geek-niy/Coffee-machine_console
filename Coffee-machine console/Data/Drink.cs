namespace Coffee_machine_console.Resources;

public class Drink
{
    public int id { get; set; }
    public string title { get; set; }
    public int price { get; set; }

    public Drink(int id, string title, int price)
    {
        this.id = id;
        this.title = title;
        this.price = price;
    }

    public void printInfo()
    {
        Console.WriteLine($"{this.id} - {this.title}, цена: {this.price} рублей");
    }
}