#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["IoT.Simulator.API.DeviceManagement/IoT.Simulator.API.DeviceManagement.API/IoT.Simulator.API.DeviceManagement.API.csproj", "IoT.Simulator.API.DeviceManagement/IoT.Simulator.API.DeviceManagement.API/"]
COPY ["IoT.Simulator.API.DeviceManagement/IoT.Simulator.API.DeviceManagement.API.Common/IoT.Simulator.API.DeviceManagement.API.Common.csproj", "IoT.Simulator.API.DeviceManagement/IoT.Simulator.API.DeviceManagement.API.Common/"]
COPY ["IoT.Simulator.API.DeviceManagement/IoT.Simulator.API.DeviceManagement.Services/IoT.Simulator.API.DeviceManagement.Services.csproj", "IoT.Simulator.API.DeviceManagement/IoT.Simulator.API.DeviceManagement.Services/"]
COPY ["IoT.Simulator.API.DeviceManagement/IoT.Simulator.API.DeviceManagement.API.DataContracts/IoT.Simulator.API.DeviceManagement.API.DataContracts.csproj", "IoT.Simulator.API.DeviceManagement/IoT.Simulator.API.DeviceManagement.API.DataContracts/"]
COPY ["IoT.Simulator.API.DeviceManagement/IoT.Simulator.API.DeviceManagement.IoC.Configuration/IoT.Simulator.API.DeviceManagement.IoC.Configuration.csproj", "IoT.Simulator.API.DeviceManagement/IoT.Simulator.API.DeviceManagement.IoC.Configuration/"]
RUN dotnet restore "IoT.Simulator.API.DeviceManagement/IoT.Simulator.API.DeviceManagement.API/IoT.Simulator.API.DeviceManagement.API.csproj"
COPY . .
WORKDIR "/src/IoT.Simulator.API.DeviceManagement/IoT.Simulator.API.DeviceManagement.API"
RUN dotnet build "IoT.Simulator.API.DeviceManagement.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IoT.Simulator.API.DeviceManagement.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IoT.Simulator.API.DeviceManagement.API.dll"]

LABEL version="v1.0" environment="dev" author="Jon Mikel Inza" copyright="JMI" github="https://github.com/jonmikeli/azureiotdevicesimulator5-dps" name="IoTHub.SDK.API" description="Docker image of a REST API exposing Azure IoTHub SDK features." tags="IoT;Simulator;IoTHub;Azure IoT SDK;net5;chsarp;REST"