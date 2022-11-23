using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pokedex.Services.Pokemon;

namespace Pokedex.Controllers;

[ApiController]
[Route("[controller]")]
public class PokemonController : ControllerBase
{
    private readonly ILogger<PokemonController> _logger;
    private readonly IPokemonService _pokemonService;

    public PokemonController(ILogger<PokemonController> logger, IPokemonService pokemonService)
    {
        _logger = logger;
        _pokemonService = pokemonService;
    }

    [HttpGet(Name = "GetPokemon")]
    [Route("{name}")]
    public async Task<ActionResult<Pokemon>> Get(string name)
    {
        var pokemon = await _pokemonService.Find(name);

        if (pokemon == null) 
        {
            return NotFound();
        }

        return Ok(pokemon);
    }
    
    [HttpGet(Name = "GetTranslatedPokemon")]
    [Route("translated/{name}")]
    public async Task<ActionResult<Pokemon>> Translated(string name)
    {
        var pokemon = await _pokemonService.FindTranslated(name);

        if (pokemon == null) 
        {
            return NotFound();
        }

        return Ok(pokemon);
    }
}
