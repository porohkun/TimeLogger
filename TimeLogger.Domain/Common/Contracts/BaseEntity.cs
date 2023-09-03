using System.ComponentModel.DataAnnotations;
using TimeLogger.Shared.Abstractions;

namespace TimeLogger.Domain.Common.Contracts
{
    public abstract record BaseEntity : IBaseEntity
    {
        private long _id;
        private bool _idSetted;

        [Key, Required]
        public long Id
        {
            get => _id;
            set
            {
                if (_idSetted) return;
                _id = value;
                _idSetted = true;
            }
        }
    }
}
