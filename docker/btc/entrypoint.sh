#!/bin/bash

set -e

echo "Params: $APP_DIR $APP_CFG $APP_BIN"

if [ -f /app/btc/bitcoin.conf ]; then
    echo "Using provided configuration file"
else
    echo "Configuration file not provided, exiting..."
    exit 1
fi

echo "Starting: bitcoind -datadir=/app/btc -printtoconsole"
bitcoind -datadir=/app/btc -printtoconsole &
BITCOIND_PID=$!

echo "Starting up BTC node..."

# Wait for bitcoin node to be ready
while ! bitcoin-cli -datadir=/app/btc getblockchaininfo > /dev/null 2>&1; do
    echo "Waiting for bitcoin node to be ready..."
    sleep 5
done

echo "BTC node is ready"

if [ "$BTC_NETWORK" = "regtest" ]; then
    echo "Checking if wallet exists..."

    sleep 10
    # Check if wallet already exists
    if [ -d "/app/btc/regtest/wallets/testwallet" ]; then
        echo "Wallet 'testwallet' already exists"
        bitcoin-cli -datadir=/app/btc loadwallet "testwallet" > /dev/null 2>&1 || true
    else
        echo "Creating wallet 'testwallet'..."
        bitcoin-cli -datadir=/app/btc createwallet "testwallet"
    fi
fi

wait $BITCOIND_PID

