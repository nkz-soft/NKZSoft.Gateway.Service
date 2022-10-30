FROM mcr.microsoft.com/dotnet/aspnet:6.0
EXPOSE 80
EXPOSE 81

WORKDIR /app
COPY . ./publish/NKZSoft.Gateway.API

ENTRYPOINT ["dotnet", "NKZSoft.Gateway.API.dll"]
