#!/bin/bash
set -e

docker build -f ./CashFlowControl/Dockerfile.migration -t cashflow-migration .
docker run --rm --network cashflow-network cashflow-migration