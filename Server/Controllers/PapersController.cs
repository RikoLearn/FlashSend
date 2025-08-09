using Asp.Versioning;
using Data;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Server.Services;

namespace Server.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class PapersController : ControllerBase
    {
        public PapersController(Context context, IUniqueNumberService uniqueNumberService)
        {
            _context = context;
            _uniqueNumberService = uniqueNumberService;
        }

        /// <summary>
        /// Create Paper for given expire time
        /// between 10 , 30 , 60 Minute
        /// </summary>
        /// <param name="expireTime"></param>
        /// <response code="200">Create with return paper unique number </response>
        /// <response code="400">operation problem</response>
        [HttpPost]
        [Route("{expireTime}")]
        public async Task<IActionResult> Create(int expireTime)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var defaultExpireTimes = new List<int>() {
                10,30,60
            };

            if (!defaultExpireTimes.Contains(expireTime))
                return BadRequest(new ArgumentOutOfRangeException().Message);


            var uniqueNumber = await _uniqueNumberService.GenerateUniqueNumberAsync();

            var paper = new Paper(TimeSpan.FromMinutes(expireTime), uniqueNumber);

            _context.Papers.Add(paper);
            await _context.SaveChangesAsync();

            return Ok(new { UniqueNumber = paper.UniqueNumber });
        }


        private readonly Context _context;
        private readonly IUniqueNumberService _uniqueNumberService;
    }
}
