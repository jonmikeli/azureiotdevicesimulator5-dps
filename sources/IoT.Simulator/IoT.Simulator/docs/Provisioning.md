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

## Passing DPS parameters

### Environment variables
### Command line parameters
### Configuration file
