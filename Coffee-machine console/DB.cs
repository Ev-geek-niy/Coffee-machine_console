using System.Data.SqlClient;
using Coffee_machine_console.Resources;

namespace Coffee_machine_console;

class DB
{
    private static string sqlParams = "Server=localhost;Database=Coffee_machine;Trusted_Connection=true;";
    private SqlConnection _connection;
    private QueryBuilder qb;

    public DB()
    {
        this._connection = new SqlConnection(sqlParams);
        this.qb = new QueryBuilder();
    }

    public List<Drink> getAllDrinks()
    {
        _connection.Open();
        try
        {
            List<Drink> drinkList = new List<Drink>();
            string sql = qb.Table("drink").Sql();
            SqlCommand command = new SqlCommand(sql, _connection);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                drinkList.Add(new Drink(
                    (int)reader["drink_id"],
                    (string)reader["drink_name"],
                    (int)reader["drink_price"]));
            }
            return drinkList;
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

    public Drink getDrink(int id)
    {
        _connection.Open();
        try
        {
            string sql = qb.Table("drink").Where("drink_id", id).Sql();
            SqlCommand command = new SqlCommand(sql, _connection);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                return new Drink(
                    (int)reader["drink_id"], 
                    (string) reader["drink_name"],
                    (int) reader["drink_price"]);
            }

            return null;
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

    public Recipe getRecipe(int drinkId)
    {
        _connection.Open();
        try
        {
            string sql = qb.Table("recipe")
                .Select("recipe_coffee", "recipe_water", "recipe_milk")
                .Where("recipe_drink_id", drinkId)
                .Sql();
            SqlCommand command = new SqlCommand(sql, _connection);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                return new Recipe(
                    new Coffee((int) reader["recipe_coffee"]),
                    new Water((int) reader["recipe_water"]),
                    new Milk((int) reader["recipe_milk"]));
            }

            return null;
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

    public int getResourceValue(string name)
    {
        _connection.Open();
        try
        {
            string sql = qb.Table("resource")
                .Select("resource_value")
                .Where("resource_name", name)
                .Sql();
            SqlCommand command = new SqlCommand(sql, _connection);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                return (int)reader["resource_value"];
            }

            return -1;
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