using Azure;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;

public class StudentEntity : ITableEntity
{
    public string PartitionKey { get; set; } = "Students";
    public string RowKey { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}

[Route("table")]
[ApiController]
public class TableController : ControllerBase
{
    private readonly TableClient _tableClient;

    public TableController(IConfiguration config)
    {
        _tableClient = new TableClient(config["AzureStorage:ConnectionString"], config["AzureStorage:TableName"]);
        _tableClient.CreateIfNotExists();
    }

    [HttpPost]
    public IActionResult AddStudent([FromBody] StudentEntity student)
    {
        student.RowKey = Guid.NewGuid().ToString();
        _tableClient.AddEntity(student);
        return Ok(student);
    }

    [HttpGet]
    public IActionResult GetStudents()
    {
        var students = _tableClient.Query<StudentEntity>().ToList();
        return Ok(students);
    }
}
