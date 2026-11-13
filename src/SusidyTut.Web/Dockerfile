# Етап збирання
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY src/SusidyTut.Web/SusidyTut.Web.csproj src/SusidyTut.Web/
RUN dotnet restore src/SusidyTut.Web/SusidyTut.Web.csproj
COPY src/SusidyTut.Web/ src/SusidyTut.Web/
RUN dotnet publish src/SusidyTut.Web/SusidyTut.Web.csproj -c Release -o /app/publish --no-restore

# Етап виконання
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
# Дані часових поясів для Europe/Kyiv
RUN apt-get update && apt-get install -y --no-install-recommends tzdata && rm -rf /var/lib/apt/lists/*
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SusidyTut.Web.dll"]
