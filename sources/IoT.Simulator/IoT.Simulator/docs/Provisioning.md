# Azure IoT Device Simulator (.NET 5) - DPS version - Provisoning

## Provisioning
The implemented provisioning relies on [Azure IoT Hub Device Provisioning Service](https://docs.microsoft.com/en-us/azure/iot-dps/).

## Azure IoT Hub Device Provisioning Service

The simulator has the capability to contact the DPS with the provided configuration and get a connection string for an Azure IoT Hub.
The Azure IoT Hub is assigned according to the rules set in the DPS.

## Simulator DPS settings

The simulator has been designed to work in different provisioning use cases:
 1-If the simulator has no connection string, a provisioning process is initiated.
   This process requires a DPS configuration to be set.
   The DPS configuration can be provided by:
     - environment variables (recommended and useful for containerized targets. It does not compromise security levels.).
     - command line parameters, that will overwrite the environment variables (recommended for not containerized targets. Similarly to the previous point,it keeps the level of the security rules.).
     - if none of the previous settings are found, a dpssettings.json file will be loaded (not recommended unless the JSON file is persisted in a secured location).
 2-If the simulator finds a connection string, it uses and it skips the provisioning process.

## Simulator connection string (device)
If the provisioning process succeeds, it will create a device identity in a given IoT Hub.
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
The simulator accepts different ways to use the DPS settings:
 - envionment variables
 - command line parameters
 - configuration file

The first two are oriented to secured environments and avoiding to persist those settings. Any locally persisted information should be saved in a very secured environment (cryptography, HSM, TPM, etc).

> NOTE
>
> The deviceId parameter is mandatory to make the provisioning successful. It is located in the `devicesettings.json` file for all the types of configuration.

### Environment variables
The list of the environment variables to set are:
 - DPS_IDSCOPE, the Id Scope of the DPS
 - PRIMARY_SYMMETRIC_KEY, the primary pey of the DPS

> NOTE
>
> If the primary key is stored at some point, it should be saved in a secured mannger (TPM/HSM, etc).

### Command line parameters
The previous variables may be set through command variables too.
The required parameters are the same:
 - -s, the Id Scope of the DPS
 - -p, the primary key of the DPS

### Configuration file

## Device modules
At the time this post has been written, it did not seem to be a way to create device modules (a.k.a. module identities) during the provisioning process.

This can be implemented by code but it requires one of the steps below:
 - reference the Azure IoT Hub Service SDK (which requires a SAS connection string to the IoT Hub with specific rights....not really matching the security constraints we are looking for in the implemented scenario with DPS).
 - reference a REST API taking in charge the logic of creation device modules. The provisioned device could call that REST API once DPS has done his job and that the device is provisioned.
