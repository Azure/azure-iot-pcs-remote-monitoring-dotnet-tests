#!/usr/bin/env bash
# Copyright (c) Microsoft. All rights reserved.
# Note: Windows Bash doesn't support shebang extra params
set -e

APP_HOME="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && cd .. && pwd )/"
cd $APP_HOME
source "$APP_HOME/scripts/.functions.sh"

start() {
    ./scripts/storageadapter.sh start
    ./scripts/devicesimulation.sh start
    ./scripts/telemetry.sh start
    ./scripts/config.sh start
    ./scripts/iothubmanager.sh start
    ./scripts/asamanager.sh start
}

stop() {
    ./scripts/storageadapter.sh stop
    ./scripts/devicesimulation.sh stop
    ./scripts/telemetry.sh stop
    ./scripts/config.sh stop
    ./scripts/iothubmanager.sh stop
    ./scripts/asamanager.sh stop
}

#Script Args
repo=""
tag="testing"
act="start"
dockeraccount="azureiotpcs"

set_up_env() {
    if [[ "$repo" == "dotnet" ]]; then
        export REPO="dotnet"
    elif [[ "$repo" == "java" ]]; then
        export REPO="java"
    else 
        error "No microservice type specified, pass either 'dotnet' or 'java'."
        echo "eg:- sh containers.sh -re java"
        exit 1
    fi
	export DOCKER_TAG=$tag
    export DOCKER_ACCOUNT=$dockeraccount
}

tear_down() {
    unset DOCKER_TAG
    unset DOCKER_ACCOUNT
}

while [[ $# -gt 0 ]] ;
do
    opt=$1;
    shift;	
    case $opt in
        -dt|--dockertag) tag=$1; shift;;
        -re|--repo) repo=$1; shift;;
        -act|--action) act=$1; shift;;
        -da|--docker-account) dockeraccount=$1; shift;; 
        *) shift;
    esac
done

set_up_env

if [[ "$act" == "start" ]]; then
    start
    exit 0
fi

if [[ "$act" == "stop" ]]; then
    stop
    exit 0
fi

tear_down

error "No command specified, pass either 'start' or 'stop'."
exit 1
