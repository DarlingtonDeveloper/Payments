FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
RUN apk add --no-cache curl
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /app
COPY ["src/Cox.Cmr.Payment.Api/Cox.Cmr.Payment.Api.csproj", "src/Cox.Cmr.Payment.Api/"]
COPY ["src/Cox.Cmr.Payment.Api.Contracts/Cox.Cmr.Payment.Api.Contracts.csproj", "src/Cox.Cmr.Payment.Api.Contracts/"]
COPY ["src/Cox.Cmr.Payment.Domain/Cox.Cmr.Payment.Domain.csproj", "src/Cox.Cmr.Payment.Domain/"]
COPY ["src/Cox.Cmr.Payment.Infrastructure/Cox.Cmr.Payment.Infrastructure.csproj", "src/Cox.Cmr.Payment.Infrastructure/"]
# Turn off husky
ENV HUSKY 0
ARG ARTIFACTORY_SOURCE
ARG ARTIFACTORY_USERNAME
ARG ARTIFACTORY_API_KEY
RUN dotnet nuget add source $ARTIFACTORY_SOURCE \
     --name Artifactory \
     --username $ARTIFACTORY_USERNAME \
     --password $ARTIFACTORY_API_KEY \
     --store-password-in-clear-text

RUN dotnet restore "src/Cox.Cmr.Payment.Api/Cox.Cmr.Payment.Api.csproj"
COPY . ./

WORKDIR "/app/src/Cox.Cmr.Payment.Api"
RUN dotnet build "Cox.Cmr.Payment.Api.csproj" -c Release -o /app/build

FROM build AS publish
WORKDIR "/app/src/Cox.Cmr.Payment.Api"
RUN dotnet publish "Cox.Cmr.Payment.Api.csproj" -c Release -o /app/publish

FROM base AS final

RUN  mkdir /usr/local/newrelic-dotnet-agent \
     && cd /usr/local \
     && export NEW_RELIC_DOWNLOAD_URI=https://download.newrelic.com/$(wget -qO - "https://nr-downloads-main.s3.amazonaws.com/?delimiter=/&prefix=dot_net_agent/latest_release/newrelic-dotnet-agent" |  \
     grep -E -o "dot_net_agent/latest_release/newrelic-dotnet-agent_[[:digit:]]{1,3}(\.[[:digit:]]{1,3}){2}_amd64\.tar\.gz") \
     && echo "Downloading: $NEW_RELIC_DOWNLOAD_URI into $(pwd)" \
     && wget -O - "$NEW_RELIC_DOWNLOAD_URI" | gzip -dc | tar xf -
# Enable the agent
ARG ENVIRONMENT_STAGE
ARG NEW_RELIC_LICENSE_KEY
ENV CORECLR_ENABLE_PROFILING=1 \
     CORECLR_PROFILER={36032161-FFC0-4B61-B559-F6C5D41BAE5A} \
     CORECLR_NEWRELIC_HOME=/usr/local/newrelic-dotnet-agent \
     CORECLR_PROFILER_PATH=/usr/local/newrelic-dotnet-agent/libNewRelicProfiler.so \
     NEW_RELIC_LICENSE_KEY=$NEW_RELIC_LICENSE_KEY \
     NEW_RELIC_APP_NAME="cmr-$ENVIRONMENT_STAGE-payment-api" \
     ASPNETCORE_HTTP_PORTS=80

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cox.Cmr.Payment.Api.dll"]
