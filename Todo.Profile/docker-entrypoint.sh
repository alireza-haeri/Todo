#!/usr/bin/env bash
set -e

if [ -d "/home/app/.aspnet/DataProtection-Keys" ]; then
  chown -R app:app /home/app/.aspnet/DataProtection-Keys || true
fi

exec dotnet Todo.Profile.dll "$@"
