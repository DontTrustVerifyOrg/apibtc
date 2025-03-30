# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
COPY . /app
WORKDIR /app/net/WalletAPI
RUN dotnet restore
RUN dotnet publish -c Release -o out ./WalletAPI.csproj

# Run
FROM mcr.microsoft.com/dotnet/aspnet:8.0
RUN apt update && apt install -y gettext jq curl
WORKDIR /app
RUN mkdir -p /app/data/
COPY ./docker/api/entrypoint.sh /app/entrypoint.sh
COPY --from=build /app/net/WalletAPI/out .
COPY ./docker/api/wallet.conf.template /app/wallet.conf.template
RUN chmod +x /app/entrypoint.sh

ENV ListenHost=http://0.0.0.0:80/
EXPOSE 80

# ENTRYPOINT ["sleep", "infinity"]
ENTRYPOINT ["/app/entrypoint.sh", "WalletAPI.dll", "wallet.conf"]