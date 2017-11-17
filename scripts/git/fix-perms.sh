#!/usr/bin/env bash
# Copyright (c) Microsoft. All rights reserved.
# Note: Windows Bash doesn't support shebang extra params
set -e

# Sometimes when creating bash scripts in Windows, bash scripts will not have
# the +x flag carried over to Linux/MacOS. This script should help setting the
# permission flags right.

APP_HOME="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && cd .. && cd .. && pwd )/"
cd $APP_HOME

chmod ugo+x ./scripts/build
chmod ugo+x ./scripts/git/setup
chmod ugo+x ./scripts/git/*.sh
chmod ugo+x ./scripts/devicesimulation.sh
chmod ugo+x ./scripts/iothubmanager.sh
chmod ugo+x ./scripts/pcsconfig.sh
chmod ugo+x ./scripts/storageadapter.sh
chmod ugo+x ./IoTHubManager/run.sh

git update-index --chmod=+x ./scripts/build
git update-index --chmod=+x ./scripts/git/setup
git update-index --chmod=+x ./scripts/git/*.sh
git update-index --chmod=+x ./scripts/devicesimulation.sh
git update-index --chmod=+x ./scripts/iothubmanager.sh
git update-index --chmod=+x ./scripts/pcsconfig.sh
git update-index --chmod=+x ./scripts/storageadapter.sh
git update-index --chmod=+x ./IoTHubManager/run.sh

set +e
