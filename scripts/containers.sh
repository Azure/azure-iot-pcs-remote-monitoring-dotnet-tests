#!/usr/bin/env bash
# Copyright (c) Microsoft. All rights reserved.
# Note: Windows Bash doesn't support shebang extra params
set -e

start() {
    ./pcsconfig.sh start
    ./storageadapter.sh start
    ./devicesimulation.sh start
    ./telemetry.sh start
    ./iothubmanager.sh start
}

stop() {
    ./pcsconfig.sh stop
    ./storageadapter.sh stop
    ./devicesimulation.sh stop
    ./telemetry.sh stop
    ./iothubmanager.sh stop
}

if [[ "$1" == "start" ]]; then
    start
    exit 0
fi

if [[ "$1" == "stop" ]]; then
    stop
    exit 0
fi