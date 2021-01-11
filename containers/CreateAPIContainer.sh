
#1-Build the docker image
docker build -t iothubsdk-api -f .\Dockerfile ..\..

#2-Run a container based on the just created image
docker run -ti --name iothubapi -P -p 5001:443 -e ASPNETCORE_URLS="https://+" -e ASPNETCORE_HTTPS_PORT=5001 -e ASPNETCORE_Kestrel__Certificates__Default__Path [path] -e ASPNETCORE_Kestrel__Certificates__Default__Password [password] iothubsdk-api