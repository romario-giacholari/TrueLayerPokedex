using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Pokedex.Response.FunTranslations;

namespace Pokedex.Services.FunTranslation;

public class FunTranslationService: IFunTranslationService
{
    private readonly ILogger<FunTranslationService> _logger;
    private readonly IConfiguration _config;

    public FunTranslationService( ILogger<FunTranslationService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _config = configuration;
    }
    
    public async Task<string?> Translate(string text, string translationType)
    {
        var endpoint = _config[$"FunTranslations:{translationType}:Endpoint"];

        if (string.IsNullOrEmpty(endpoint))
        {
            _logger.LogWarning("The endpoint could not be found or is empty at [FunTranslations:{TranslationType}:Endpoint]", translationType);
            
            return null;
        }
        
        var content = new
        {
            text = text
        };
        var json = JsonSerializer.Serialize(content);
        var data = new StringContent(json, Encoding.UTF8, "application/json");
        var client = new HttpClient();
        var response = client.PostAsync(endpoint, data).Result;
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