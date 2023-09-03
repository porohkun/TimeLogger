using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TimeLogger.Abstractions;
using TimeLogger.Attributes;
using TimeLogger.Domain.Data;
using TimeLogger.Shared.Abstractions;

namespace TimeLogger.Services
{
    /// <inheritdoc/>
    [AsSingletone(typeof(IValuesService))]
    public class ValuesService : IValuesService
    {
        private readonly IRepository<Value> _valuesRepository;

        public ValuesService(IRepository<Value> valuesRepository)
        {
            _valuesRepository = valuesRepository;
        }

        public async Task<T?> Get<T>(string name)
        {
            var value = await _valuesRepository.Query(q => q.Where(v => v.Name == name).FirstOrDefault());
            if (value == null || value.Caption is null)
                return default(T);
            return JsonConvert.DeserializeObject<T>(value.Caption);
        }

        public async Task Set(string name, object value)
        {
            var val = await _valuesRepository.Query(q => q.Where(v => v.Name == name).FirstOrDefault())
                ?? new Value() { Name = name };
            val.Caption = JsonConvert.SerializeObject(value);
            await _valuesRepository.AddOrUpdateAsync(val);
        }
    }
}
