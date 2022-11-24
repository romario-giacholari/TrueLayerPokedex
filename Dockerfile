FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

COPY *.sln .
COPY Pokedex/*.csproj ./Pokedex/
COPY Pokedex.Tests/*.csproj ./Pokedex.Tests/
RUN dotnet restore

COPY . .

RUN dotnet build
FROM build AS testrunner
WORKDIR /app/Pokedex.Tests
CMD ["dotnet", "test", "--logger:trx"]

FROM build AS test
WORKDIR /app/Pokedex.Tests
RUN dotnet test --logger:trx

FROM build AS publish
WORKDIR /app/Pokedex
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY --from=publish /app/Pokedex/out ./
EXPOSE 80
ENTRYPOINT ["dotnet", "Pokedex.dll"]