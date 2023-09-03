using TimeLogger.Shared.Abstractions;

namespace TimeLogger.Domain.Common.Contracts
{
    public abstract record BaseEntityWithOwner<TOwner> : BaseEntity, IHaveOwner<TOwner> where TOwner : IBaseEntity
    {
        public long OwnerId { get; init; }
        public TOwner? Owner { get; set; }
    }
}
