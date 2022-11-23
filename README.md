# Pokedex

## Prerequisites
- Install docker https://docs.docker.com/get-docker/

## How to run the application?
1. Start docker
2. `cd` into `Pokedex`
3. Then build it by running the following command in your terminal `docker build -t pokedex .`
4. Next start it up by running `docker run -it --rm -p 3000:80 --name pokedexcontainer pokedex`
5. Finally in your browser, go to 
```
http://localhost:3000/pokemon/pokemon-name-here
```
```
http://localhost:3000/pokemon/translated/pokemon-name-here
```
Alternatively, you can use **curl** to call the API (in your terminal paste the following and hit enter):

```
curl http://localhost:3000/pokemon/pokemon-name-here
```
```
curl http://localhost:3000/pokemon/translated/pokemon-name-here
```

## Things to consider when and if the code reaches production
- Integrate to an alerting / monitoring service such as Datadog in order to capture logs
- Use GitHub Actions to trigger tests when pushing / requesting a PR
- Use a service to scan the codebase for vulnerabilities