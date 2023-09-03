using System.Threading.Tasks;

namespace TimeLogger.Abstractions
{
    /// <summary>
    /// Сервис работы с сохраняемыми значениями
    /// </summary>
    public interface IValuesService
    {
        Task<T?> Get<T>(string name);
        Task Set(string name, object value);
    }
}
