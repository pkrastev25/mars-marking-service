FROM nexus.informatik.haw-hamburg.de/microsoft/aspnetcore:2.0

# Marking service API runs on port 8080.
EXPOSE 8080

# Install mongo related utilites, needed for seeding data
RUN apt-get update && apt-get install -y mongodb