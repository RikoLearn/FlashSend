using Data;
using Microsoft.EntityFrameworkCore;

namespace Server.Services
{
    public class UniqueNumberService : IUniqueNumberService
    {
        public UniqueNumberService(Context context)
        {
            _context = context;
        }

        public async Task<int> GenerateUniqueNumberAsync()
        {
            int attempts = 0;
            int maxAttempts = 100;

            while (attempts < maxAttempts)
            {
                var number = _random.Next(100000, 1000000); 

                var exists = await _context.Papers.AnyAsync(e => e.UniqueNumber == number);

                if (!exists)
                {
                    return number;
                }

                attempts++;
            }

            throw new Exception("Failed to generate a unique number after multiple attempts");
        }

        private readonly Context _context;
        private readonly Random _random = new Random();

    }
}
