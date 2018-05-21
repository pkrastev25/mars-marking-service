#!/usr/bin/env bash

# database-utility-svc
mongoimport --host result-mongodb --port 27017 --db ResultData --collection result-data-marks --type json --file /SeedResultData/result-data-marks.json --jsonArray