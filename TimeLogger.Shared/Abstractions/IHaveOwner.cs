namespace TimeLogger.Shared.Abstractions
{
    public interface IHaveOwner<T> : IHaveOwnerId where T : IBaseEntity
    {
        T? Owner { get; set; }
    }
}
