FROM nexus.informatik.haw-hamburg.de/microsoft/dotnet:2.0.0-sdk

# Marking service API runs on port 8080.
EXPOSE 8080

RUN curl -L https://github.com/docker/compose/releases/download/1.21.2/docker-compose-$(uname -s)-$(uname -m) -o /usr/local/bin/docker-compose
RUN chmod +x /usr/local/bin/docker-compose