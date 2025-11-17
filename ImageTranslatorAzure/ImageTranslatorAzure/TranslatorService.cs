using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

public class TranslatorService
{
    private readonly HttpClient _client;
    private readonly string _endpoint;

    public TranslatorService(string key, string region, string endpoint)
    {
        _endpoint = endpoint;
        _client = new HttpClient();
        _client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);
        _client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Region", region);
    }

    public async Task<string> TranslateAsync(string text, string to = "uk")
    {
        var url = $"{_endpoint}/translate?api-version=3.0&to={to}";
        var body = new object[] { new { Text = text } };

        var response = await _client.PostAsJsonAsync(url, body);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        // Парсим Json вручную
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        if (root.GetArrayLength() == 0)
            return "";

        var translations = root[0].GetProperty("translations");
        if (translations.GetArrayLength() == 0)
            return "";

        var translatedText = translations[0].GetProperty("text").GetString();
        return translatedText ?? "";
    }
}
