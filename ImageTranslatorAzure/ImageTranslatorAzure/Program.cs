using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        var visionKey = "4vfISrjT82gVt7OIVOKHfqb68lQ4CdImg9gXN09GFS3Kf7AymGNoJQQJ99BKACYeBjFXJ3w3AAAFACOGBc3G";
        var visionEndpoint = "https://imagetranslator.cognitiveservices.azure.com/";

        var translatorKey = "3EJywuhTVyJEDNDcjZM3WSruMrUoV9NhwsxaYAJLscMli7zQbnpkJQQJ99BKACYeBjFXJ3w3AAAbACOGxqhh";
        var region = "eastus";
        var translatorEndpoint = "https://api.cognitive.microsofttranslator.com/";

        var ocr = new OcrService(visionEndpoint, visionKey);
        var translator = new TranslatorService(translatorKey, region, translatorEndpoint);

        Console.WriteLine("Введите полный путь к изображению (например, C:\\Users\\major\\source\\repos\\foto_test\\image.jpg):");
        var imagePath = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(imagePath))
        {
            Console.WriteLine("Путь не введён. Выход.");
            return;
        }

        try
        {
            var original = await ocr.ExtractTextAsync(imagePath);
            Console.WriteLine("\nТекст с изображения:\n" + original);

            // Перевод на английский
            var translated = await translator.TranslateAsync(original, "en");
            Console.WriteLine("\nПеревод:\n" + translated);
        }
        catch (Exception ex)
        {
            Console.WriteLine("\nПроизошла ошибка: " + ex.Message);
        }

        // Задержка, чтобы консоль не закрылась
        Console.WriteLine("\nНажмите Enter для выхода...");
        Console.ReadLine();
    }
}
