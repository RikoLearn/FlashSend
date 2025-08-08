namespace Server.Services
{
    public interface IUniqueNumberService
    {
        Task<int> GenerateUniqueNumberAsync();
    }
}
