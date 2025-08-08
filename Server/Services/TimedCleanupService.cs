using Data;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Server.Services
{
    public class TimedCleanupService : ITimedCleanupService
    {
        private readonly IServiceProvider _service;
        public TimedCleanupService(IServiceProvider service)
        {
            _service = service;
        }


        public async Task Cleanup()
        {
            try
            {
                
                var scope = _service.CreateScope();
                Context _context = scope.ServiceProvider.GetRequiredService<Context>();

                var now = DateTime.Now;
                var expiredRecords = _context.Papers.AsEnumerable()
                    .Where(e => e.InsertDateTime.AddMinutes(e.ExpireTime.Minutes) < now)
                    .ToList();

                //var expiredRecords = await _context.Papers
                //    .Where(e => e.InsertDateTime + e.ExpireTime < now)
                //    .ToListAsync();

                if (expiredRecords.Any())
                {
                    foreach (var rec in expiredRecords)
                    {
                        var documents = _context.Documents.Where(x => x.PaperId == rec.Id).ToList();

                        foreach (var item in documents)
                        {
                            if (item.Type == DocumnetType.Image || item.Type == DocumnetType.File)
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
                    await _context.SaveChangesAsync();
                }

            }
            catch (Exception ex)
            {
            }
        }
    }
}
