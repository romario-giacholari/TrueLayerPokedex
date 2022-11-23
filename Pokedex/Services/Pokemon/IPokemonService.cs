using System.Threading.Tasks;

namespace Pokedex.Services.Pokemon;

public interface IPokemonService
{
    public Task<Pokedex.Pokemon?> Find(string name);
    public Task<Pokedex.Pokemon?> FindTranslated(string name);
}