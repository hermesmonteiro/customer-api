#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build-base

RUN dotnet nuget add source https://nexus.itspty.com/repository/nuget-group/ --name Nexus_Serve
RUN dotnet tool install --tool-path /tools dotnet-trace
RUN dotnet tool install --tool-path /tools dotnet-counters
RUN dotnet tool install --tool-path /tools dotnet-dump

WORKDIR /app

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base

RUN apk add icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

WORKDIR /app
FROM build-base as build
# library layer
COPY ["Launch.sln", "."]
COPY ["src/Web/Launch.Api/Launch.Api.csproj", "src/Web/Launch.Api/"]
COPY ["src/Web/Launch.Host/Launch.Host.csproj", "src/Web/Launch.Host/"]
COPY ["src/Infrastructure/Launch.Infrastructure/Launch.Infrastructure.csproj", "src/Infrastructure/Launch.Infrastructure/"]
COPY ["src/Core/CustomDateTimeValidatorLib/CustomDateTimeValidatorLib.csproj", "src/Core/CustomDateTimeValidatorLib/"]
COPY ["src/Core/Launch.Domain/Launch.Domain.csproj", "src/Core/Launch.Domain/"]
COPY ["src/Core/Launch.Application/Launch.Application.csproj", "src/Core/Launch.Application/"]
COPY ["tests/Launch.Integration.Tests.Api/Launch.Integration.Tests.Api.csproj", "tests/Launch.Integration.Tests.Api/Launch.Integration.Tests.Api.csproj"]
COPY ["tests/Launch.Unit.Tests.Api/Launch.Unit.Tests.Api.csproj", "tests/Launch.Unit.Tests.Api/Launch.Unit.Tests.Api.csproj"]
RUN dotnet restore "src/Web/Launch.Host/Launch.Host.csproj"
RUN dotnet restore "tests/Launch.Integration.Tests.Api/Launch.Integration.Tests.Api.csproj"
RUN dotnet restore "tests/Launch.Unit.Tests.Api/Launch.Unit.Tests.Api.csproj"

#build output layer

# this doesn't work, returns 3 files missing
# COPY ["./src","."]
# COPY ["./tests", "."]

COPY [".", "."]

FROM build as publish-tests

RUN dotnet publish "/app/tests/Launch.Integration.Tests.Api/Launch.Integration.Tests.Api.csproj" -o /app/publish/tests
RUN dotnet publish "/app/tests/Launch.Unit.Tests.Api/Launch.Unit.Tests.Api.csproj" -o /app/publish/tests

COPY Launch.sln "/app/publish/tests"
WORKDIR "/app/tests/Launch.Api"

FROM build as publish-app
WORKDIR "/app/src/Web/Launch.Host/"
RUN dotnet publish "Launch.Host.csproj" -c Release -o /app/publish/release

FROM build-base AS test-release
WORKDIR /app
COPY --from=publish-tests /app/publish/tests ./tests

FROM base AS app-release
WORKDIR /app
COPY --from=publish-app /app/publish/release ./release

WORKDIR /tmp
FROM nexus.itspty.com:5002/alpine as newrelic
RUN  mkdir /tmp/newrelic-netcore20-agent \
&& cd /tmp \
&& export NEW_RELIC_DOWNLOAD_URI=http://download.newrelic.com/dot_net_agent/previous_releases/9.4.0/newrelic-netcore20-agent_9.4.0.0_amd64.tar.gz \
&& echo "Downloading: $NEW_RELIC_DOWNLOAD_URI into $(pwd)" \
&& wget -O - "$NEW_RELIC_DOWNLOAD_URI" | gzip -dc | tar xf -

FROM app-release as release
ENV APP_PATH=/app/release
WORKDIR $APP_PATH
COPY --from=newrelic /tmp/newrelic-netcore20-agent ${APP_PATH}/newrelic-netcore20-agent
RUN apk add tzdata \
&& ln -s /usr/share/zoneinfo/EST5EDT /etc/localtime \
&& date

COPY --from=build /tools /tools

ENTRYPOINT ["dotnet", "Launch.Host.dll"]

EXPOSE 80 443
ENV CORECLR_NEWRELIC_HOME=${APP_PATH}/newrelic-netcore20-agent
ENV CORECLR_PROFILER_PATH=${APP_PATH}/newrelic-netcore20-agent/libNewRelicProfiler.so
ENV CORECLR_ENABLE_PROFILING=1
ENV CORECLR_PROFILER={36032161-FFC0-4B61-B559-F6C5D41BAE5A}


