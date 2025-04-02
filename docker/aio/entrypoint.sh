#!/bin/bash

set -e

docker-entrypoint.sh postgres &
POSTGRES_PID=$!

if [ -f /app/btc/bitcoin.conf ]; then
    echo "Using provided bitcoin configuration file"
else
    echo "Bitcoin configuration file not provided, exiting..."
    exit 1
fi

if [ -f /app/lnd/lnd.conf ]; then
    echo "Using provided lightning node configuration file"
else
    echo "Lightning node configuration file not provided, exiting..."
    exit 1
fi

if [ -f /app/apibtc/wallet.conf ]; then
    echo "Using provided wallet configuration file"
else
    echo "Wallet configuration file not provided, exiting..."
    exit 1
fi

echo "Starting: bitcoind -datadir=/app/btc -printtoconsole"
bitcoind -datadir=/app/btc -printtoconsole &
BTC_PID=$!

echo "Checking if wallet exists..."

# Wait for bitcoin node to be ready
while ! bitcoin-cli -datadir=/app/btc getblockchaininfo > /dev/null 2>&1; do
    echo "Waiting for bitcoin node to be ready..."
    sleep 5
done

sleep 10

# Check if wallet already exists
if [ -d "/app/btc/regtest/wallets/testwallet" ]; then
    echo "Wallet 'testwallet' already exists"
    bitcoin-cli -datadir=/app/btc loadwallet "testwallet" > /dev/null 2>&1 || true
else
    echo "Creating wallet 'testwallet'..."
    bitcoin-cli -datadir=/app/btc createwallet "testwallet"
fi

while ! bitcoin-cli -datadir=/app/btc getbalances > /dev/null 2>&1; do
    echo "Waiting for bitcoin wallet to be ready..."
    sleep 5
done

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
lnd --lnddir=/app/lnd --wallet-unlock-password-file=/secret/password.txt &
LND_PID=$!

while ! lncli -n regtest --lnddir=/app/lnd --rpcserver=localhost:11009 getinfo > /dev/null 2>&1; do
    echo "Waiting for lightning node to be ready..."
    sleep 5
done

# Check if PostgreSQL is ready with our database and user
echo "Checking if PostgreSQL database is ready..."
while ! PGPASSWORD=$POSTGRES_PASSWORD psql -h localhost -U $POSTGRES_USER -d $POSTGRES_DB -c "SELECT 1" > /dev/null 2>&1; do
    echo "Waiting for PostgreSQL database to be ready with user $POSTGRES_USER and database $POSTGRES_DB..."
    sleep 5
done
echo "PostgreSQL database is ready!"

echo "Starting API..."
cd /usr/local/walletapi
/root/.dotnet/dotnet WalletAPI.dll --basedir=/app/apibtc &
DOTNET_PID=$!

# Wait for all processes to complete
echo "All services started. Waiting for processes to complete..."
wait $POSTGRES_PID $BTC_PID $LND_PID $DOTNET_PID

