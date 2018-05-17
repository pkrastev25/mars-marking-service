#!/usr/bin/env bash

# metadata-svc
mongoimport --port 27017 --db import --collection metadata --type json --file ./metadata.json --jsonArray