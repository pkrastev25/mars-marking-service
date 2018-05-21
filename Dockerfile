FROM nexus.informatik.haw-hamburg.de/microsoft/aspnetcore:2.0

# Marking service API runs on port 8080.
EXPOSE 8080

# Copy the sources into the Docker container.
ADD ./mars-marking-svc/out /out 

# Build and run the program on container startup.
WORKDIR /out

ENTRYPOINT dotnet mars-marking-svc.dll