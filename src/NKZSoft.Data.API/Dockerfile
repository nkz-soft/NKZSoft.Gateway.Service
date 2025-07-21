FROM mcr.microsoft.com/dotnet/aspnet:9.0
EXPOSE 80
EXPOSE 81

RUN apt-get update && apt-get install -y curl

WORKDIR /app
RUN pwd
COPY ./publish .

ENTRYPOINT ["dotnet", "NKZSoft.Data.API.dll"]
