FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 9093

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Todo.Profile/Todo.Profile.csproj", "Todo.Profile/"]
RUN dotnet restore "Todo.Profile/Todo.Profile.csproj"
COPY . .
WORKDIR "/src/Todo.Profile"
RUN dotnet build "./Todo.Profile.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Todo.Profile.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

RUN mkdir -p /home/app/.aspnet/DataProtection-Keys && \
    chown -R app:app /home/app/.aspnet/DataProtection-Keys

USER app

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ["Todo.Profile/docker-entrypoint.sh", "/docker-entrypoint.sh"]
USER root
RUN chmod +x /docker-entrypoint.sh
USER app
ENTRYPOINT ["/docker-entrypoint.sh"]
