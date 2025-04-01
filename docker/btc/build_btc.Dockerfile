FROM debian:12.7 AS build

ENV BTC_VERSION=27.1

RUN apt-get update && apt-get install -y wget gpg

WORKDIR /app
RUN wget https://bitcoincore.org/bin/bitcoin-core-${BTC_VERSION}/bitcoin-${BTC_VERSION}-x86_64-linux-gnu.tar.gz
RUN tar -xvf bitcoin-${BTC_VERSION}-x86_64-linux-gnu.tar.gz
RUN mv ./bitcoin-${BTC_VERSION} ./btc



FROM debian:12.7

RUN apt-get update && apt-get install -y gettext jq curl

WORKDIR /app

COPY --from=build /app/btc/bin/* /usr/local/bin/
COPY ./docker/btc/bitcoin.conf.template /app/bitcoin.conf.template
COPY ./docker/btc/entrypoint.sh /app/entrypoint.sh
RUN chmod +x /app/entrypoint.sh

ENTRYPOINT ["/app/entrypoint.sh"]
