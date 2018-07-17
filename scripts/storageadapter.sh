#!/usr/bin/env bash
# Copyright (c) Microsoft. All rights reserved.
# Note: Windows Bash doesn't support shebang extra params
set -e

if [[ "$DOCKER_TAG" == "" ]]; then
    export DOCKER_TAG=testing
fi

DOCKER_IMAGE="azureiotpcs/pcs-storage-adapter-$REPO:$DOCKER_TAG"
DOCKER_PORT=9022
DOCKER_NAME="storage-adapter"
DOCKER_NETWOK="integrationtests"

APP_HOME="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && cd .. && pwd )/"
cd $APP_HOME

source "$APP_HOME/scripts/.functions.sh"

check_env_variables()
{
    if [[ ! -n "${PCS_STORAGEADAPTER_DOCUMENTDB_CONNSTRING}" ]]; then
        error "PCS_STORAGEADAPTER_DOCUMENTDB_CONNSTRING is not set"
        exit -1
    fi

    header3 "Connection strings set in environment variables"
}

exit_if_running() {
    ISUP=$(curl -ks http://127.0.0.1:$DOCKER_PORT/v1/status | grep -i "{" | wc -l | tr -d '[:space:]')
    if [[ "$ISUP" != "0" ]]; then
        error "'$DOCKER_IMAGE' is already running"
        exit 0
    fi
}

fail_if_not_running() {
    sleep 1
    ISUP=$(docker ps | grep $DOCKER_NAME | wc -l | tr -d '[:space:]')
    if [[ "$ISUP" == "0" ]]; then
        error "'$DOCKER_IMAGE' failed to start or crashed"
        exit -1
    fi
}

create_network() {
    set +e
    docker network create $DOCKER_NETWOK 2> /dev/null
    set -e
}

start() {
    docker pull $DOCKER_IMAGE
    header3 "Starting '$DOCKER_IMAGE'"
    docker run --detach --network=$DOCKER_NETWOK -p 127.0.0.1:$DOCKER_PORT:$DOCKER_PORT \
        --env-file $APP_HOME/scripts/env.list --rm --name $DOCKER_NAME $DOCKER_IMAGE
    sleep 1
    fail_if_not_running
}

stop() {
    set +e
    header3 "Stopping '$DOCKER_IMAGE'"
    docker rm -f $DOCKER_NAME
    set -e
}

if [[ "$1" == "start" ]]; then
    exit_if_running
    check_env_variables
    create_network
    start
    exit 0
fi

if [[ "$1" == "stop" ]]; then
    stop
    exit 0
fi

error "No command specified, pass either 'start' or 'stop'."
exit 1
