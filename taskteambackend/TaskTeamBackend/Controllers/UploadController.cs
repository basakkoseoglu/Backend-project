using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TaskTeamBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UploadController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<UploadController> _logger;

    public UploadController(IWebHostEnvironment env, ILogger<UploadController> logger)
    {
        _env = env;
        _logger = logger;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
                return BadRequest("Dosya seçilmedi.");

            // Dosya boyutu kontrolü (örnek: max 10MB)
            if (file.Length > 10 * 1024 * 1024)
                return BadRequest("Dosya boyutu 10MB'dan büyük olamaz.");

            var uploadPath = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(uploadPath, fileName);

            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            _logger.LogInformation("Dosya yüklendi: {FileName}, Boyut: {Size} bytes", fileName, file.Length);

            return Ok(new
            {
                FileName = fileName,
                OriginalName = file.FileName,
                Size = file.Length,
                Path = $"/uploads/{fileName}"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Dosya yüklenirken hata oluştu");
            return StatusCode(500, "Dosya yüklenirken bir hata oluştu.");
        }
    }

    [HttpGet]
    public IActionResult GetFiles()
    {
        try
        {
            var uploadPath = Path.Combine(_env.WebRootPath, "uploads");

            if (!Directory.Exists(uploadPath))
                return Ok(new List<object>());

            var files = Directory.GetFiles(uploadPath)
                .Select(f => new FileInfo(f))
                .Select(fi => new
                {
                    FileName = fi.Name,
                    OriginalName = fi.Name.Substring(fi.Name.IndexOf('_') + 1),
                    Size = fi.Length,
                    SizeFormatted = FormatFileSize(fi.Length),
                    UploadDate = fi.CreationTime,
                    Path = $"/uploads/{fi.Name}"
                })
                .OrderByDescending(f => f.UploadDate)
                .ToList();

            return Ok(files);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Dosyalar listelenirken hata oluştu");
            return StatusCode(500, "Dosyalar listelenirken bir hata oluştu.");
        }
    }

    [HttpGet("{fileName}")]
    public async Task<IActionResult> DownloadFile(string fileName)
    {
        try
        {
            // Path traversal saldırısına karşı koruma
            if (fileName.Contains("..") || fileName.Contains("/") || fileName.Contains("\\"))
                return BadRequest("Geçersiz dosya adı.");

            var uploadPath = Path.Combine(_env.WebRootPath, "uploads");
            var filePath = Path.Combine(uploadPath, fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound("Dosya bulunamadı.");

            var memory = new MemoryStream();
            await using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            var contentType = GetContentType(fileName);
            var originalFileName = fileName.Contains('_') 
                ? fileName.Substring(fileName.IndexOf('_') + 1) 
                : fileName;

            _logger.LogInformation("Dosya indirildi: {FileName}", fileName);

            return File(memory, contentType, originalFileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Dosya indirilirken hata oluştu: {FileName}", fileName);
            return StatusCode(500, "Dosya indirilirken bir hata oluştu.");
        }
    }

    [HttpDelete("{fileName}")]
    public IActionResult DeleteFile(string fileName)
    {
        try
        {
            // Path traversal saldırısına karşı koruma
            if (fileName.Contains("..") || fileName.Contains("/") || fileName.Contains("\\"))
                return BadRequest("Geçersiz dosya adı.");

            var uploadPath = Path.Combine(_env.WebRootPath, "uploads");
            var filePath = Path.Combine(uploadPath, fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound("Dosya bulunamadı.");

            System.IO.File.Delete(filePath);

            _logger.LogInformation("Dosya silindi: {FileName}", fileName);

            return Ok(new { Message = "Dosya başarıyla silindi." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Dosya silinirken hata oluştu: {FileName}", fileName);
            return StatusCode(500, "Dosya silinirken bir hata oluştu.");
        }
    }

    private static string GetContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".pdf" => "application/pdf",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".txt" => "text/plain",
            ".zip" => "application/zip",
            _ => "application/octet-stream"
        };
    }

    private static string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB" };
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len /= 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }
}