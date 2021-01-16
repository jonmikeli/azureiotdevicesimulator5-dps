#!/bin/bash
echo "IoT Simulator DPS-Image-Creation"
docker build -t iotsimulator-dps -f ..\sources\IoT.Simulator\IoT.Simulator\Dockerfile ..\sources\IoT.Simulator

echo "IoT Simulator DPS-Container-Creation"
docker run -ti --rm --name device10 -e PROVISIONING_REGISTRATION_ID=device10 -e ENVIRONMENT=Development --network="host" iotsimulator-dps

echo "IoT Simulator DPS-Image and container creation"