#!/bin/bash

set -e

echo "Params: $APP_DIR $APP_CFG $APP_BIN"

if [ -e /app/lnd/lnd.conf ]; then
    echo "Using existing configuration file"
else
    echo "Configuration file not provided, exiting..."
    exit 1
fi

if [ ! -f "/app/lnd/data/chain/bitcoin/regtest/wallet.db" ]; then
    echo "Starting in init mode: lnd --lnddir=/app/lnd"
    lnd --lnddir=/app/lnd &

    echo "Waiting for lightning node to start..."
    while ! timeout 1 bash -c ">/dev/tcp/localhost/11009" 2>/dev/null; do
        echo "Waiting for lightning node port to open..."
        sleep 2
    done

    while ! lncli -n regtest --lnddir=/app/lnd --rpcserver=localhost:11009 state > /dev/null 2>&1; do
        echo "Waiting for lightning node to start..."
        sleep 5
    done

    while ! lncli -n regtest --lnddir=/app/lnd --rpcserver=localhost:11009 getinfo > /dev/null 2>&1; do
        echo "Make sure the lightning node is initialized - check README.md for more information. Waiting for it to be ready..."
        sleep 5
    done

    pkill lnd
    sleep 10
fi

echo "Starting lightning node with unlocked wallet..."
lnd --lnddir=/app/lnd --wallet-unlock-password-file=/secret/password.txt


