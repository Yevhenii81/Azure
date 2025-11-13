using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;

[Route("blob")]
[ApiController]
public class BlobController : ControllerBase
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;

    public BlobController(IConfiguration config)
    {
        _blobServiceClient = new BlobServiceClient(config["AzureStorage:ConnectionString"]);
        _containerName = config["AzureStorage:BlobContainerName"];
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        var container = _blobServiceClient.GetBlobContainerClient(_containerName);
        await container.CreateIfNotExistsAsync();
        var blobClient = container.GetBlobClient(file.FileName);
        await blobClient.UploadAsync(file.OpenReadStream(), true);
        return Ok(new { file.FileName });
    }

    [HttpGet("files")]
    public async Task<IActionResult> ListFiles()
    {
        var container = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobs = new List<string>();
        await foreach (var blob in container.GetBlobsAsync())
            blobs.Add(blob.Name);
        return Ok(blobs);
    }

    [HttpGet("download/{fileName}")]
    public async Task<IActionResult> Download(string fileName)
    {
        var container = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = container.GetBlobClient(fileName);

        if (!await blobClient.ExistsAsync())
            return NotFound();

        var stream = await blobClient.OpenReadAsync();
        return File(stream, "application/octet-stream", fileName);
    }
}
