#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["src/MQRunners/Consumers/Parakeet.NetCore.ConsumerServer/Parakeet.NetCore.ConsumerServer.csproj", "src/MQRunners/Consumers/Parakeet.NetCore.ConsumerServer/"]
RUN dotnet restore "src/MQRunners/Consumers/Parakeet.NetCore.ConsumerServer/Parakeet.NetCore.ConsumerServer.csproj"
COPY . .
WORKDIR "/src/src/MQRunners/Consumers/Parakeet.NetCore.ConsumerServer"
RUN dotnet build "Parakeet.NetCore.ConsumerServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Parakeet.NetCore.ConsumerServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Parakeet.NetCore.ConsumerServer.dll"]