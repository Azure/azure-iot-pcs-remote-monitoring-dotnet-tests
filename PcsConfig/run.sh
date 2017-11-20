#!/usr/bin/env bash
# Copyright (c) Microsoft. All rights reserved.
# Note: Windows Bash doesn't support shebang extra params
set -e

TEST_SUITE=$1

APP_HOME="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && cd .. && pwd )/"
cd $APP_HOME

source "$APP_HOME/scripts/.functions.sh"

start_containers() {
    header2 "$TEST_SUITE - Starting services..."
    ./scripts/storageadapter.sh start
    ./scripts/pcsconfig.sh start
    ./scripts/telemetry.sh start
}

stop_containers() {
    header2 "$TEST_SUITE - Stopping services..."
    ./scripts/storageadapter.sh stop
    ./scripts/iothubmanager.sh stop
    ./scripts/devicesimulation.sh stop
}

run_tests() {
    header2 "$TEST_SUITE - Downloading dependencies..."
    dotnet restore

    header2 "$TEST_SUITE - Compiling tests..."
    dotnet build --configuration Release

    header2 "$TEST_SUITE - Running tests..."
    dotnet test --configuration Release PcsConfig/PcsConfig.csproj
}

header "Running $TEST_SUITE"

check_dependency_docker
check_dependency_dotnet

start_containers
run_tests
stop_containers

set +e
