using System.Data.SqlClient;

namespace Coffee_machine_console;

public class DB
{
    private string _dbParams = "Server=localhost;Database=Coffee_machine;Integrated Security=True";
    private SqlConnection? _connection;

    public DB()
    {
        _connection = new SqlConnection(_dbParams);
    }

    public void getAllDrinks()
    {
        _connection.Open();
        try
        {
            QueryBuilder qb = new QueryBuilder();
            string sql = qb.Table("drink").Sql();
            
            SqlCommand command = new SqlCommand(sql, _connection);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                var id = reader.GetValue(0);
                var name = reader.GetValue(1);
                var price = reader.GetValue(2);
                
                Console.WriteLine($"{id} - {name} цена: {price} рублей");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            _connection.Close();
        }
    }

    public int getDrinkCount()
    {
        _connection.Open();
        try
        {
            QueryBuilder qb = new QueryBuilder();
            string sql = qb.Table("drink").Count().Sql();
            SqlCommand command = new SqlCommand(sql, _connection);
            int count = (int) command.ExecuteScalar();
            return count;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            _connection.Close();
        }
    }
}