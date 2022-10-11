namespace Coffee_machine_console.Wrappers;

public class LogData : DbData
{
    private readonly int _logId;
    private readonly int _logPrice;

    public LogData(int logId, int logPrice)
    {
        this._logId = logId;
        this._logPrice = logPrice;
    }

    public override string getValuesSql()
    {
        return $"{this._logId}, {this._logPrice}";
    }
}