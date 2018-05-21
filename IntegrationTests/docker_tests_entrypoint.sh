#!/usr/bin/env bash
# Used to set the commands for the integration_tests image inside the docker compose

cd mars-marking-svc/IntegrationTests &&
dotnet test -c Release
