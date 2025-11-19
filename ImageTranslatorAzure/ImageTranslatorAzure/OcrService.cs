using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.AI.Vision.ImageAnalysis;

public class OcrService
{
    private readonly ImageAnalysisClient _client;

    public OcrService(string endpoint, string key)
    {
        _client = new ImageAnalysisClient(new Uri(endpoint), new AzureKeyCredential(key));
    }

    public async Task<string> ExtractTextAsync(string imagePath)
    {
        if (!File.Exists(imagePath))
            throw new FileNotFoundException("Image not found", imagePath);

        await using var stream = File.OpenRead(imagePath);

        var response = await _client.AnalyzeAsync(
            BinaryData.FromStream(stream),
            VisualFeatures.Read,
            new ImageAnalysisOptions());

        var result = response.Value;

        var read = result.Read;
        if (read == null || read.Blocks == null || read.Blocks.Count == 0)
            return "";

        var lines = read.Blocks
            .SelectMany(block => block.Lines)
            .Select(line => line.Text); 
        return string.Join(Environment.NewLine, lines);
    }
}

