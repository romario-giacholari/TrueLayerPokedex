# Pokedex

## Prerequisites
- Install docker https://docs.docker.com/get-docker/

## How to run the application
- Start docker
- `cd` into `Pokedex`
- Then build it by running the following command in your terminal `docker build -t pokedex .`
- Next start it up by running `docker run -it --rm -p 3000:80 --name pokedexcontainer pokedex`
- Finally in your browser (alternatively, you can use curl, postman etc), go to `http://localhost:3000/pokemon/pokemon-name-here` and `http://localhost:3000/pokemon/translated/pokemon-name-here`
