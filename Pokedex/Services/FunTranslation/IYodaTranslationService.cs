namespace Pokedex.Services.FunTranslation;

public interface IYodaTranslationService
{
    public Task<string?> Translate(string text);
}