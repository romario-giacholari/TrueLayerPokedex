using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Moq;
using Pokedex.Services.FunTranslation;
using RichardSzalay.MockHttp;

namespace Pokedex.Tests;

public class ShakespeareTranslationServiceTest
{
    private readonly ShakespeareTranslationService _shakespeareTranslationService;
    private readonly Mock<ILogger<ShakespeareTranslationService>> _loggerMock = new();
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock = new();
    private readonly MockHttpMessageHandler _handlerMock = new MockHttpMessageHandler();

    public ShakespeareTranslationServiceTest()
    {
        _shakespeareTranslationService = new ShakespeareTranslationService(_loggerMock.Object, _httpClientFactoryMock.Object);
    }
        
    [Fact]
    public async void Test_It_Translates_Text()
    {
        // Arrange
        var text = "This is great";
        var expectedTranslated = "This is most wondrous";
        _handlerMock.When("https://api.funtranslations.com/translate/shakespeare.json/")
            .Respond(HttpStatusCode.OK, JsonContent.Create(new
            {
                success = new
                {
                  total = 1  
                },
                contents = new
                {
                    translated = expectedTranslated,
                    text = text,
                    translation = "shakespeare"
                }
            }));
        
        _httpClientFactoryMock.Setup(x => x.CreateClient("Shakespeare"))
            .Returns(new HttpClient(_handlerMock)
            {
                BaseAddress = new Uri("https://api.funtranslations.com/translate/shakespeare.json/")
            });
        
        // Act
        var translated = await _shakespeareTranslationService.Translate(text);
        
        // Assert
        Assert.Equal(expectedTranslated, translated);
    }
    
    [Fact]
    public async void Test_It_Returns_Null_When_The_Request_Fails()
    {
        // Arrange
        _handlerMock.When("https://api.funtranslations.com/translate/shakespeare.json/")
            .Respond(HttpStatusCode.UnprocessableEntity);
        
        _httpClientFactoryMock.Setup(x => x.CreateClient("Shakespeare"))
            .Returns(new HttpClient(_handlerMock)
            {
                BaseAddress = new Uri("https://api.funtranslations.com/translate/shakespeare.json/")
            });
        
        // Act
        var translated = await _shakespeareTranslationService.Translate("This is great");
        
        // Assert
        Assert.Null(translated);
    }
    
    [Fact]
    public async void Test_It_Returns_Null_When_The_Response_Has_No_Content()
    {
        // Arrange
        _handlerMock.When("https://api.funtranslations.com/translate/shakespeare.json/")
            .Respond(HttpStatusCode.OK);
        
        _httpClientFactoryMock.Setup(x => x.CreateClient("Shakespeare"))
            .Returns(new HttpClient(_handlerMock)
            {
                BaseAddress = new Uri("https://api.funtranslations.com/translate/shakespeare.json/")
            });
        
        // Act
        var translated = await _shakespeareTranslationService.Translate("This is great");
        
        // Assert
        Assert.Null(translated);
    }
}