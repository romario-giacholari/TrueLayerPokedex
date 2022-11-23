namespace Pokedex.Services.FunTranslation;

public interface IShakespeareTranslationService
{
    public Task<string?> Translate(string text);
}