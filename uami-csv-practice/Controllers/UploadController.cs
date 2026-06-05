using CsvUploadApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/upload")]
public class UploadController : ControllerBase
{
    private readonly BlobStorageService _blobStorageService;

    public UploadController(BlobStorageService blobStorageService)
    {
        _blobStorageService = blobStorageService;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    [Authorize]
    public async Task<IActionResult> UploadCsv(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File is required.");

        if (!Path.GetExtension(file.FileName)
            .Equals(".csv", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Only CSV files are allowed.");
        }

        using var stream = file.OpenReadStream();

        var blobUrl = await _blobStorageService.UploadFileAsync(stream, file.FileName);

        return Ok(new
        {
            Message = "Upload successful",
            BlobUrl = blobUrl
        });
    }
}