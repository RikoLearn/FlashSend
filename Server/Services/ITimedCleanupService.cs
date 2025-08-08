namespace Server.Services
{
    public interface ITimedCleanupService
    {
        Task Cleanup();
    }
}
