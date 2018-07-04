#!/usr/bin/env bash
# Copyright (c) Microsoft. All rights reserved.
# Note: Windows Bash doesn't support shebang extra params
set -e

TEST_HOME="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )/"
APP_HOME="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && cd .. && pwd )/"

start_containers() {

    cd $APP_HOME
    ./scripts/storageadapter.sh start
    ./scripts/pcsconfig.sh start
    ./scripts/telemetry.sh start
    ./scripts/devicesimulation.sh start
    docker ps -a
}

start_containers

set +e