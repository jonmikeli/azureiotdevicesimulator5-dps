# Azure IoT Device Simulator (.NET 5) - DPS version - Readme

## New features
This version of the Azure IoT Device Simulator integrates the use of a DPS (Device Provisioning Service). For now, it implements provisioning based on:
 - group enrollment
 - symmetric keys*

_*Upcoming versions will include provisioning with X509 devices._

## Description - Not just a simple DPS integration
What could seem a simple feature addition, it has involved some deep work to keep the whole underlying mechanism coherent with full automatic provisioning and create "real life" representative value.

The purpose of the previous simulators were to:
 - cover a maximum set of features in the SDK, in a realistc way.
 - help developers, testers, customers or any IoT related worker to facilitate the adoption and the use of device side code / tools / samples.

With those same purposes in mind, this version of the simulator implements mainly the same kind of experience but adapted to DPS oriented provisioning contexts.
The experience will be the same for one device, a few of them or hundreds / thousands of them.

A bash script is provided in the repository to illustrate how to create as many instances of the simulator (containerized version) as wanted very fast (in a few seconds)
, provisioned by themselves and interacting with the attributed [Azure IoT Hub](https://azure.microsoft.com/en-us/services/iot-hub/) in a totally autonomous way.
The whole D2C/C2D experience is implemented (except file uploads), with devices as well as module identities.

Messages are based on the templates used in the previous simulators.
I will consider merging this version with the one using IoT PnP as soon as the tests of the latter will be validated.

That may seem a logic next step but I am looking forward reading the feedbacks of both of the simulators in order to take into account the community needs.


<br/>

For more information:
 - [*How to (Quickstart)*](docs/HowTo.md)
 - [*Help  and details*](docs/Help.md) 
 
 <br/>

Example of uses:
 - development tool for developers working in Microsoft Azure IoT solutions (cloud)
 - tester tool in IoT-oriented projects
 - scalable IoT simulation platforms
 - fast and simple development of IoT devices
 - etc

<br/>

Technical information:
 - .NET 5 (C#)
 - Microsoft Azure IoT SDK (provisioning, properties, tags, C2D/D2C, device modules*)

_* Device modules do not refer to IoT Edge modules but to IoT Device modules._

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
 - simple and lightweight application, containerizable.
 - message templates based on JSON (3 message types by default in this first version).
 - implementation of full IoT flows (C2D, D2C, C2M, M2C) - see below for more details.


## Functional features

### Device level (C2D/D2C)

*Provisioning*
The implemented provisioning relies on [Azure IoT Hub Device Provisioning Service](https://docs.microsoft.com/en-us/azure/iot-dps/).
The simulator has the capability to call the DPS with the provided configuration and get an IoT Hub connection string according to the policies configured in it.
More details [here](docs/Provisioning.md).

The simulator has been designed to work in different provisioning use cases:
 1-If the simulator has no connection string, a provisioning process is initiated.
   This process requires a DPS configuration to be set (details [here](docs/Provisioning.md)).
   The DPS configuration can be provided by:
     - environment variables (recommended and useful for containerized targets. It does not compromise security levels.).
     - command line parameters, that will overwrite the environment variables (recommended for not containerized targets. Similarly to the previous point,it keeps the level of the security rules.).
     - if none of the previous settings are found, a dpssettings.json file will be loaded (not recommended unless the JSON file is persisted in a secured location).
 2-If the simulator finds a connection string, it uses and it skips the provisioning process.
 
*Commands*
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


[details](docs/Help.md).

  
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
- IoT Plug and Play integration. An especial version totally IoT Plug and Play-oriented has been released [here](https://github.com/jonmikeli/azureiotdevicesimulator5-pnp).
- "fileupload" feature implementation.

## More information

- Details about **HOW the solution WORKS** are provided in the [help](docs/Help.md) section.
- Details about **HOW the solution can be USED** are provided in the [how to](docs/HowTo.md) section.
