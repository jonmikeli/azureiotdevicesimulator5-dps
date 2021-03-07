FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS buildwithupdatedca
#RUN apt update & apt upgrade
## REQUIRED TO OVERCOME THE ISSUE INTRODUCED AT 2021/01 DUE TO CA CERTIFICATES
#https://github.com/NuGet/Announcements/issues/49
RUN apt-get update \
&& apt-get remove ca-certificates -y \
&& apt-get purge ca-certificates -y \
&& apt-get install -f ca-certificates=20200601~deb10u2 -y
##

FROM buildwithupdatedca AS build
WORKDIR /src
COPY ["IoT.Simulator/IoT.Simulator.csproj", "IoT.Simulator/"]
RUN dotnet restore "IoT.Simulator/IoT.Simulator.csproj"
COPY . .
#RUN rm -f dpssettings.*
WORKDIR "/src/IoT.Simulator"
RUN dotnet build "IoT.Simulator.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "IoT.Simulator.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "IoT.Simulator.dll"]

EXPOSE 8883
EXPOSE 443
EXPOSE 5671

LABEL version="v1.0" environment="dev" author="Jon Mikel Inza" copyright="JMI" github="https://github.com/jonmikeli/azureiotdevicesimulator5-dps" name="IoT.Simulator.Dps" description="IoT Simulator docker image. Implements DPS provisioning with symmetric key and CA X509 authentication." tags="IoT;Simulator;DPS;IoTHub;Azure IoT SDK;net5;csharp;Symmetric key;CA X509"