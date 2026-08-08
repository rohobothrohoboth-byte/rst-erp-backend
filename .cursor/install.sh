#!/usr/bin/env bash
# Idempotent dependency refresh for the RST ERP backend.
# Runs after the repository is checked out. Restores and builds the full
# .NET solution and ensures a local HTTPS development certificate exists.
set -euo pipefail

export DOTNET_ROOT="${DOTNET_ROOT:-/usr/local/dotnet}"
export PATH="$DOTNET_ROOT:$PATH"
export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_NOLOGO=1

# Resolve repository root (this script lives in <repo>/.cursor).
REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$REPO_ROOT"

echo ">> dotnet: $(dotnet --version)"

echo ">> Restoring solution (RST_ERP.sln)..."
dotnet restore RST_ERP.sln

# The test project is not part of the solution file; restore it explicitly.
echo ">> Restoring test project (Svc.Auth.Tests)..."
dotnet restore tests/Svc.Auth.Tests/Svc.Auth.Tests.csproj

echo ">> Building solution (Debug)..."
dotnet build RST_ERP.sln -c Debug --no-restore

echo ">> Ensuring HTTPS development certificate..."
dotnet dev-certs https || true

echo ">> Install complete."
