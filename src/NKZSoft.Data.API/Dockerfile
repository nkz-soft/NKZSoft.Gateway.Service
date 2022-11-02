FROM mcr.microsoft.com/dotnet/aspnet:6.0
EXPOSE 80
EXPOSE 81

RUN apt-get update && apt-get install -y curl

WORKDIR /app
COPY ./publish/NKZSoft.Data.API .

ENTRYPOINT ["dotnet", "NKZSoft.Data.API.dll"]
