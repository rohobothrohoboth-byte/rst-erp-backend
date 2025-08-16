#!/usr/bin/env bash
set -euo pipefail
AUTH_BASE=${1:-https://localhost:1213}
echo "Rotating keys at $AUTH_BASE ..."
curl -s -X POST "$AUTH_BASE/api/keys/rotate" | jq .
