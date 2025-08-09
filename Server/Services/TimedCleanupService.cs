using Data;
using Domain;

namespace Server.Services
{
    public class TimedCleanupService : ITimedCleanupService
    {
        public TimedCleanupService(IServiceProvider service)
        {
            _service = service;
        }

        public async Task Cleanup(CancellationToken cancellationToken = default)
        {
            try
            {
                var scope = _service.CreateScope();
                Context _context = scope.ServiceProvider.GetRequiredService<Context>();

                var now = DateTime.Now;

                //var expiredRecords = await _context.Papers
                //    .Where(e => e.InsertDateTime + e.ExpireTime < now)
                //    .ToListAsync();

                //TODO: use better solution for check expired papers
                var expiredRecords = _context.Papers.AsEnumerable()
                    .Where(e => e.InsertDateTime.AddMinutes(e.ExpireTime.Minutes) < now)
                    .ToList();

                if (expiredRecords.Any())
                {
                    foreach (var rec in expiredRecords)
                    {
                        var documents = _context.Documents.Where(x => x.PaperId == rec.Id).ToList();
                        if (documents.Count == 0)
                            continue;

                        foreach (var item in documents)
                        {
                            if (item.Type == DocumnetType.File)
                            {
                                var folderPath = Path.Combine(
                                 Directory.GetCurrentDirectory(), item.FilePath
                                 );

                                if (File.Exists(folderPath))
                                {
                                    File.Delete(folderPath);
                                }
                            }
                        }
                    }
                    _context.Papers.RemoveRange(expiredRecords);
                    await _context.SaveChangesAsync(cancellationToken);
                }
            }
            catch (Exception)
            {
            }
        }

        private readonly IServiceProvider _service;
    }
}
