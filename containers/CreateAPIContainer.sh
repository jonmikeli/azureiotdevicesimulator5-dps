#!/bin/bash

#1-Build the docker image
echo "IoT HUB SDK REST API-Image-Creation"
docker build -t iothubsdk-api -f ..\sources\IoT.Simulator\IoT.Simulator.API.DeviceManagement\IoT.Simulator.API.DeviceManagement.API\Dockerfile ..\sources\IoT.Simulator

#2-Run a container based on the just created image
echo "IoT HUB SDK REST API-Container-Creation"
docker run -ti --name iothubapi -P -p 5001:443 -e ASPNETCORE_URLS="https://+" -e ASPNETCORE_HTTPS_PORT=5001  iothubsdk-api

echo "IoT HUB SDK REST API-Image and container created"