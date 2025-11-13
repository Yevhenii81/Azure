using Azure;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ====== Simple API Endpoints ======

// GET /api/hello
app.MapGet("/api/hello", () => "Hello from Azure!")
   .WithName("GetHello")
   .WithOpenApi();

// GET /api/time
app.MapGet("/api/time", () => new { ServerTime = DateTime.Now.ToString("HH:mm:ss") })
   .WithName("GetTime")
   .WithOpenApi();

// POST /api/sum
app.MapPost("/api/sum", (SumRequest request) => new { Sum = request.A + request.B })
   .WithName("PostSum")
   .WithOpenApi();

// ====== Azure Storage Settings ======
var connectionString = builder.Configuration["AzureStorage:ConnectionString"];
var blobContainerName = builder.Configuration["AzureStorage:BlobContainerName"];
var tableName = builder.Configuration["AzureStorage:TableName"];
var queueName = builder.Configuration["AzureStorage:QueueName"];

// --- Blob Storage ---

// GET список файлов
app.MapGet("/blob/files", async () =>
{
    var container = new BlobServiceClient(connectionString).GetBlobContainerClient(blobContainerName);
    var blobs = new List<string>();
    await foreach (var blob in container.GetBlobsAsync())
        blobs.Add(blob.Name);
    return blobs;
})
.WithName("ListFiles")
.WithOpenApi();

// POST загрузка файла через IFormFile
app.MapPost("/blob/upload", async (IFormFile file) =>
{
    if (file == null || file.Length == 0)
        return Results.BadRequest("File is missing");

    var container = new BlobServiceClient(connectionString).GetBlobContainerClient(blobContainerName);
    await container.CreateIfNotExistsAsync();

    var blobClient = container.GetBlobClient(file.FileName);
    await blobClient.UploadAsync(file.OpenReadStream(), true);

    return Results.Ok(new { file.FileName });
})
.WithName("UploadFile")
.WithOpenApi();

// GET скачать файл
app.MapGet("/blob/download/{fileName}", async (string fileName) =>
{
    var container = new BlobServiceClient(connectionString).GetBlobContainerClient(blobContainerName);
    var blobClient = container.GetBlobClient(fileName);

    if (!await blobClient.ExistsAsync())
        return Results.NotFound();

    var stream = await blobClient.OpenReadAsync();
    return Results.File(stream, "application/octet-stream", fileName);
})
.WithName("DownloadFile")
.WithOpenApi();

// --- Table Storage ---

// POST добавить студента
app.MapPost("/table/students", async (TableStudentEntity student) =>
{
    var tableClient = new TableClient(connectionString, tableName);
    await tableClient.CreateIfNotExistsAsync();

    student.PartitionKey = "Students";
    student.RowKey = Guid.NewGuid().ToString();

    await tableClient.AddEntityAsync(student);
    return Results.Ok(student);
})
.WithName("AddStudent")
.WithOpenApi();

// GET список студентов
app.MapGet("/table/students", async () =>
{
    var tableClient = new TableClient(connectionString, tableName);
    var students = tableClient.Query<TableStudentEntity>().ToList();
    return Results.Ok(students);
})
.WithName("GetStudents")
.WithOpenApi();

// --- Queue Storage ---

// POST отправить сообщение
app.MapPost("/queue/send", async (string message) =>
{
    var queueClient = new QueueClient(connectionString, queueName);
    await queueClient.CreateIfNotExistsAsync();
    await queueClient.SendMessageAsync(message);
    return Results.Ok(new { message });
})
.WithName("SendQueueMessage")
.WithOpenApi();

// GET получить сообщение
app.MapGet("/queue/receive", async () =>
{
    var queueClient = new QueueClient(connectionString, queueName);
    await queueClient.CreateIfNotExistsAsync();
    var msg = await queueClient.ReceiveMessageAsync();
    if (msg?.Value != null)
    {
        await queueClient.DeleteMessageAsync(msg.Value.MessageId, msg.Value.PopReceipt);
        return Results.Ok(msg.Value.MessageText);
    }
    return Results.NotFound("No messages in queue");
})
.WithName("ReceiveQueueMessage")
.WithOpenApi();

app.Run();

// ====== Классы ======
record SumRequest(int A, int B);

public class TableStudentEntity : ITableEntity
{
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}
