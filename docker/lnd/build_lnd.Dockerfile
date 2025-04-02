FROM debian:12.7 AS build

ENV LND_VERSION=0.18.3-beta

RUN apt-get update && apt-get install -y wget gpg

WORKDIR /app
RUN wget https://github.com/lightningnetwork/lnd/releases/download/v${LND_VERSION}/lnd-linux-amd64-v${LND_VERSION}.tar.gz
RUN tar -xvf lnd-linux-amd64-v${LND_VERSION}.tar.gz
RUN mv ./lnd-linux-amd64-v${LND_VERSION} ./lnd



FROM debian:12.7

ENV APP_DIR=lnd
ENV APP_CFG=lnd.conf
ENV APP_BIN=lnd

RUN apt-get update && apt-get install -y gettext jq curl procps

WORKDIR /app

RUN mkdir -p /app/$APP_DIR

COPY --from=build /app/lnd/* /usr/local/bin/
COPY ./docker/lnd/lnd.conf.template /app/${APP_CFG}.template
COPY ./docker/lnd/entrypoint.sh /app/entrypoint.sh
RUN chmod +x /app/entrypoint.sh

ENTRYPOINT ["/app/entrypoint.sh"]
CMD [$APP_DIR, $APP_CFG, $APP_BIN]



