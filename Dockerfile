FROM nexus.informatik.haw-hamburg.de/microsoft/aspnetcore:2.0

# Marking service API runs on port 8080.
EXPOSE 8080

# Copy the sources into the Docker container.
ADD ./mars-marking-svc/out /out 

# Build and run the program on container startup.
WORKDIR /out

# Install mongo related utilites, needed for seeding data
RUN apt-get update && apt-get install -y mongodb

ENTRYPOINT dotnet mars-marking-svc.dll