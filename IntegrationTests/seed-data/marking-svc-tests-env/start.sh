#!/usr/bin/env bash
# Used to build you application, push a docker image to Nexus and restart the service in your cluster

DOCKER_REGISTRY="nexus.informatik.haw-hamburg.de"
SERVICE_NAME="marking-svc-tests-env"

docker build -t ${DOCKER_REGISTRY}/${SERVICE_NAME} .
docker push ${DOCKER_REGISTRY}/${SERVICE_NAME}