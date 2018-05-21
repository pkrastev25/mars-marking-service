#!/usr/bin/env bash
# Starts the integration tests within marking-svc-tests image in docker compose

cd mars-marking-svc/IntegrationTests &&
dotnet test -c Release
