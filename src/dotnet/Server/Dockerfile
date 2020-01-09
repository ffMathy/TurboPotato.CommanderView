#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["CommanderView.Client/CommanderView.Client.csproj", "CommanderView.Client/"]
RUN dotnet restore "CommanderView.Client/CommanderView.Client.csproj"
COPY . .
WORKDIR "/src/CommanderView.Client"
RUN dotnet build "CommanderView.Client.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CommanderView.Client.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CommanderView.Client.dll"]