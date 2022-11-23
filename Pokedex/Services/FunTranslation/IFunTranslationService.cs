namespace Pokedex.Services.FunTranslation;

public interface IFunTranslationService
{
    public Task<string?> Translate(string text, string translationType);
}