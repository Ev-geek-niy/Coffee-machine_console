using System.Data.SqlClient;
using System.Numerics;

namespace Coffee_machine_console;

public class DB
{
    private string _dbParams = "Server=localhost;Database=Coffee_machine;Integrated Security=True";
    private SqlConnection? _connection;

    public DB()
    {
        _connection = new SqlConnection(_dbParams);
    }

    public void GetAllDrinks()
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

                //TODO: Добавить return
                //Переписать функция получения всех напитков не должна ничего выводить в консоль
                //Она должна возвращать скорее всего словарь, с сопоставлением, а вывод на консоль уже API
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

    public void GetResource(string resourceName)
    {
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
            Dictionary<string, int> cost = GetDrinkRecipe(coffeeType);
            cost = UnionDictionary(supplyment, cost);
            var resources = GetAllResources();
            var result = EvalCost(resources, cost);
            
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
                int number = command.ExecuteNonQuery();
            }
        }
        catch (SqlException e)
        {
            Console.WriteLine("Не хватает Ингредиент , пожалуйста пополните машину");
        }
        finally
        {
            _connection.Close();
        }
    }

    public Dictionary<string, int> UnionDictionary(Dictionary<string, int> dict1, Dictionary<string, int> dict2)
    {
        foreach (var pair in dict2)
        {
            if (dict1.ContainsKey(pair.Key))
                dict1[pair.Key] += pair.Value;
            else
                dict1.Add(pair.Key, pair.Value);
        }

        return dict1;
    }

    public Dictionary<int, int> EvalCost(Dictionary<int, int> resources, Dictionary<string, int> cost)
    {
        resources[1] -= 1;
        resources[2] -= cost["coffee"];
        resources[3] -= cost["water"];
        resources[4] -= cost["milk"];
        resources[5] -= cost["sugar"];

        return resources;
    }
}