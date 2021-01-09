# Notes

## HTTPS Settings
[Source](https://docs.microsoft.com/en-us/aspnet/core/security/enforcing-ssl?view=aspnetcore-5.0&tabs=visual-studio)


### Environment variables
ASPNETCORE_HTTPS_PORT
ASPNETCORE_URLS

### Trust the ASP.NET Core HTTPS development certificate on Windows
```bash
dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\aspnetapp.pfx -p { password here }
dotnet dev-certs https --trust
```

### Trust HTTPS certificate on Linux
See distribution documentation.

### Trust HTTPS certificate from WSL

### HTTPS in Docker container
[Source](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host)

[Direct link](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-3.0#https_port-1)

## Run the container
```bash
docker pull mcr.microsoft.com/dotnet/core/samples:aspnetapp
docker run --rm -it -p 8000:80 -p 8001:443 -e ASPNETCORE_URLS="https://+;http://+" -e ASPNETCORE_HTTPS_PORT=8001 -e ASPNETCORE_Kestrel__Certificates__Default__Password="password" -e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx -v %USERPROFILE%\.aspnet\https:/https/ mcr.microsoft.com/dotnet/core/samples:aspnetapp
```

## How to
Detailled [how to](https://thegreenerman.medium.com/set-up-https-on-local-with-net-core-and-docker-7a41f030fc76).