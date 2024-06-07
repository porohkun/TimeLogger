using System;
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
            try
            {
                var value = await _valuesRepository.Query(q => q.FirstOrDefault(v => v.Name == name));
                return value?.Caption is null
                    ? default
                    : JsonConvert.DeserializeObject<T>(value.Caption);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task Set(string name, object value)
        {
            var val = await _valuesRepository.Query(q => q.FirstOrDefault(v => v.Name == name))
                ?? new Value { Name = name };
            val.Caption = JsonConvert.SerializeObject(value);
            await _valuesRepository.AddOrUpdateAsync(val);
        }
    }
}
