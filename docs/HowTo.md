# Azure IoT Device Simulator (.NET 5, C#) - DPS version - How To


## How to use the Azure IoT Device Simulator?

You can use the source code to compile the application and use it as a regular C# .NET 5 Console application. This may be enough if you need the simulator as a development tool.

If you need instead a reusable simulator and create more than one instance of the simulator, you will probably prefer to containerize it. This may be particularly interesting if you want to create a set of simulated devices.
[This folder](https://github.com/jonmikeli/azureiotdevicesimulator5-dps/tree/master/containers) contains same examples about how to do that.

## Docker prerequisites
In order to use a Docker container, you need to check [Docker](https://www.docker.com/) prerequisites.

Do not forget you will need an internet connection with specific open ports:
 - 8883
 - 5671
 - 443


[Ports](https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-devguide-protocols) required to communicate with Microsoft Azure IoT Hub.


## Steps to run the simulator
The Azure IoT Device Simulator needs three basic things before starting:
 - Required: **settings** (need to be updated with proper values)
 - Required: **[message templates](Help.md)** (included by default)
 - Optional (needed for module identities): deploy the REST API exposing the Azure IoT Hub Service SDK (this is required because module identities cannot be created from the Azure IoT Hub Device SDK). The code is provided. It can also be [containerized](https://github.com/jonmikeli/azureiotdevicesimulator5-dps/tree/master/containers).


### Settings
All the settings except those related to DPS rely on JSON files.
DPS settings can be provided through 3 different ways ([details](Provisioning.md)).

#### JSON files
 - appsettings.json
 - devicesettings.json
 - modulessettings.json
 - dpssettings.json

This [section](Help.md) explains in details all these files.

> [!TIP]
> 
> The solution takes into account **settings** depending on the environment.
> The environment can be set trough the environment variable ENVIRONMENT.
> The solution looks for settings files following the pattern *file.ENVIRONMENT.json* (similar to former transformation files).
> Default setting files will be loaded first in case no environment file is found.

