#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY . .
RUN dotnet restore "Cringe.Web/Cringe.Web.csproj"
WORKDIR "/src/Cringe.Web"
RUN dotnet build "Cringe.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Cringe.Web.csproj" -c Release -o /app/publish

FROM base AS final

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cringe.Web.dll"]