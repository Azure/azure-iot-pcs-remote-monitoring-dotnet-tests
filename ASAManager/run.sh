#!/usr/bin/env bash
# Copyright (c) Microsoft. All rights reserved.
# Note: Windows Bash doesn't support shebang extra params
set -e

TEST_SUITE="ASA Manager Tests"

TEST_HOME="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )/"
APP_HOME="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && cd .. && pwd )/"

source "$APP_HOME/scripts/.functions.sh"

start_containers() {
    header2 "$TEST_SUITE - Starting services..."

    cd $APP_HOME
    ./scripts/storageadapter.sh start
    ./scripts/telemetry.sh start
    ./scripts/devicesimulation.sh start
    ./scripts/config.sh start
    ./scripts/iothubmanager.sh start
    ./scripts/asamanager.sh start
    docker ps -a
}

stop_containers() {
    header2 "$TEST_SUITE - Stopping services..."

    cd $APP_HOME
    docker ps -a
    ./scripts/storageadapter.sh stop
    ./scripts/telemetry.sh stop
    ./scripts/devicesimulation.sh stop
    ./scripts/config.sh stop
    ./scripts/iothubmanager.sh stop
    ./scripts/asamanager.sh stop
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

    docker logs "asa-manager"
}

#Script options  
repo=""
tag="testing"

set_up() {
    export DOCKER_TAG=$tag
    export REPO=dotnet
    check_dependency_docker
    check_dependency_dotnet
}

run() {
    start_containers
    run_tests
    stop_containers
}

tear_down() {
    unset DOCKER_TAG
}

while [[ $# -gt 0 ]] ;
do
    opt=$1;
    shift;
    case $opt in
        -t|--tag) tag=$1; shift;;
        *) shift;;
    esac
done

header "Running $TEST_SUITE"

set_up
run
tear_down

set +e
