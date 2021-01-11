docker build -t iotsimulator-dps -f .\Dockerfile ..
docker run -ti --rm --name device10 -e PROVISIONING_REGISTRATION_ID=device10 -e ENVIRONMENT=Development --network="host" iotsimulator-dps