# Estágio base
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
WORKDIR /app

# Estágio de build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Worker/Worker.csproj", "Worker/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["Shared/Shared.csproj", "Shared/"]

RUN dotnet restore "Worker/Worker.csproj"

COPY . .

WORKDIR "/src/Worker"
RUN dotnet publish "Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

# Install the New Relic agent
USER root
RUN apt-get update \
    && apt-get install -y ca-certificates \
    && echo 'deb [trusted=yes] https://apt.newrelic.com/debian/ newrelic non-free' | tee /etc/apt/sources.list.d/newrelic.list \
    && apt-get update \
    && apt-get install -y newrelic-dotnet-agent \
    && rm -rf /var/lib/apt/lists/*

# Enable the New Relic agent
ENV CORECLR_ENABLE_PROFILING=1 \
    CORECLR_PROFILER={36032161-FFC0-4B61-B559-F6C5D41BAE5A} \
    CORECLR_NEWRELIC_HOME=/usr/local/newrelic-dotnet-agent \
    CORECLR_PROFILER_PATH=/usr/local/newrelic-dotnet-agent/libNewRelicProfiler.so \
    NEW_RELIC_APP_NAME="ProcessamentoService"

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ProcessamentoService.Worker.dll"]
