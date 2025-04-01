FROM debian:12.7 AS build

ENV LND_VERSION=0.18.3-beta

RUN apt-get update && apt-get install -y wget gpg

WORKDIR /app
RUN wget https://github.com/lightningnetwork/lnd/releases/download/v${LND_VERSION}/lnd-linux-amd64-v${LND_VERSION}.tar.gz
RUN tar -xvf lnd-linux-amd64-v${LND_VERSION}.tar.gz
RUN mv ./lnd-linux-amd64-v${LND_VERSION} ./lnd



FROM debian:12.7

RUN apt-get update && apt-get install -y gettext jq curl procps

WORKDIR /app

COPY --from=build /app/lnd/* /usr/local/bin/
COPY ./docker/lnd/lnd.conf.template /app/lnd.conf.template
COPY ./docker/lnd/entrypoint.sh /app/entrypoint.sh
RUN chmod +x /app/entrypoint.sh

ENTRYPOINT ["/app/entrypoint.sh"]

