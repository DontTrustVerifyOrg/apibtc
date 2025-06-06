FROM debian:12.7 AS build_btc_lnd

ENV BTC_VERSION=27.1
ENV LND_VERSION=0.18.3-beta

RUN apt-get update && apt-get install -y wget gpg

WORKDIR /app
RUN wget https://bitcoincore.org/bin/bitcoin-core-${BTC_VERSION}/bitcoin-${BTC_VERSION}-x86_64-linux-gnu.tar.gz
RUN tar -xvf bitcoin-${BTC_VERSION}-x86_64-linux-gnu.tar.gz
RUN mv ./bitcoin-${BTC_VERSION} ./btc

RUN wget https://github.com/lightningnetwork/lnd/releases/download/v${LND_VERSION}/lnd-linux-amd64-v${LND_VERSION}.tar.gz
RUN tar -xvf lnd-linux-amd64-v${LND_VERSION}.tar.gz
RUN mv ./lnd-linux-amd64-v${LND_VERSION} ./lnd



FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build_api

COPY . /app
WORKDIR /app/net/WalletAPI
RUN dotnet restore
RUN dotnet publish -c Release -o out ./WalletAPI.csproj



FROM postgres:17.4-bookworm

RUN apt-get update && apt-get install -y gettext procps curl lsb-release
RUN curl -L https://dot.net/v1/dotnet-install.sh -o dotnet-install.sh
RUN chmod +x dotnet-install.sh
RUN ./dotnet-install.sh --runtime aspnetcore
RUN rm dotnet-install.sh
ENV PATH=$PATH:$DOTNET_ROOT:$DOTNET_ROOT/tools

WORKDIR /app

RUN mkdir -p api lnd btc data
RUN ln -s /app/lnd /lnd

COPY --from=build_btc_lnd /app/btc/bin/* /usr/local/bin/
COPY --from=build_btc_lnd /app/lnd/* /usr/local/bin/
COPY --from=build_api /app/net/WalletAPI/out /usr/local/walletapi/

COPY ./docker/btc/bitcoin.conf.template /app/bitcoin.conf.template
COPY ./docker/lnd/lnd.conf.template /app/lnd.conf.template
COPY ./docker/apibtc/wallet.conf.template /app/wallet.conf.template

COPY ./docker/aio/entrypoint.sh /app/entrypoint.sh
RUN chmod +x /app/entrypoint.sh

ENTRYPOINT ["/app/entrypoint.sh"]
