
#1-Build the docker image
docker build -t iothubsdk-api -f ..\sources\IoT.Simulator\IoT.Simulator.API.DeviceManagement\IoT.Simulator.API.DeviceManagement.API\Dockerfile ..\sources\IoT.Simulator

#2-Run a container based on the just created image
docker run -ti --name iothubapi -P -p 5001:443 -e ASPNETCORE_URLS="https://+" -e ASPNETCORE_HTTPS_PORT=5001 -e ASPNETCORE_Kestrel__Certificates__Default__Path [path] -e ASPNETCORE_Kestrel__Certificates__Default__Password [password] iothubsdk-api