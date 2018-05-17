#!/usr/bin/env bash

# metadata-svc
mongoimport --host mongo --port 27017 --db import --collection metadata --type json --file ./metadata.json --jsonArray