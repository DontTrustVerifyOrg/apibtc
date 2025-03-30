#!/bin/bash

set -e

if [ -e /app/btc/bitcoin.conf ]; then
    echo "Using existing configuration file"
else
    echo "Creating configuration file from template and environment variables"
    envsubst < /app/bitcoin.conf.template > /app/btc/bitcoin.conf
fi

if [ -e /app/lnd/lnd.conf ]; then
    echo "Using existing configuration file"
else
    echo "Creating configuration file from template and environment variables"
    envsubst < /app/lnd.conf.template > /app/lnd/lnd.conf
fi

if [ -e /app/data/wallet.conf ]; then
    echo "Using existing configuration file"
else
    echo "Creating configuration file from template and environment variables"
    envsubst < /app/wallet.conf.template > /app/data/wallet.conf
fi



echo
echo "Starting: bitcoind -datadir=/app/btc -printtoconsole"
echo
bitcoind -datadir=/app/btc &


echo "Attempt to create a wallet..."
while ! bitcoin-cli -datadir=/app/btc createwallet "testwallet" > /dev/null 2>&1; do
    echo "Waiting for bitcoin node to be ready..."
    sleep 5
done


echo "Starting in init mode: lnd --lnddir=/app/lnd"
lnd --lnddir=/app/lnd &
while ! lncli -n regtest --lnddir=/app/lnd --rpcserver=localhost:11009 state > /dev/null 2>&1; do
    echo "Waiting for lightning node to start..."
    sleep 5
done

while ! lncli -n regtest --lnddir=/app/lnd --rpcserver=localhost:11009 getinfo > /dev/null 2>&1; do
    echo "Make sure the lightning node is initialized. Waiting for it to be ready..."
    sleep 5

    if ! pgrep -f "lnd" > /dev/null; then
        echo "Starting lightning node..."
        lnd --lnddir=/app/lnd --wallet-unlock-password-file=/secret/password.txt &
    fi
done



echo "Starting API..."
cd /app/api
dotnet WalletAPI.dll --basedir=/app/data

