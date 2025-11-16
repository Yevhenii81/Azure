using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("[controller]")]
public class TranslatorController : Controller
{
    private readonly TranslatorService _translator;
    private static List<string> _history = new List<string>();

    public TranslatorController()
    {
        var key = "3EJywuhTVyJEDNDcjZM3WSruMrUoV9NhwsxaYAJLscMli7zQbnpkJQQJ99BKACYeBjFXJ3w3AAAbACOGxqhh";
        var endpoint = "https://yevheniitranslator.cognitiveservices.azure.com/";
        var region = "eastus";

        _translator = new TranslatorService(key, endpoint, region);
    }

    [HttpGet]
    public IActionResult Index()
    {
        return Content(@"
            <html>
            <body>
                <h2>Azure Translator</h2>
                <form method='post'>
                    <input type='text' name='text' placeholder='Введите текст' />
                    <input type='text' name='lang' placeholder='Язык (en, fr, etc)' />
                    <button type='submit'>Перевести</button>
                </form>
            </body>
            </html>", "text/html");
    }

    [HttpPost]
    public async Task<IActionResult> Translate([FromForm] string text, [FromForm] string lang)
    {
        if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(lang))
            return BadRequest("Введите текст и язык перевода.");

        var translation = await _translator.TranslateTextAsync(text, lang);
        _history.Add($"[{lang}] {text} → {translation}");

        var historyHtml = string.Join("<br>", _history);

        return Content($@"
            <html>
            <body>
                <h2>Azure Translator</h2>
                <form method='post'>
                    <input type='text' name='text' placeholder='Введите текст' />
                    <input type='text' name='lang' placeholder='Язык (en, fr, etc)' />
                    <button type='submit'>Перевести</button>
                </form>
                <h3>Перевод:</h3>
                <p>{translation}</p>
                <h3>История:</h3>
                <p>{historyHtml}</p>
            </body>
            </html>", "text/html");
    }
}
