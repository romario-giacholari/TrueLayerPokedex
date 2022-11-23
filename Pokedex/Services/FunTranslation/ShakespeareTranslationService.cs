using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Pokedex.Response.FunTranslations;

namespace Pokedex.Services.FunTranslation;

public class ShakespeareTranslationService: IShakespeareTranslationService
{
    private readonly ILogger<ShakespeareTranslationService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public ShakespeareTranslationService(ILogger<ShakespeareTranslationService> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }
    
    public async Task<string?> Translate(string text)
    {
        var content = new
        {
            text = text
        };
        var json = JsonSerializer.Serialize(content);
        var data = new StringContent(json, Encoding.UTF8, "application/json");
        var client = _httpClientFactory.CreateClient("Shakespeare");
        var response = client.PostAsync(client.BaseAddress, data).Result;
        var responseContent = response.Content.ReadAsStringAsync().Result;

        if (string.IsNullOrEmpty(responseContent) || response.StatusCode != HttpStatusCode.OK)
        {
            _logger.LogWarning("Text [{Text}] could not be translated", text);
            
            return null;
        }
        
        var node = JsonNode.Parse(responseContent);
        var deserializedContent = node!["contents"]!.Deserialize<Translated>();

        return deserializedContent?.translated;
    }
}