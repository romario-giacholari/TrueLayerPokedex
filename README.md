# Pokedex

## Prerequisites
- Install docker https://docs.docker.com/get-docker/

## How to run the application?
1. Start docker
2. Then build it by running the following command in your terminal 
```
docker build -t pokedex:latest .
```
3. Next start it up by running 
```
docker run --rm -it -p 5000:80 pokedex:latest
```
4. Finally in your browser, go to 
```
http://localhost:5000/pokemon/pokemon-name-here
```
```
http://localhost:5000/pokemon/translated/pokemon-name-here
```
Alternatively, you can use **curl** to call the API (in your terminal paste the following and hit enter):

```
curl http://localhost:5000/pokemon/pokemon-name-here
```
```
curl http://localhost:5000/pokemon/translated/pokemon-name-here
```

## How to trigger the tests?
1. Start docker
2. Build the tests using this command 
```
docker build --target testrunner -t pokedex-tests:latest .
```
3. Run them by invoking the following 
```
docker run pokedex-tests:latest
```

## Things to consider when and if the code reaches production
- Integrate to an alerting / monitoring service such as Datadog in order to capture logs
- Use GitHub Actions to trigger tests when pushing / requesting a PR
- Use a service to scan the codebase for vulnerabilities

## Useful Links / References
[.Net Core + Docker](https://joehonour.medium.com/a-guide-to-setting-up-a-net-core-project-using-docker-with-integrated-unit-and-component-tests-a326ca5a0284)