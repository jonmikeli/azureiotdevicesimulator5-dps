#!/bin/bash
i=1

while [ $i -lt 20 ]
do
  deviceName='autodevice'$i

  echo "Creating the device: $deviceName"
  docker run -ti --rm --name $deviceName -d -e PROVISIONING_REGISTRATION_ID=$deviceName -e ENVIRONMENT=Development -e DPS_SECURITY_TYPE="X509CA" -e DEVICE_CERTIFICATE_PATH="" -e DEVICE_CERTIFICATE_PASSWORD="" --network="host" iotsimulator-dps
  docker run -ti --rm --name $deviceName -d -e PROVISIONING_REGISTRATION_ID=$deviceName -e ENVIRONMENT=Development -e DPS_SECURITY_TYPE="SymmetricKey" -e PRIMARY_SYMMETRIC_KEY="" --network="host" iotsimulator-dps
  echo "Device created: $deviceName"
  
  let "i+=2" 
done