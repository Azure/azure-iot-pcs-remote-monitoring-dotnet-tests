#!/usr/bin/env bash
# Copyright (c) Microsoft. All rights reserved.
# Note: Windows Bash doesn't support shebang extra params
set -e

APP_HOME="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && cd .. && pwd )/"
cd $APP_HOME
source "$APP_HOME/scripts/.functions.sh"

start() {
    ./scripts/pcsconfig.sh start
    ./scripts/storageadapter.sh start
    ./scripts/devicesimulation.sh start
    ./scripts/telemetry.sh start
    ./scripts/iothubmanager.sh start
    ./scripts/asamanager.sh start
}

stop() {
    ./scripts/pcsconfig.sh stop
    ./scripts/storageadapter.sh stop
    ./scripts/devicesimulation.sh stop
    ./scripts/telemetry.sh stop
    ./scripts/iothubmanager.sh stop
    ./scripts/asamanager.sh stop
}

if [[ "$1" == "start" ]]; then
    start
    exit 0
fi

if [[ "$1" == "stop" ]]; then
    stop
    exit 0
fi

error "No command specified, pass either 'start' or 'stop'."
exit 1