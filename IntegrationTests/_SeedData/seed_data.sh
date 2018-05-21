#!/usr/bin/env bash

# metadata-svc
mongoimport --host mongodb --port 27017 --db import --collection metadata --type json --file /SeedData/metadata.json --jsonArray &&

# scenario-svc
mongoimport --host mongodb --port 27017 --db mars_websuite --collection mapping_index --type json --file /SeedData/mapping_index.json --jsonArray &&
mongoimport --host mongodb --port 27017 --db mars_websuite --collection scenario_metadata --type json --file /SeedData/scenario_metadata.json --jsonArray &&
mongoimport --host mongodb --port 27017 --db mars_websuite --collection scenario_parameter_sets --type json --file /SeedData/scenario_parameter_sets.json --jsonArray &&

# resultcfg-svc
mongoimport --host mongodb --port 27017 --db Configs --collection OutputConfigs --type json --file /SeedData/OutputConfigs.json --jsonArray &&
mongoimport --host mongodb --port 27017 --db Configs --collection VisualizationConfigs --type json --file /SeedData/VisualizationConfigs.json --jsonArray &&

# sim-runner-svc
mongoimport --host mongodb --port 27017 --db mars-mission-control --collection SimulationPlans --type json --file /SeedData/SimulationPlans.json --jsonArray &&
mongoimport --host mongodb --port 27017 --db mars-mission-control --collection SimulationRuns --type json --file /SeedData/SimulationRuns.json --jsonArray &&

# marking-svc
mongoimport --host mongodb --port 27017 --db marking-svc --collection mark-session --type json --file /SeedData/mark-session.json --jsonArray &&
mongoimport --host mongodb --port 27017 --db hangfire-marking-svc --collection hangfire.stateData --type json --file /SeedData/hangfire.stateData.json --jsonArray