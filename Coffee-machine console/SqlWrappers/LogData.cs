using Coffee_machine_console.Resources;

namespace Coffee_machine_console.SqlWrappers;

public class LogData : DbData
{
    private readonly int _logDrinkId;
    private readonly int _logDrinkPrice;

    public LogData(Order order)
    {
        this._logDrinkId = order.drink.id;
        this._logDrinkPrice = order.drink.price;
    }

    public override string getValuesSql()
    {
        return $"{this._logDrinkId}, {this._logDrinkPrice}";
    }
}