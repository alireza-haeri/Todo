#!/usr/bin/env bash
set -e

# Attempt to ensure data protection key directory is writable
if [ -d "/home/app/.aspnet/DataProtection-Keys" ]; then
  chown -R app:app /home/app/.aspnet/DataProtection-Keys || true
fi

exec dotnet Todo.Api.dll "$@"
