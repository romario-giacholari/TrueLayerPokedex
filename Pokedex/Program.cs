using Pokedex.Services.FunTranslation;
using Pokedex.Services.Pokemon;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add services to the container.
builder.Services.AddHttpClient("Pokemon", httpClient =>
{
    httpClient.BaseAddress = new Uri(config.GetValue<string>("PokemonService:Endpoint"));
});

builder.Services.AddHttpClient("Yoda", httpClient =>
{
    httpClient.BaseAddress = new Uri(config.GetValue<string>("FunTranslations:Yoda:Endpoint"));
});

builder.Services.AddHttpClient("Shakespeare", httpClient =>
{
    httpClient.BaseAddress = new Uri(config.GetValue<string>("FunTranslations:Shakespeare:Endpoint"));
});

builder.Services.AddScoped<IPokemonService, PokemonService>();
builder.Services.AddScoped<IYodaTranslationService, YodaTranslationService>();
builder.Services.AddScoped<IShakespeareTranslationService, ShakespeareTranslationService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
