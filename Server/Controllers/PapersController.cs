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

        [HttpGet]
        [Route("{uniqueNumber}")]
        public async Task<IActionResult> DocumentList(int uniqueNumber)
        {

            var paper = _context.Papers.FirstOrDefault(x => x.UniqueNumber == uniqueNumber);

            if (paper == null)
                return NotFound(new Exception(nameof(paper)).Message);

            var documents = _context.Documents.Where(x => x.PaperId == paper.Id).ToList();
            if (!documents.Any())
                return NotFound(new Exception(nameof(documents)).Message);

            var DocumentList = new List<DocumentDto>();

            foreach (var item in documents)
            {
                DocumentList.Add(new DocumentDto()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Content = item.Content,
                    Size = item.FileSize,
                    Type = item.Type,
                    UploadedAt = item.InsertDateTime,
                    Language = item.Type == DocumnetType.Code ? Language.CSharp : Language.Auto,
                });
            }


            return Ok(DocumentList);
        }

        private readonly Context _context;
        private readonly IUniqueNumberService _uniqueNumberService;
    }
    public class DocumentDto
    {
        public Guid Id { get; set; }            
        public DocumnetType Type { get; set; }
        public string Name { get; set; }         
        public long? Size { get; set; }        
        public string Content { get; set; }      
        public Language? Language { get; set; }  
        public DateTime? UploadedAt { get; set; }
    }


    public enum Language
    {
        Auto,
        CSharp,
        Html,
        Cpp,
        Python,
        Php,
        JavaScript,
        Java
    }

}
