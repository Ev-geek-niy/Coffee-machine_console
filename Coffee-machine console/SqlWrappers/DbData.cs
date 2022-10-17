namespace Coffee_machine_console.SqlWrappers;

/// <summary>
/// Абстрактный класс "обертки" для БД.
/// </summary>
public abstract class DbData
{
    /// <summary>
    /// Генерирует строку c перечислением всех значений для SQL-запроса.
    /// </summary>
    /// <returns>Строка с полями класса, разделенная запятыми</returns>
    public abstract string getValuesSql();
}