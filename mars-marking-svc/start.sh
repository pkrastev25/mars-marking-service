DOCKER_REGISTRY="nexus.informatik.haw-hamburg.de"
SERVICE_NAME="marking-svc"

rm -rf out

dotnet publish -o out

cd ..

docker build -t ${DOCKER_REGISTRY}/${SERVICE_NAME}:dev .
docker push ${DOCKER_REGISTRY}/${SERVICE_NAME}:dev

kubectl delete pod -l service=${SERVICE_NAME} --force