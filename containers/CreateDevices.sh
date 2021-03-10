#!/bin/bash
i=1

transportType="Mqtt"
dpsIDScope="0ne000XXXXX"
x509CertificatePath="X509/new-device.devx5092.cert.pfx"
x509certificatePassword="1234"
symmetricKey="ii3VEjzWZpxhPlWT85O8sg/hZvqk2sNPHPDsP+M9v73BKs9NQHky+Tvg/IFNu1QEWqt5OPZuz1Ia/9IM6R+rag=="



while [ $i -lt 20 ]
do
  deviceName1='autodevice'$i
  deviceName2='autodevice'$i+1

  echo "Creating the device: $deviceName1"
  docker run -ti --rm --name $deviceName1 -d -e PROVISIONING_REGISTRATION_ID=$deviceName -e ENVIRONMENT=Development -e DPS_SECURITY_TYPE="X509CA" -e TRANSPORT_TYPE=$transportType -e DPS_IDSCOPE=$dpsIDScope -e DEVICE_CERTIFICATE_PATH=$x509CertificatePath -e DEVICE_CERTIFICATE_PASSWORD=$x509certificatePassword --network="host" iotsimulator-dps
  echo "Device created: $deviceName1"

  echo "Creating the device: $deviceName2"
  docker run -ti --rm --name $deviceName2 -d -e PROVISIONING_REGISTRATION_ID=$deviceName -e ENVIRONMENT=Development -e DPS_SECURITY_TYPE="SymmetricKey" -e TRANSPORT_TYPE=$transportType -e DPS_IDSCOPE=$dpsIDScope -e PRIMARY_SYMMETRIC_KEY=$symmetricKey --network="host" iotsimulator-dps
  echo "Device created: $deviceName2"
  
  let "i+=2" 
done