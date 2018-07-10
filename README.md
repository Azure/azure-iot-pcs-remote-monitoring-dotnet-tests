[![Build][build-badge]][build-url]
[![Issues][issues-badge]][issues-url]
[![Gitter][gitter-badge]][gitter-url]

Azure IoT Remote Monitoring Tests
=================================

This repository contains a set of functional tests periodically executed
on Microsoft Azure IoT Remote Monitoring solution.

Dependencies
============

The integration tests solution depends on all the environment variables set on the machine. The list of variables are in [env.list](scripts/env.list) file

How to run the integration tests
===========================

Note: If using Windows, Git Bash is recommended.

## Build and Run from the command line

The [scripts](scripts) folder contains scripts for many frequent tasks:

* `build`: compiles all the project and runs all the integration tests projects that are listed in [build](scripts/build) script file

## Running the integration tests with Visual Studio

### Prerequisite

All the containers need to be started before the tests can be run/debug in Visual Studio.

* navigate to the `scripts` folder
* `<name-of-service>.sh start`: starts all the containers.
* `<name-of-service>.sh stop`: stops all the containers.

Ex:
```
cd scripts
./pcsconfig.sh start
```

### Build and run tests

* `Ctrl+Shift+B`: builds solution
* `Run All`: from the `Test` menu select `Windows-->Test Explorer`. Click `Run All` to run all the tests 


[build-badge]: https://img.shields.io/travis/Azure/azure-iot-pcs-remote-monitoring-dotnet-tests.svg
[build-url]: https://travis-ci.org/Azure/azure-iot-pcs-remote-monitoring-dotnet-tests
[issues-badge]: https://img.shields.io/github/issues/azure/azure-iot-pcs-remote-monitoring-dotnet-tests.svg
[issues-url]: https://github.com/azure/azure-iot-pcs-remote-monitoring-dotnet-tests/issues
[gitter-badge]: https://img.shields.io/gitter/room/azure/iot-solutions.js.svg
[gitter-url]: https://gitter.im/azure/iot-solutions
