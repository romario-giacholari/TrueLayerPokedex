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
    private readonly IConfiguration _config;
    
    public FunTranslationService(IConfiguration configuration)
    {
        _config = configuration;
    }
    
    public async Task<string?> Translate(string text, string translationType)
    {
        var endpoint = _config[$"FunTranslations:{translationType}:Endpoint"];
        var content = new
        {
            text = text
        };
        var json = JsonSerializer.Serialize(content);
        var data = new StringContent(json, Encoding.UTF8, "application/json");
        var client = new HttpClient();
        var response = client.PostAsync(endpoint, data).Result;
        var responseContent = response.Content.ReadAsStringAsync().Result;

        if (string.IsNullOrEmpty(responseContent) || response.StatusCode != HttpStatusCode.OK) return null;
        var node = JsonNode.Parse(responseContent);
        var deserializedContent = node!["contents"]!.Deserialize<Translated>();

        return deserializedContent?.translated;

    }
}