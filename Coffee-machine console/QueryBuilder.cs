namespace Coffee_machine_console;

public class QueryBuilder
{
    private string _table;
    private string _sql;
    private string[] _cols;

    public string Sql()
    {
        return _sql;
    }

    public QueryBuilder Table(string table)
    {
        this._table = table;
        this._sql = string.Empty;
        this._sql = this.Select("*").Sql();
        return this;
    }

    public QueryBuilder Select(params string[] cols)
    {
        this._cols = cols;
        this._sql = string.Concat(new string[]{
            "SELECT ",
            string.Join(", ", cols),
            " FROM ",
            _table
        });
        return this;
    }

    public QueryBuilder Count()
    {
        this._sql = $"SELECT COUNT(*) FROM {this._table}";
        return this;
    }

    public QueryBuilder Count(string col)
    {
        this._sql = $"SELECT COUNT({col}) FROM {this._table}";
        return this;
    }

    public QueryBuilder Where<T>(string col, T value)
    {
        this._sql += $" WHERE {col} = {value}";
        return this;
    }

    public QueryBuilder AndWhere<T>(string col, T value)
    {
        this._sql += $" AND WHERE {col} = {value}";
        return this;
    }
}