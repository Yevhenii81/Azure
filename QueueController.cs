using Azure.Storage.Queues;
using Microsoft.AspNetCore.Mvc;

[Route("queue")]
[ApiController]
public class QueueController : ControllerBase
{
    private readonly QueueClient _queueClient;

    public QueueController(IConfiguration config)
    {
        _queueClient = new QueueClient(config["AzureStorage:ConnectionString"], config["AzureStorage:QueueName"]);
        _queueClient.CreateIfNotExists();
    }

    [HttpPost("send")]
    public IActionResult SendMessage([FromBody] string message)
    {
        _queueClient.SendMessage(message);
        return Ok(new { message });
    }

    [HttpGet("receive")]
    public IActionResult ReceiveMessage()
    {
        var msg = _queueClient.ReceiveMessage();
        if (msg?.Value != null)
        {
            _queueClient.DeleteMessage(msg.Value.MessageId, msg.Value.PopReceipt);
            return Ok(msg.Value.MessageText);
        }
        return NotFound("No messages in queue");
    }
}
