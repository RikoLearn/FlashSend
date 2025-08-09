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

        /// <summary>
        /// Upload text/code as Document for a paper
        /// </summary>
        /// <param name="uniqueNumber"></param>
        /// <param name="content"></param>
        /// <param name="type"></param>
        /// <response code="200">Upload Successfully</response>
        /// <response code="400">operation problem</response>
        [HttpPost("{uniqueNumber}")]
        public async Task<IActionResult> UploadTextDocument(int uniqueNumber, string content, DocumnetType type = DocumnetType.Text)
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

        /// <summary>
        /// Upload File as Document for a paper
        /// </summary>
        /// <param name="uniqueNumber">paper unique number</param>
        /// <param name="file">file</param>
        /// <response code="200">Upload Successfully</response>
        /// <response code="400">operation problem</response>
        [HttpPost("File/{uniqueNumber}")]
        public async Task<IActionResult> UploadFileDocument(int uniqueNumber, IFormFile file)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            // TODO: try catch should be added

            // TODO: validate memetype file should be added

            const int maxFileSize = 10 * 1024 * 1024;

            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("file", "please select a file.");
                return BadRequest(ModelState);
            }

            if (file.Length > maxFileSize)
            {
                ModelState.AddModelError("file", $"file size should'nt be more than {maxFileSize / (1024 * 1024)}MB");

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

            // TODO: encryption should be added

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

            var document = new Document(paper, Path.GetFileNameWithoutExtension(file.FileName), folderPathUrl, Path.GetExtension(file.FileName), (int)file.Length, DocumnetType.File);

            _context.Documents.Add(document);

            await _context.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// Delete paper directly
        /// </summary>
        /// <param name="uniqueNumber"></param>
        /// <response code="200">Delete Successfully</response>
        /// <response code="404">If the item is null</response>
        [HttpDelete("{uniqueNumber}")]
        public async Task<IActionResult> Delete(int uniqueNumber)
        {

            var paper = _context.Papers.FirstOrDefault(x => x.UniqueNumber == uniqueNumber);

            if (paper == null)
                return NotFound(new Exception(nameof(paper)).Message);

            var documents = _context.Documents.Where(x => x.PaperId == paper.Id).ToList();

            foreach (var item in documents)
            {
                if (item.Type == DocumnetType.File && !string.IsNullOrEmpty(item.FilePath))
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
