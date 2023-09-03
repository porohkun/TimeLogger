namespace TimeLogger.Shared.Abstractions
{
    public interface IBaseEntity : IHaveId
    {
        new long Id { get; set; }
    }
}