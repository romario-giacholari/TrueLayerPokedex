using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Moq;
using Pokedex.Services.FunTranslation;
using Pokedex.Services.Pokemon;
using RichardSzalay.MockHttp;

namespace Pokedex.Tests.Pokemon;

public class PokemonServiceTest
{
    private readonly PokemonService _pokemonService;
    private readonly Mock<ILogger<PokemonService>> _loggerMock = new();
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock = new();
    private readonly Mock<IYodaTranslationService> _yodaTranslationServiceMock = new();
    private readonly Mock<IShakespeareTranslationService> _shakespeareTranslationServiceMock = new();
    private readonly MockHttpMessageHandler _handlerMock = new MockHttpMessageHandler();

    [Fact]
    public async void Test_Find_Returns_A_Pokemon_When_It_Exists()
    {
        // Arrange
        _handlerMock.When("https://pokeapi.co/api/v2/pokemon-species/pikachu")
            .Respond(HttpStatusCode.OK, JsonContent.Create(new
            {
                flavor_text_entries = new List<object>
                {
                    new
                    {
                        flavor_text = "some description here..",
                        language = new
                        {
                            name = "en",
                            url = ""
                        }
                    }
                },
                habitat = new
                {
                    name = "a habitat",
                    url = ""
                },
                is_legendary = false,
                name = "pikachu"
            }));
        
        _httpClientFactoryMock.Setup(x => x.CreateClient("Pokemon"))
            .Returns(new HttpClient(_handlerMock)
            {
                BaseAddress = new Uri("https://pokeapi.co/api/v2/pokemon-species/")
            });
        
        var pokemonService = new PokemonService(_loggerMock.Object, 
            _httpClientFactoryMock.Object, 
            _yodaTranslationServiceMock.Object, 
            _shakespeareTranslationServiceMock.Object);
        
        // Act
        var pokemon = await pokemonService.Find("pikachu");
        
        // Assert
        Assert.NotNull(pokemon);
        Assert.Equal("pikachu", pokemon?.Name);
        Assert.Equal("some description here..", pokemon?.Description);
        Assert.Equal("a habitat", pokemon?.Habitat);
        Assert.False(pokemon?.IsLegendary);
    }
    
    [Fact]
    public async void Test_It_Returns_Null_When_The_Request_Fails()
    {
        // Arrange
        var pokemonName = "unknown";
        _handlerMock.When($"https://pokeapi.co/api/v2/pokemon-species/{pokemonName}")
            .Respond(HttpStatusCode.NotFound);
        
        _httpClientFactoryMock.Setup(x => x.CreateClient("Pokemon"))
            .Returns(new HttpClient(_handlerMock)
            {
                BaseAddress = new Uri("https://pokeapi.co/api/v2/pokemon-species/")
            });

        var pokemonService = new PokemonService(_loggerMock.Object, 
            _httpClientFactoryMock.Object, 
            _yodaTranslationServiceMock.Object, 
            _shakespeareTranslationServiceMock.Object);
        
        // Act
        var pokemon = await pokemonService.Find(pokemonName);
        
        // Assert
        Assert.Null(pokemon);
    }
    
    [Fact]
    public async void Test_It_Yoda_Translates_The_Description()
    {
        // Arrange
        var pokemonName = "mewtwo";
        _handlerMock.When($"https://pokeapi.co/api/v2/pokemon-species/{pokemonName}")
            .Respond(HttpStatusCode.OK, JsonContent.Create(new
            {
                flavor_text_entries = new List<object>
                {
                    new
                    {
                        flavor_text = "some description here..",
                        language = new
                        {
                            name = "en",
                            url = ""
                        }
                    }
                },
                habitat = new
                {
                    name = "cave",
                    url = ""
                },
                is_legendary = true,
                name = pokemonName
            }));
        
        _httpClientFactoryMock.Setup(x => x.CreateClient("Pokemon"))
            .Returns(new HttpClient(_handlerMock)
            {
                BaseAddress = new Uri("https://pokeapi.co/api/v2/pokemon-species/")
            });

        _yodaTranslationServiceMock.Setup(x => x.Translate(It.IsAny<string>())).Returns(Task.FromResult("Yoda"));
        _shakespeareTranslationServiceMock.Setup(x => x.Translate(It.IsAny<string>())).Returns(Task.FromResult("Shakespeare"));
        
        var pokemonService = new PokemonService(_loggerMock.Object, 
            _httpClientFactoryMock.Object, 
            _yodaTranslationServiceMock.Object, 
            _shakespeareTranslationServiceMock.Object);
        
        // Act
        var pokemon = await pokemonService.FindTranslated(pokemonName);
        
        // Assert
        Assert.Equal("Yoda", pokemon?.Description);
    }
    
    [Fact]
    public async void Test_It_Shakespeare_Translates_The_Description()
    {
        // Arrange
        var pokemonName = "bulbasaur";
        _handlerMock.When($"https://pokeapi.co/api/v2/pokemon-species/{pokemonName}")
            .Respond(HttpStatusCode.OK, JsonContent.Create(new
            {
                flavor_text_entries = new List<object>
                {
                    new
                    {
                        flavor_text = "some description here..",
                        language = new
                        {
                            name = "en",
                            url = ""
                        }
                    }
                },
                habitat = new
                {
                    name = "",
                    url = ""
                },
                is_legendary = false,
                name = pokemonName
            }));
        
        _httpClientFactoryMock.Setup(x => x.CreateClient("Pokemon"))
            .Returns(new HttpClient(_handlerMock)
            {
                BaseAddress = new Uri("https://pokeapi.co/api/v2/pokemon-species/")
            });

        _yodaTranslationServiceMock.Setup(x => x.Translate(It.IsAny<string>())).Returns(Task.FromResult("Yoda"));
        _shakespeareTranslationServiceMock.Setup(x => x.Translate(It.IsAny<string>())).Returns(Task.FromResult("Shakespeare"));
        
        var pokemonService = new PokemonService(_loggerMock.Object, 
            _httpClientFactoryMock.Object, 
            _yodaTranslationServiceMock.Object, 
            _shakespeareTranslationServiceMock.Object);
        
        // Act
        var pokemon = await pokemonService.FindTranslated(pokemonName);
        
        // Assert
        Assert.Equal("Shakespeare", pokemon?.Description);
    }
    
    [Fact]
    public async void Test_It_FallsBack_To_Original_Description_When_It_Cannot_Translate()
    {
        // Arrange
        var pokemonName = "charizard";
        _handlerMock.When($"https://pokeapi.co/api/v2/pokemon-species/{pokemonName}")
            .Respond(HttpStatusCode.OK, JsonContent.Create(new
            {
                flavor_text_entries = new List<object>
                {
                    new
                    {
                        flavor_text = "original description",
                        language = new
                        {
                            name = "en",
                            url = ""
                        }
                    }
                },
                habitat = new
                {
                    name = "cave",
                    url = ""
                },
                is_legendary = true,
                name = pokemonName
            }));
        
        _httpClientFactoryMock.Setup(x => x.CreateClient("Pokemon"))
            .Returns(new HttpClient(_handlerMock)
            {
                BaseAddress = new Uri("https://pokeapi.co/api/v2/pokemon-species/")
            });

        _yodaTranslationServiceMock.Setup(x => x.Translate(It.IsAny<string>())).Returns(Task.FromResult((string)null));
        _shakespeareTranslationServiceMock.Setup(x => x.Translate(It.IsAny<string>())).Returns(Task.FromResult((string)null));
        
        var pokemonService = new PokemonService(_loggerMock.Object, 
            _httpClientFactoryMock.Object, 
            _yodaTranslationServiceMock.Object, 
            _shakespeareTranslationServiceMock.Object);
        
        // Act
        var pokemon = await pokemonService.FindTranslated(pokemonName);
        
        // Assert
        Assert.Equal("original description", pokemon?.Description);
    }
}