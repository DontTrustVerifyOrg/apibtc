#!/bin/bash

set -e

echo "Params: $APP_DIR $APP_CFG $APP_BIN"

if [ -f /app/$APP_DIR/$APP_CFG ]; then
    echo "Using provided configuration file: $APP_CFG"
else
    echo "Configuration file not provided, exiting..."
    exit 1
fi

START_DOTNET_APP="dotnet $APP_BIN --basedir=/app/$APP_DIR"
echo "Starting: $START_DOTNET_APP"
cd /usr/local/$APP_DIR
exec $START_DOTNET_APP

