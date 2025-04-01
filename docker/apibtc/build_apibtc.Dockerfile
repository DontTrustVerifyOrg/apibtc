# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
COPY . /app
WORKDIR /app/net/WalletAPI
RUN dotnet restore
RUN dotnet publish -c Release -o out ./WalletAPI.csproj

# Run
FROM mcr.microsoft.com/dotnet/aspnet:8.0
RUN apt-get update && apt-get install -y gettext jq curl
RUN mkdir -p /app/apibtc/
WORKDIR /app/apibtc
COPY ./docker/apibtc/wallet.conf.template /app/wallet.conf.template
COPY ./docker/apibtc/entrypoint.sh /app/entrypoint.sh
RUN chmod +x /app/entrypoint.sh

COPY --from=build /app/net/WalletAPI/out /usr/local/apibtc/

EXPOSE 80

ENTRYPOINT ["/app/entrypoint.sh"]

CMD ["WalletAPI.dll", "wallet.conf", "apibtc"]
