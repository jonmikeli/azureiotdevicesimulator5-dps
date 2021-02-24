# Azure IoT Device Simulator (.NET 5) - DPS version - Readme

## New features
This version of the Azure IoT Device Simulator integrates the use of a DPS (Device Provisioning Service). For now, it implements provisioning based on:
 - group enrollment
 - symmetric keys
 - CA X509 certificates

## Description - Not just a simple DPS integration
What could seem at first glance a simple feature addition has involved some deep work to keep the whole underlying mechanism coherent with full automatic provisioning and create at the same time "real life" representative value.

The purposes of the previous simulators were to:
 - cover a maximum set of features in the SDK, in a realistc way.
 - help developers, testers, customers or any IoT related worker to facilitate the adoption and the use of device side code / tools / samples.

With those same purposes in mind, this version of the simulator implements mainly the same kind of experience but adapted to DPS oriented provisioning contexts.
The experience will be the same for one device, a few of them or hundreds / thousands of them.

A script is provided in the repository to showcase how to create as many instances of the simulator (containerized version) as wanted. The whole set of simulated devices may be provisioned and running in a few seconds.....literally. Moreover, once the initial settings configured (configuration files), everything happens automatically (provisioning, modules, telemetries, D2C/C2D flows, etc).

Telemetry messages are based on the templates used in the previous simulators.

<br/>

More information:
 - [*How to (Quickstart)*](docs/HowTo.md)
 - [*Help  and details*](docs/Help.md) 
 - [*Provisioning*](https://github.com/jonmikeli/azureiotdevicesimulator5-dps/blob/master/docs/Provisioning.md)
 
 <br/>

Examples of uses:
 - development tool for developers working in Microsoft Azure IoT solutions (cloud).
 - tester tool in IoT-oriented projects.
 - scalable IoT simulation platforms.
 - complete and configurable D2C (data and reported properties) and C2D (direct methods, desired properties, messages, jobs, etc).
 - etc

<br/>

Technical information:
 - .NET 5 (C#)
 - Microsoft Azure IoT SDK (provisioning, properties, tags, C2D/D2C, device modules*).
 - Docker and bash for the containerized experiences.

<br/>

*Azure IoT Device Simulator logs*

![Azure IoT Device Simulator Logs](docs/images/AzureIoTDeviceSimulatorLos.gif)

<br/>

## Global features
 - device provisioning (group enrollment with symetric keys).
 - device level simulation (C2D/D2C).
 - module level simulation (C2M/M2C).
 - device simulation configuration based on JSON files.
 - module simulation configuration based on JSON files.
 - no specific limitation on the number of modules (only limited by Microsoft Azure IoT Hub constraints).
 - containerizable and accepts environment variables.
 - message templates based on JSON (3 message types by default in this first version).
 - implementation of full IoT flows (C2D, D2C, C2M, M2C).

See below for more details.

## Functional features

### Device level (C2D/D2C)

*Provisioning*


The implemented provisioning relies on [Azure IoT Hub Device Provisioning Service](https://docs.microsoft.com/en-us/azure/iot-dps/).
The simulated device calls the DPS with the provided configuration and gets an Azure IoT Hub connection string according to the policies configured in the DPS.
More details [here](docs/Provisioning.md).

The simulator has been designed to work in different provisioning use cases:
 1. If the simulator has no connection string, a provisioning process is initiated.
   This process requires a DPS configuration to be set (details [here](docs/Provisioning.md)).
   The DPS configuration can be provided by:
    - environment variables (recommended and useful for containerized targets. It is less prone to compromise security.).
    - command line parameters, that will overwrite the environment variables (recommended for not containerized targets. Similarly to the previous point, it keeps the level of security rules.).
    - if none of the previous settings are found, a `dpssettings.json` file will be loaded (not recommended unless the JSON file is persisted in a secured location).
 1. If the simulator finds a connection string, it uses and it skips the provisioning process.
 
> NOTE
>
> The device id may be provided through an environment variable too. This allows easier scripted provisioning scenarios (ex: loop to create a set of simulated devices).

*Commands implemented by default*
 - request latency test
 - reboot device
 - device On/Off
 - read device Twin
 - generic command (with JSON payload)
 - generic command
 - update telemetry interval

 
 *Messages*


 D2C: The device can send messages of different types (telemetry, error, commissioning).
 
 C2D: Microsoft Azure IoT Hub can send messages to a given device.

 
 *Twin*


 Any change in the Desired properties is notified and handled by the device.

 The device reports changes in different types of information to the Microsoft Azure IoT Hub.


### Module level (C2M/M2C)
The features at the module level are the identical to the device features except for the latency tests.

> NOTE
>
> The security type implemented for module identities is based on symetric keys. Depending on the faced use cases, additional use cases may be added in upcoming versions. Adding X509 module identities will require processes to add the required certificates in a secured way (to the cloud and the device sides).
> Personnally, I am wondering why a given device and its module identities could implement different types of security. Which cases may lead to such scenarii?



[Details](docs/Help.md).

  
## Global technical features

Functional features are based on these generic technical features:
 - device provisioning and reprovisioning.
 - telemetry sent from a device.
 - a device can contain one or many modules.
 - each module behaves independently with its own flows (C2M/M2C) and its configuration settings.
 - the device that contains the modules has its own behavior (based on its own configuration file).
 - telemetry sent from a module.
 - messages received by a device.
 - messages received by a module.
 - commands received by a device.
 - commands received by a module.
 - Twin Desired properties changed notification (for devices).
 - Twin Desired properties changed notification (for modules).
 - Twin Reported properties updates from a device.
 - Twin Reported properties updates from a module.


### D2C
#### Device level
 - IoT Messages
 - Twins (Reported)

#### Module level (M2C)
 - IoT Messages
 - Twins (Reported)

### C2D
#### Device level
 - Twins (Desired)
 - Twins (Tags)
 - Direct Methods
 - Messages

#### Module level (C2M)
 - Twins (Desired)
 - Twins (Tags)
 - Direct Methods
 - Messages

## Upcoming features
- Add X509 devices
- IoT Plug and Play integration. An especial version totally IoT Plug and Play-oriented has been released [here](https://github.com/jonmikeli/azureiotdevicesimulator5-pnp).
- "fileupload" feature implementation.

## More information

- Details about **HOW the solution WORKS** are provided in the [help](docs/Help.md) section.
- Details about **HOW the solution can be USED** are provided in the [how to](docs/HowTo.md) section.
