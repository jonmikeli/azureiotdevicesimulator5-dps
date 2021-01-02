# Azure IoT Device Simulator (.NET 5) - DPS version - Provisoning

## Provisioning
The implemented provisioning relies on [Azure IoT Hub Device Provisioning Service](https://docs.microsoft.com/en-us/azure/iot-dps/).

## Azure IoT Hub Device Provisioning Service

The simulator has the capability to contact the DPS with the provided configuration and get a connection string for an Azure IoT Hub.
The Azure IoT Hub is assigned according to the rules set in the DPS.

## Simulator DPS settings

The simulator has been designed to work with different provisioning use cases:
 1-If the simulator has no connection string, a provisioning process is initiated.
   This process requires a DPS configuration to be set.
   The DPS configuration can be provided by:
     - environment variables (might be useful in containerized platforms. Pay attention though to the processes allowed to read the environment variables.).
     - command line parameters, that will overwrite any existing environment variables setting (recommended for not containerized targets. This is probably the safest alternative among the three, in terms of security.).
     - if none of the previous settings are found, a `dpssettings.json` file will be loaded (recommended only for development platforms, unless the JSON is encrypted or stored in safe location.).
 2-If the simulator finds a connection string, the provisioning process is skipped and the found connection string is used.

## Simulator connection string (device)
If the provisioning process succeeds, it will create a device identity in the associated Azure IoT Hub.
The DPS will send back to the device requesting the provisioning, the data allowing the device to identify and connect to the IoT Hub.

That connection settings may be stored at the device level to avoid having to reprovision the device.
They should be stored in a secured manner.

The simulator stores the connection string in the `devicesettings.json` file.
It is persisted in clear for develoment purposes but keep in mind this data should be protected more securely in production environments.

```json
{
  "deviceId": "",
  "connectionString": "",
  "simulationSettings": {
    "enableLatencyTests": false,
    "latencyTestsFrecuency": 30,
    "enableDevice": true,
    "enableModules": false,
    "enableTelemetryMessages": false,
    "telemetryFrecuency": 10,
    "enableErrorMessages": false,
    "errorFrecuency": 20,
    "enableCommissioningMessages": false,
    "commissioningFrecuency": 30,
    "enableTwinReportedMessages": false,
    "twinReportedMessagesFrecuency": 60,
    "enableReadingTwinProperties": false,
    "enableC2DDirectMethods": true,
    "enableC2DMessages": true,
    "enableTwinPropertiesDesiredChangesNotifications": true
  }
}

```

## Passing DPS parameters
### Different possibilities
The simulator accepts different ways to use the DPS settings:
 - envionment variables
 - command line parameters
 - configuration file

> NOTE
>
> The deviceId parameter is required by the provisioning process. It is located in the `devicesettings.json` file for all the configuration types.

### Priorities
The first settings to be checked are environment variables.
Command line settings overwrite any environment variables settings.
If no environment setting or command line parameters are provided, a configuration file is searched. 

### Environment variables
The list of the environment variables to set are:
 - DPS_IDSCOPE, the Id Scope of the DPS
 - PRIMARY_SYMMETRIC_KEY, the primary pey of the DPS

_Linux (bash)_
```bash
export DPS_IDSCOPE=[YOUR ID SCOPE]
export PRIMARY_SYMMETRIC_KEY=[YOUR PRIMMARY KEY]
dotnet IoT.Simulator.dll
```

_Windows (cmd)_
```cmd
set DPS_IDSCOPE=[YOUR ID SCOPE]
set PRIMARY_SYMMETRIC_KEY=[YOUR PRIMMARY KEY]
dotnet IoT.Simulator.dll
```

> NOTE
>
> If the primary key is stored at some point, it should be saved in a secured mannger (TPM/HSM, etc).

### Command line parameters
The previous variables may be set through command variables too.
The required parameters are the same:
 - -s, the Id Scope of the DPS
 - -p, the primary key of the DPS

_Linux (bash)_
```bash
dotnet IoT.Simulator.dll -s [YOUR ID SCOPE] -p [YOUR PRIMMARY KEY]
```

_Windows (cmd)_
```cmd
dotnet IoT.Simulator.dll -s [YOUR ID SCOPE] -p [YOUR PRIMMARY KEY]
```

### Configuration file
DPS settings can also be provided through a configuration file.
It looks like this:

```json
{
  "dpsSettings": {
    "enrollmentType": "Group",
    "groupEnrollmentSettings": {
      "securityType": "SymetricKey",
      "symetricKeySettings": {
        "idScope": "[YOUR ID SCOPE]",
        "primaryKey": "[YOUR PRIMARY KEY]",
        "enrollmentType": "Group",
        "globalDeviceEndpoint": "global.azure-devices-provisioning.net",
        "transportType": "Mqtt"
      }
    }
  }
}
```

Parameters like `enrollmentType`, `securityType`, `globalDeviceEndpoint` and `transportType` cannot be changed for now.
Upcoming versions should allow more possibilities.

## Device modules
At the time this post has been written, it did not seem to be a way to create device modules (a.k.a. module identities) during the provisioning process.

This can be implemented by code but it requires one of the steps below:
 - reference the Azure IoT Hub Service SDK (which requires a SAS connection string to the IoT Hub with specific rights....not really matching the security constraints we are looking for in the implemented scenario with DPS).
 - reference a REST API taking in charge the logic of creation device modules. The provisioned device could call that REST API once DPS has done his job and that the device is provisioned.
