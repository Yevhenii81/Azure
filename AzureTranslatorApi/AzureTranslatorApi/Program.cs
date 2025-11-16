using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        var key = "3EJywuhTVyJEDNDcjZM3WSruMrUoV9NhwsxaYAJLscMli7zQbnpkJQQJ99BKACYeBjFXJ3w3AAAbACOGxqhh";
        var endpoint = "https://yevheniitranslator.cognitiveservices.azure.com/";
        var region = "eastus";

        var translator = new TranslatorService(key, endpoint, region);

        while (true)
        {
            Console.Write("¬ведите текст дл€ перевода (или 'exit' дл€ выхода):\n> ");
            var text = Console.ReadLine();
            if (text.ToLower() == "exit") break;

            Console.Write("¬ведите €зык перевода (например, 'en' или 'fr'):\n> ");
            var lang = Console.ReadLine();

            var translation = await translator.TranslateTextAsync(text, lang);
            Console.WriteLine($"ѕеревод: {translation}\n");
        }
    }
}
