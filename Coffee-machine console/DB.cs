using System.Data.SqlClient;
using Coffee_machine_console.Wrappers;

namespace Coffee_machine_console;

public class DB
{
    private string _dbParams = "Server=localhost;Database=Coffee_machine;Integrated Security=True";
    private SqlConnection? _connection;
    private int maxResValue = 1000;


    public DB()
    {
        _connection = new SqlConnection(_dbParams);
    }

    public Dictionary<string, object> GetDrink(int id)
    {
        Dictionary<string, object> drinkItem = new Dictionary<string, object>();
        _connection.Open();
        try
        {
            QueryBuilder qb = new QueryBuilder();
            string sql = qb.Table("drink")
                .Where("drink_id", id)
                .Sql();
            SqlCommand command = new SqlCommand(sql, _connection);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                drinkItem.Add("drink_name", reader["drink_name"]);
                drinkItem.Add("drink_price", reader["drink_price"]);
            }

            return drinkItem;
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

    public Dictionary<int, object[]> GetAllDrinks()
    {
        Dictionary<int, object[]> drinkList = new Dictionary<int, object[]>();
        _connection.Open();
        try
        {
            QueryBuilder qb = new QueryBuilder();
            string sql = qb.Table("drink").Sql();

            SqlCommand command = new SqlCommand(sql, _connection);
            SqlDataReader reader = command.ExecuteReader();

            int i = 0;
            while (reader.Read())
            {
                drinkList.Add(i, new object[] { reader["drink_id"], reader["drink_name"], reader["drink_price"] });
                i++;
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

    public int GetResource(string resourceName)
    {
        int value = 0;
        _connection.Open();
        try
        {
            QueryBuilder qb = new QueryBuilder();
            string sql = qb.Table("resource").Select("resource_value").Where("resource_name", resourceName).Sql();
            SqlCommand command = new SqlCommand(sql, _connection);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                value = (int)reader["resource_value"];
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            _connection.Close();
        }

        return value;
    }

    public Dictionary<int, int> GetAllResources()
    {
        Dictionary<int, int> resources = new Dictionary<int, int>();
        _connection.Open();
        try
        {
            QueryBuilder qb = new QueryBuilder();
            string sql = qb.Table("resource")
                .Select("resource_id", "resource_value")
                .Sql();
            SqlCommand command = new SqlCommand(sql, _connection);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                resources.Add((int)reader["resource_id"], (int)reader["resource_value"]);
            }

            return resources;
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

    public int GetDrinkCount()
    {
        _connection.Open();
        try
        {
            QueryBuilder qb = new QueryBuilder();
            string sql = qb.Table("drink")
                .Count()
                .Sql();
            SqlCommand command = new SqlCommand(sql, _connection);
            int count = (int)command.ExecuteScalar();
            return count;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            _connection.Close();
        }

        return 0;
    }

    public Dictionary<string, int> GetDrinkRecipe(int drinkID)
    {
        Dictionary<string, int> ingredients = new Dictionary<string, int>();
        _connection.Open();
        try
        {
            QueryBuilder qb = new QueryBuilder();
            string sql = qb.Table("recipe")
                .Select("recipe_coffee", "recipe_water", "recipe_milk")
                .LeftJoin("drink", "recipe_drink_id", "drink_id")
                .Where("recipe_drink_id", drinkID)
                .Sql();

            SqlCommand command = new SqlCommand(sql, _connection);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                ingredients["coffee"] = (int)reader[0];
                ingredients["water"] = (int)reader[1];
                ingredients["milk"] = (int)reader[2];
            }

            return ingredients;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            _connection.Close();
        }

        return null;
    }

    public void ExecuteOrder(int coffeeType, Dictionary<string, int> supplyment, int money)
    {
        try
        {
            Dictionary<int, int> result = EvalCost(coffeeType, supplyment);
            int price = (int)GetDrink(coffeeType)["drink_price"];

            _connection.Open();

            foreach (var res in result)
            {
                QueryBuilder qb = new QueryBuilder();
                string sql = qb.Table("resource")
                    .Update()
                    .Set("resource_value", res.Value)
                    .Where("resource_id", res.Key)
                    .Sql();
                SqlCommand command = new SqlCommand(sql, _connection);
                command.ExecuteNonQuery();
            }

            CreateLog(new LogData(coffeeType, price));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            _connection.Close();
        }
    }

    public bool CheckResources(int coffeeType, Dictionary<string, int> supplyment)
    {
        foreach (KeyValuePair<int, int> pair in EvalCost(coffeeType, supplyment))
        {
            if (pair.Value < 0)
            {
                return false;
            }
        }

        return true;
    }

    public void CreateLog(DbData obj)
    {
        try
        {
            QueryBuilder qb = new QueryBuilder();
            string sql = qb.Table("log").Insert("log_drink_id", "log_price").Values(obj).Sql();
            SqlCommand command = new SqlCommand(sql, _connection);
            command.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public void FillResource(string resourceName, int value)
    {
        value = value > maxResValue ? maxResValue : value;
        _connection.Open();
        try
        {
            QueryBuilder qb = new QueryBuilder();
            string sql = qb.Table("resource")
                .Update()
                .Set("resource_value", value)
                .Where("resource_name", resourceName)
                .Sql();
            Console.WriteLine(sql);
            SqlCommand command = new SqlCommand(sql, _connection);
            command.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            _connection.Close();
        }
    }

    private Dictionary<string, int> UnionDictionary(Dictionary<string, int> dict1, Dictionary<string, int> dict2)
    {
        Dictionary<string, int> tempDict = dict1.ToDictionary(
            entry => entry.Key,
            entry => entry.Value);

        foreach (var pair in dict2)
        {
            if (tempDict.ContainsKey(pair.Key))
                tempDict[pair.Key] += pair.Value;
            else
                tempDict.Add(pair.Key, pair.Value);
        }

        return tempDict;
    }

    //TODO: придумать как переписать без хардкода
    private Dictionary<int, int> EvalCost(int coffeeType, Dictionary<string, int> supplyment)
    {
        Dictionary<string, int> cost = UnionDictionary(supplyment, GetDrinkRecipe(coffeeType));
        Dictionary<int, int> resources = GetAllResources();

        resources[1] -= 1;
        resources[2] -= cost["coffee"];
        resources[3] -= cost["water"];
        resources[4] -= cost["milk"];
        resources[5] -= cost["sugar"];

        return resources;
    }
}