namespace Coffee_machine_console.Resources;

/// <summary>
/// Класс ресурса.
/// От него наследуются все базовые ресурсы.
/// </summary>
public abstract class Resource
{
    public int value;
    protected int maxValue = 1000;

    protected virtual int delimeter
    {
        get => 1000;
    }

    protected abstract string valueType { get; }

    public Resource()
    {
        this.value = 0;
    }

    public Resource(int value)
    {
        if (value > maxValue)
        {
            this.value = maxValue;
            return;
        }

        this.value = value;
    }

    /// <summary>
    /// Прибавляет определенное количество ресурса.
    /// </summary>
    /// <param name="value">Количество ресурса.</param>
    public virtual void Add(int value)
    {
        this.value += value;
    }

    /// <summary>
    /// Выводит в консоль информацию о ресурсе.
    /// </summary>
    public virtual void PrintInfo()
    {
        double convertedValue = value / delimeter;
        if (value == 0)
            Console.WriteLine($"{this.GetType().Name}: отсутствует");
        else
            Console.WriteLine($"{this.GetType().Name}: {convertedValue} {valueType}");
    }
}