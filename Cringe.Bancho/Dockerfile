#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0-bullseye-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0-bullseye-slim AS build

WORKDIR /src
COPY . .
RUN dotnet restore "Cringe.Bancho/Cringe.Bancho.csproj"
WORKDIR "/src/Cringe.Bancho"
RUN dotnet build "Cringe.Bancho.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Cringe.Bancho.csproj" -c Release -o /app/publish

FROM base AS final

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cringe.Bancho.dll"]