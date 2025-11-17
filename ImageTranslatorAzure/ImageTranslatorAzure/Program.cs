using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        var visionKey = "";
        var visionEndpoint = "";

        var translatorKey = "";
        var region = "";
        var translatorEndpoint = "";

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

