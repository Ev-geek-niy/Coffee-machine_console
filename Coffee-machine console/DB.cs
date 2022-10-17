using System.Data.SqlClient;
using Coffee_machine_console.PaymentMethods;
using Coffee_machine_console.Resources;
using Coffee_machine_console.SqlWrappers;

namespace Coffee_machine_console;

/// <summary>
/// Класс базы данных.
/// </summary>
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

    /// <summary>
    /// Получает список всех напитков из БД.
    /// </summary>
    /// <returns>Список всех напитков.</returns>
    public List<Drink> GetAllDrinks()
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

    /// <summary>
    /// Получает выбранный напиток из БД.
    /// </summary>
    /// <param name="id">ID напитка.</param>
    /// <returns>Экземпляр класса Drink.</returns>
    public Drink GetDrink(int id)
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

    /// <summary>
    /// Получает рецепт напитка из БД.
    /// </summary>
    /// <param name="drinkId">ID напитка.</param>
    /// <returns>Экземпляр класса Recipe.</returns>
    public Recipe GetRecipe(int drinkId)
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

    /// <summary>
    /// Получает значение выбранного ресурса из БД.
    /// </summary>
    /// <param name="name">Тип ресурса.</param>
    /// <returns>
    /// Количество ресурса.
    /// Если ресурса нет в БД, то возвращает -1.
    /// </returns>
    public int GetResourceValue(string name)
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

    /// <summary>
    /// Заполняет выбранный ресурс в базе данных.
    /// </summary>
    /// <param name="resource">Экземпляр одного из наследников класса Resource
    /// с необходимым количеством ресурса.</param>
    /// <returns>Число строк, которые подверглись изменению</returns>
    public int FillResource(Resource resource)
    {
        _connection.Open();
        try
        {
            string sql = qb.Table("resource")
                .Update()
                .Set("resource_value", resource.value)
                .Where("resource_name", resource.GetType().Name.ToLower())
                .Sql();
            SqlCommand command = new SqlCommand(sql, _connection);
            return command.ExecuteNonQuery();
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

    /// <summary>
    /// Отнимает из каждой строки материала требуемое значение.
    /// </summary>
    /// <param name="cost">Массив чисел, элемент которого соответствует строке в БД.</param>
    public void ExecuteOrder(int[] cost)
    {
        _connection.Open();
        try
        {
            string[] cols = new[] { "cup", "coffee", "water", "milk", "sugar" };
            for (int i = 0; i < cols.Length; i++)
            {
                string sql = qb.Table("resource")
                    .Update()
                    .Set("resource_value", cost[i])
                    .Where("resource_name", cols[i])
                    .Sql();

                SqlCommand command = new SqlCommand(sql, _connection);
                command.ExecuteNonQuery();
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
    
    /// <summary>
    /// Создание записи о заказе в БД.
    /// </summary>
    /// <param name="order">Объект заказа.</param>
    public void CreateLog(Order order)
    {
        _connection.Open();
        try
        {
            DbData log = new LogData(order);
            string sql = qb.Table("log").Insert("log_drink_id", "log_price").Values(log).Sql();
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
}