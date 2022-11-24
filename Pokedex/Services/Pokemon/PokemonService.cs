using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using Pokedex.Response.Pokemon;
using Pokedex.Services.FunTranslation;

namespace Pokedex.Services.Pokemon;

public class PokemonService: IPokemonService
{
    private const string En = "en";
    private readonly ILogger<PokemonService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IYodaTranslationService _yodaTranslationService;
    private readonly IShakespeareTranslationService _shakespeareTranslationService;
    
    public PokemonService(ILogger<PokemonService> logger, 
        IHttpClientFactory httpClientFactory, 
        IYodaTranslationService yodaTranslationService, 
        IShakespeareTranslationService shakespeareTranslationService)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _yodaTranslationService = yodaTranslationService;
        _shakespeareTranslationService = shakespeareTranslationService;
    }
    
    public async Task<Pokedex.Pokemon?> Find(string name)
    {
        var client = _httpClientFactory.CreateClient("Pokemon");
        var response = await client.GetAsync($"{name}");
        var stringContent = response.Content.ReadAsStringAsync().Result;

        if (string.IsNullOrEmpty(stringContent) || response.StatusCode != HttpStatusCode.OK)
        {
            _logger.LogWarning("Pokemon [{Name}] could not be found", name);
            
            return null;
        }
        var content = JsonNode.Parse(stringContent);

        var pokemonName = content!["name"]!.GetValue<string>();
        var descriptions = content!["flavor_text_entries"]!.Deserialize<List<Description>>();
        var description = "";
        var habitat = content!["habitat"]!?.Deserialize<Habitat>();
        var isLegendary = content!["is_legendary"]!.GetValue<bool>();

        if (descriptions?.Count > 0)
        {
            foreach (var d in descriptions)
            {
                if (d.language.name != En) continue;
                description = d.flavor_text;
                
                break;
            }
        }

        return new Pokedex.Pokemon
        {
            Name = pokemonName,
            Description = description,
            Habitat = habitat?.name,
            IsLegendary = isLegendary
        };
    }

    public async Task<Pokedex.Pokemon?> FindTranslated(string name)
    {
        var pokemon = await Find(name);

        if (pokemon == null) return null;
        if (pokemon.IsLegendary || pokemon.Habitat == "cave")
        {
            pokemon.Description = await _yodaTranslationService.Translate(pokemon.Description) ?? pokemon.Description;
        }
        else
        {
            pokemon.Description = await _shakespeareTranslationService.Translate(pokemon.Description) ?? pokemon.Description;
        }

        return pokemon;
    }
}