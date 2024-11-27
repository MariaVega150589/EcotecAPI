FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app
COPY "./WebUI.csproj" .
RUN ls
RUN dotnet restore .
COPY . .
RUN dotnet publish -c release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY --from=build /app .
ENTRYPOINT ["dotnet", "WebUI.dll"]
