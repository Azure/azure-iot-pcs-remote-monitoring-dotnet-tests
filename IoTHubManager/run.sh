#!/usr/bin/env bash
# Copyright (c) Microsoft. All rights reserved.
# Note: Windows Bash doesn't support shebang extra params
set -e

TEST_SUITE="IOTHub Manager Tests"

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

#Script options  
repo=""
tag="testing"

set_up() {
    export DOCKER_TAG=$tag
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

#### Parse script arguments
while [[ $# -gt 0 ]] ;
do
    opt=$1;
    shift;
    case $opt in
        -t|--tag) tag=$1; shift;;
        -re|--repo) repo=$1; shift;;
        *)  shift;;
    esac
done

header "Running $TEST_SUITE"

#### Set Up Tests
set_up

#### Run tests based on repo
if [ "$repo" == "both" ] || [ "$repo" == "" ]; then
    export REPO=dotnet
    run
    export REPO=java
    run
elif [[ "$repo" == "dotnet" ]]; then
    export REPO=dotnet
    run
elif [[ "$repo" == "java" ]]; then
    export REPO=java
    run
fi

#### Tear down test
tear_down

set +e
