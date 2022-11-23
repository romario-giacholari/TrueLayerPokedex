using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using Pokedex.Response.Pokemon;
using Pokedex.Services.FunTranslation;

namespace Pokedex.Services.Pokemon;

public class PokemonService: IPokemonService
{
    private const string En = "en";
    private readonly string _endpoint;
    private readonly IFunTranslationService _funTranslationService;
    
    public PokemonService(IConfiguration configuration, IFunTranslationService funTranslationService)
    {
        _endpoint = configuration["PokemonService:Endpoint"];
        _funTranslationService = funTranslationService;
    }
    
    public async Task<Pokedex.Pokemon?> Find(string name)
    {
        var uri = $"{_endpoint}/{name}";
        var client = new HttpClient();
        var response = await client.GetAsync(uri);
        var stringContent = response.Content.ReadAsStringAsync().Result;

        if (string.IsNullOrEmpty(stringContent) || response.StatusCode != HttpStatusCode.OK) return null;
        var content = JsonNode.Parse(stringContent);

        var pokemonName = content!["name"]!?.GetValue<string>();
        var descriptions = content!["flavor_text_entries"]!?.Deserialize<List<Description>>();
        var description = "";
        var habitat = content!["habitat"]!?.Deserialize<Habitat>();
        var isLegendary = content!["is_legendary"]!.GetValue<bool>();

        foreach (var d in descriptions)
        {
            if (d.language.name != En) continue;
            description = d.flavor_text;
                
            break;
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

        if (pokemon == null) return pokemon;
        if (pokemon.IsLegendary || pokemon.Habitat == "cave")
        {
            pokemon.Description = await _funTranslationService.Translate(pokemon.Description, "Yoda") ?? pokemon.Description;
        }
        else
        {
            pokemon.Description = await _funTranslationService.Translate(pokemon.Description, "Shakespeare") ?? pokemon.Description;
        }

        return pokemon;
    }
}