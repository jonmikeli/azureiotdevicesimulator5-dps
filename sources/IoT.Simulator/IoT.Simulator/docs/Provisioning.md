# Azure IoT Device Simulator (.NET 5) - DPS version - Provisoning

## Provisioning
The implemented provisioning relies on [Azure IoT Hub Device Provisioning Service](https://docs.microsoft.com/en-us/azure/iot-dps/).
The simulator has the capability to contact the DPS with the provided configuration and get a connection string to an Azure IoT Hub.
The Azure IoT Hub is assigned according to the rules set in the DPS.

## Azure IoT Hub Device Provisioning Service

DPS allows the simulator to provision and get its running connection string dynamically. For more details about what this means, see [here](sources/IoT.Simulator/IoT.Simulator/docs/Provisioning.md).

The simulator is prepared to work with different types of settings situations.
 1-If the simulator has no connection string, a provisioning process is initiated.
   This process requires a DPS configuration to be set (details [here](sources/IoT.Simulator/IoT.Simulator/docs/Provisioning.md)).
   The DPS configuration can be provided by:
     - environment variables (recommended and useful for containerized targets)
     - command line parameters, that will overwrite the environment variables (recommended for not containerized targets)
     - if none of former are found, a dpssettings.json file with the mentioned settings (not recommended even though it is probably quite practical for development purposes)
 2-If the simulator finds a connection string, it uses it avoiding the provisioning process.

This implementation allows to cover different use cases, many of them being security (or good practices) oriented.