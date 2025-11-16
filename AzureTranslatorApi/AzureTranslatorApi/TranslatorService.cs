using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class TranslatorService
{
    private readonly string _key;
    private readonly string _endpoint;
    private readonly string _region;
    private readonly HttpClient _client;

    public TranslatorService(string key, string endpoint, string region)
    {
        _key = key;
        _endpoint = endpoint;
        _region = region;
        _client = new HttpClient();
        _client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _key);
        _client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Region", _region);
    }

    // Вот этот метод и делает перевод
    public async Task<string> TranslateTextAsync(string text, string toLanguage)
    {
        // Обрати внимание на правильный путь
        var route = $"/translator/text/v3.0/translate?to={toLanguage}";
        var requestBody = new object[] { new { Text = text } };
        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        var response = await _client.PostAsync(_endpoint + route, content);
        var result = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            return $"Ошибка API: {result}";

        using var doc = JsonDocument.Parse(result);
        var translation = doc.RootElement[0].GetProperty("translations")[0].GetProperty("text").GetString();
        return translation;
    }
}
