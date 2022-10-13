using System.Data.SqlClient;
using Coffee_machine_console.Wrappers;

namespace Coffee_machine_console;

public class DB
{
    //Параметры для подключения к БД
    private string _dbParams = "Server=localhost;Database=Coffee_machine;Integrated Security=True";
    // Экземляр класса подключения к БД
    private SqlConnection _connection;
    //Максимально допустимое количество добавляемого ресурса
    private int _maxResValue = 1000;


    public DB()
    {
        _connection = new SqlConnection(_dbParams);
    }

    //Получение данных о напитке по ID
    //Возвращает словарь: название столбца - значение столбца
    public Dictionary<string, object> GetDrink(int id)
    {
        _connection.Open();
        try
        {
            Dictionary<string, object> drinkItem = new Dictionary<string, object>();
            
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

    //Получение всех напитков
    //Возвращает словарь: id - массив значений столбцов
    public Dictionary<int, object[]> GetAllDrinks()
    {
        _connection.Open();
        try
        {
            Dictionary<int, object[]> drinkList = new Dictionary<int, object[]>();
            
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

    //Получение количества выбранного ресурса
    //Возвращает количество ресурса
    public int GetResource(string resourceName)
    {
        _connection.Open();
        try
        {
            int value = 0;
            
            QueryBuilder qb = new QueryBuilder();
            string sql = qb.Table("resource")
                .Select("resource_value")
                .Where("resource_name", resourceName)
                .Sql();
            SqlCommand command = new SqlCommand(sql, _connection);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                value = (int)reader["resource_value"];
            }

            return value;
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

    //Получение всех ресурсов
    //Возвращает словарь: id - количество
    public Dictionary<int, int> GetAllResources()
    {
        _connection.Open();
        try
        {
            Dictionary<int, int> resources = new Dictionary<int, int>();
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

    //Получение количества напитков в базе данных
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
            throw;
        }
        finally
        {
            _connection.Close();
        }
    }

    //Получение рецепта напитка по id
    //Возвращает словарь: название ингредиента - необходимое количество
    public Dictionary<string, int> GetDrinkRecipe(int drinkID)
    {
        _connection.Open();
        try
        {
            Dictionary<string, int> ingredients = new Dictionary<string, int>();
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
            throw;
        }
        finally
        {
            _connection.Close();
        }

    }

    //Выполнение заказа
    public void ExecuteOrder(int coffeeType, Dictionary<string, int> supplyment)
    {
        try
        {
            //Создание словаря с посчитанными затратами на приготовление
            Dictionary<int, int> result = EvalCost(coffeeType, supplyment);
            
            //Получение цены напитка
            int price = (int)GetDrink(coffeeType)["drink_price"];

            _connection.Open();

            //Построчное обновление каждой строки ресурсов в БД
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

            //Запись данных в логи
            CreateLog(new LogData(coffeeType, price));
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

    //Проверка что результаты после затрат не будут отрицательными
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

    //Создание записи с данными в таблице с логами
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
            throw;
        }
    }

    //Заполнение выбранного ресурса
    public void FillResource(string resourceName, int value)
    {
        _connection.Open();
        try
        {
            value = value > _maxResValue ? _maxResValue : value;
            
            QueryBuilder qb = new QueryBuilder();
            string sql = qb.Table("resource")
                .Update()
                .Set("resource_value", value)
                .Where("resource_name", resourceName)
                .Sql();
            SqlCommand command = new SqlCommand(sql, _connection);
            command.ExecuteNonQuery();
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

    //Объединение двух словарей в один
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

    //Рассчитывание стоимости
    //Объединяет полученный словарь добавок с рецептом выбранного кофе
    //Отнимает объедененный словарь из словаря с общим количеством ресурсов
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