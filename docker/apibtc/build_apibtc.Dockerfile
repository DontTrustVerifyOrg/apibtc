# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
COPY . /app
WORKDIR /app/net/WalletAPI
RUN dotnet restore
RUN dotnet publish -c Release -o out ./WalletAPI.csproj

# Run
FROM mcr.microsoft.com/dotnet/aspnet:8.0

ENV APP_DIR=apibtc
ENV APP_CFG=wallet.conf
ENV APP_BIN=WalletAPI.dll

RUN apt-get update && apt-get install -y gettext jq curl
RUN mkdir -p /app/$APP_DIR
WORKDIR /app/$APP_DIR

COPY ./docker/apibtc/wallet.conf.template /app/${APP_CFG}.template
COPY ./docker/apibtc/entrypoint.sh /app/entrypoint.sh
RUN chmod +x /app/entrypoint.sh

COPY --from=build /app/net/WalletAPI/out /usr/local/${APP_DIR}/

EXPOSE 80

ENTRYPOINT ["/app/entrypoint.sh"]
CMD [$APP_DIR, $APP_CFG, $APP_BIN]

