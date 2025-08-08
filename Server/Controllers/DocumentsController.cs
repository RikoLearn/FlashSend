using Asp.Versioning;
using Data;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class DocumentsController : ControllerBase
    {
        public DocumentsController(Context context)
        {
            _context = context;
        }

        [HttpPost("{uniqueNumber}")]
        public async Task<IActionResult> Create(int uniqueNumber, string content, DocumnetType type = DocumnetType.Text)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var paper = _context.Papers.FirstOrDefault(x => x.UniqueNumber == uniqueNumber);

            if (paper == null)
                return BadRequest(new Exception(nameof(paper)).Message);

            var document = new Document(paper, type.ToString(), content, type);

            _context.Documents.Add(document);

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("Image/{uniqueNumber}")]
        public async Task<IActionResult> Create(int uniqueNumber, IFormFile file, DocumnetType type = DocumnetType.Image)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            //try catch باید اضافه بشه 

            const int maxFileSize = 10 * 1024 * 1024;

            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("file", "لطفا یک فایل انتخاب کنید.");
                return BadRequest(ModelState);
            }

            if (file.Length > maxFileSize)
            {
                ModelState.AddModelError("file", $"حجم فایل نباید بیشتر از {maxFileSize / (1024 * 1024)} مگابایت باشد.");
                return BadRequest(ModelState);
            }

            var paper = _context.Papers.FirstOrDefault(x => x.UniqueNumber == uniqueNumber);

            string fileName = Guid.NewGuid().ToString();

            var folderPath = Path.Combine(
                 Directory.GetCurrentDirectory(), $"wwwroot\\Upload"
                 );

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var path = Path.Combine(folderPath,
                fileName + Path.GetExtension(file.FileName));

            // encrption باید اضافه بشه

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            var folderPathUrl = Path.Combine(
                 $"wwwroot\\Upload",
                  fileName + Path.GetExtension(file.FileName)
             );

            if (paper == null)
                return BadRequest(new Exception(nameof(paper)).Message);

            var document = new Document(paper, Path.GetFileNameWithoutExtension(file.FileName), folderPathUrl, Path.GetExtension(file.FileName), (int)file.Length, type);

            _context.Documents.Add(document);

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{uniqueNumber}")]
        public async Task<IActionResult> Delete(int uniqueNumber)
        {

            var paper = _context.Papers.FirstOrDefault(x => x.UniqueNumber == uniqueNumber);

            if (paper == null)
                return NotFound(new Exception(nameof(paper)).Message);

            var documents = _context.Documents.Where(x => x.PaperId == paper.Id).ToList();

            foreach (var item in documents)
            {
                if (item.Type == DocumnetType.Image || item.Type == DocumnetType.File)
                {
                    var folderPath = Path.Combine(
                     Directory.GetCurrentDirectory(), item.FilePath
                     );

                    System.IO.File.Delete(folderPath);
                }
            }

            _context.Papers.Remove(paper);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private readonly Context _context;
    }
}
