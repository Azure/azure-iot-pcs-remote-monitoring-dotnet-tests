#!/usr/bin/env bash
# Copyright (c) Microsoft. All rights reserved.
# Note: Windows Bash doesn't support shebang extra params
set -e

TEST_SUITE=$1

TEST_HOME="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )/"
APP_HOME="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && cd .. && pwd )/"

source "$APP_HOME/scripts/.functions.sh"

start_containers() {
    header2 "$TEST_SUITE - Starting services..."

    cd $APP_HOME
    ./scripts/storageadapter.sh start
    ./scripts/iothubmanager.sh start
    ./scripts/devicesimulation.sh start
    docker ps -a
}

stop_containers() {
    header2 "$TEST_SUITE - Stopping services..."

    cd $APP_HOME
    docker ps -a
    ./scripts/storageadapter.sh stop
    ./scripts/iothubmanager.sh stop
    ./scripts/devicesimulation.sh stop
}

run_tests() {
    header2 "$TEST_SUITE - Downloading dependencies..."

    cd $APP_HOME
    dotnet restore

    header2 "$TEST_SUITE - Compiling tests..."
    dotnet build --configuration Release

    header2 "$TEST_SUITE - Running tests..."
    cd $TEST_HOME
    find . -name *.csproj | xargs dotnet test --configuration Release
}

header "Running $TEST_SUITE"

check_dependency_docker
check_dependency_dotnet

start_containers
run_tests
stop_containers

set +e
