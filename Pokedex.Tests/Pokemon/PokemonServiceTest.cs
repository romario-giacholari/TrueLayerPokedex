using Pokedex.Services.Pokemon;

namespace Pokedex.Tests.Pokemon;

public class PokemonServiceTest
{
    private readonly PokemonService _pokemonService;
    // private readonly Mock<IHttpClientFactory> _httpClientFactory = new();
    // private readonly Mock<ILogger<PokemonService>> _logger = new();

    public PokemonServiceTest()
    {
        // _pokemonService = new PokemonService();
    }
        
    [Fact]
    public void Test_Find_Returns_A_Pokemon_When_It_Exists()
    {
        // Arrange
        
        // Act
        
        // Assert
        Assert.True(true);
    }
}