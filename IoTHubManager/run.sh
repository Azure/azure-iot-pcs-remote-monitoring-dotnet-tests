#!/usr/bin/env bash
# Copyright (c) Microsoft. All rights reserved.
# Note: Windows Bash doesn't support shebang extra params
set -e

APP_HOME="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && cd .. && pwd )/"
cd $APP_HOME

// start your containers

./scripts/stroageadapter.sh start
./scripts/pcsconfig.sh start
./scripts/telemetry.sh start

// run the test
dotnet....