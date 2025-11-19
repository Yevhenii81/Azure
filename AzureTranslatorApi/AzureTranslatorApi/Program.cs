using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        var key = "";
        var endpoint = "";
        var region = "eastus";

        var translator = new TranslatorService(key, endpoint, region);

        while (true)
        {
            Console.Write("Ââåäèòå òåêñò äëÿ ïåðåâîäà (èëè 'exit' äëÿ âûõîäà):\n> ");
            var text = Console.ReadLine();
            if (text.ToLower() == "exit") break;

            Console.Write("Ââåäèòå ÿçûê ïåðåâîäà (íàïðèìåð, 'en' èëè 'fr'):\n> ");
            var lang = Console.ReadLine();

            var translation = await translator.TranslateTextAsync(text, lang);
            Console.WriteLine($"Ïåðåâîä: {translation}\n");
        }
    }
}

