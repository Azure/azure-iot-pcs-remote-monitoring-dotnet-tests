#!/usr/bin/env bash
# Copyright (c) Microsoft. All rights reserved.
# Note: Windows Bash doesn't support shebang extra params
set -e

DOCKER_IMAGE="azureiotpcs/iothub-manager-dotnet:$DOCKER_TAG"
DOCKER_PORT=9002
DOCKER_NAME="iothub-manager"

APP_HOME="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && cd .. && pwd )/"
cd $APP_HOME

exit_if_running() {
    ISUP=$(curl -ks http://127.0.0.1:$DOCKER_PORT/v1/status | grep -i "{" | wc -l | tr -d '[:space:]')
    if [[ "$ISUP" != "0" ]]; then
        echo "'$DOCKER_IMAGE' is already running"
        exit 0
    fi
}

create_network() {
    set +e
    docker network create integrationtests 2> /dev/null
    set -e
}

start() {
    docker pull $DOCKER_IMAGE
    echo "Starting '$DOCKER_IMAGE'"
    docker run --detach --network="integrationtests" -p 127.0.0.1:$DOCKER_PORT:$DOCKER_PORT \
        --env-file $APP_HOME/scripts/env.list --rm --name $DOCKER_NAME $DOCKER_IMAGE
}

stop() {
    echo "Stopping '$DOCKER_IMAGE'"
    docker rm -f $DOCKER_NAME
}

if [[ "$1" == "start" ]]; then
    create_network
    exit_if_running
    start
    exit 0
fi

if [[ "$1" == "stop" ]]; then
    stop
    exit 0
fi

echo "No command specified, pass either 'start' or 'stop'."
exit 1