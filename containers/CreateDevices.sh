#!/bin/bash
echo "test"

i=1

while [ $i -lt 20 ]
do
  deviceName='autodevice'$i

  echo "Creating the device: $deviceName"
  docker run -ti --rm --name $deviceName -d -e PROVISIONING_REGISTRATION_ID=$deviceName -e ENVIRONMENT=Development --network="host" iotsimulator-dps
  echo "Device created: $deviceName"
  let "i+=1" 
done