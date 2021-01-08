FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["IoT.Simulator/IoT.Simulator.csproj", "IoT.Simulator/"]
RUN dotnet restore "IoT.Simulator/IoT.Simulator.csproj"
COPY . .
WORKDIR "/src/IoT.Simulator"
RUN dotnet build "IoT.Simulator.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "IoT.Simulator.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
#ENTRYPOINT ["dotnet", "IoT.Simulator.dll"]

EXPOSE 8883
EXPOSE 443
EXPOSE 5671

LABEL version="v1.0" environment="dev" author="Jon Mikel Inza" copyright="JMI" github="https://github.com/jonmikeli/azureiotdevicesimulator5-dps" name="IoT.Simulator.Dps" description="Docker image fo an IoT Simulator taking in charge DPS provisioning." tags="IoT;Simulator;DPS;IoTHub;Azure IoT SDK;net5;chsarp"