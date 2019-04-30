FROM mcr.microsoft.com/dotnet/core/aspnet:2.1 AS base
WORKDIR /app
FROM mcr.microsoft.com/dotnet/core/sdk:2.1 AS build
WORKDIR /apps
COPY . .
RUN dotnet restore -nowarn:msb3202,nu1503
RUN dotnet build --no-restore -c Release -o /app

FROM build AS publish
RUN dotnet publish --no-restore -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
COPY key.txt /app
ENV ASPNETCORE_URLS http://+:5000
EXPOSE 5000

ENTRYPOINT ["dotnet", "ticketsbackend.dll"]
