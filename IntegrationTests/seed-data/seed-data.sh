#!/usr/bin/env bash

# metadata-svc
mongoimport --host mongodb --port 27017 --db import --collection metadata --type json --file /seed-data/metadata.json --jsonArray