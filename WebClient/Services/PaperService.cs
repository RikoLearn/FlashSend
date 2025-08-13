using Microsoft.AspNetCore.Components.Forms;

namespace WebClient.Services
{
    public class PaperService
    {
        private readonly HttpClient _http;

        public PaperService(HttpClient http) => _http = http;

        public async Task<int> CreatePaperAsync(int expireMinutes)
        {
            var res = await _http.PostAsync($"https://localhost:44375/api/v1/Papers/{expireMinutes}", null);
            res.EnsureSuccessStatusCode();
            var payload = await res.Content.ReadFromJsonAsync<CreatePaperResponse>();
            return payload!.UniqueNumber;
        }

        public async Task<bool> DeletePaperAsync(int uniqueNumber)
        {
            var res = await _http.DeleteAsync($"https://localhost:44375/api/v1/Documents/{uniqueNumber}");
            return res.IsSuccessStatusCode;
        }

        public async Task<List<DocumentDto>> GetPaperAsync(int uniqueNumber)
        {
            var res = await _http.GetAsync($"https://localhost:44375/api/v1/Papers/{uniqueNumber}");
            if (res.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
            res.EnsureSuccessStatusCode();
            var docs = await res.Content.ReadFromJsonAsync<List<DocumentDto>>();
            return docs ?? new();
        }

        public async Task<bool> PostTextAsync(int uniqueNumber, string text)
        {
            var url = $"https://localhost:44375/api/v1/Documents/{uniqueNumber}?content={Uri.EscapeDataString(text)}&type=1";
            var res = await _http.PostAsync(url, null);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> PostCodeAsync(int uniqueNumber, string code)
        {
            var url = $"https://localhost:44375/api/v1/Documents/{uniqueNumber}?content={Uri.EscapeDataString(code)}&type=2";
            var res = await _http.PostAsync(url, null);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> PostFileAsync(int uniqueNumber, IBrowserFile file)
        {
            var content = new MultipartFormDataContent();
            var streamContent = new StreamContent(file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024)); // 10MB
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
            content.Add(streamContent, "file", file.Name);

            var res = await _http.PostAsync($"https://localhost:44375/api/v1/Documents/File/{uniqueNumber}", content);
            return res.IsSuccessStatusCode;
        }
    }

    public class DocumentDto
    {
        public string Id { get; set; }            // بسته به خروجی API تغییر بده
        public DocumentType Type { get; set; }
        public string Name { get; set; }          // برای فایل
        public long? Size { get; set; }           // برای فایل (بایت)
        public string Content { get; set; }       // برای متن/کد
        public Language? Language { get; set; }   // برای کد
        public DateTime? UploadedAt { get; set; }
    }

    public class CreatePaperResponse
    {
        public int UniqueNumber { get; set; }
    }
    public enum DocumentType
    {
        Text = 1,
        Code = 2,
        File = 3
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
