
FROM lightninglabs/lnd:v0.18.3-beta

RUN apk add --no-cache --update gettext jq curl
RUN mkdir -p /secret /app/lnd

COPY ./docker/lnd/lnd.conf.template /app/lnd.conf.template
COPY ./docker/lnd/entrypoint.sh /app/entrypoint.sh
RUN chmod +x /app/entrypoint.sh

ENTRYPOINT ["/app/entrypoint.sh"]
