#!/usr/bin/env bash
# Per-boot startup for the RST ERP backend development infrastructure.
# Starts PostgreSQL, Redis and RabbitMQ (the services the APIs depend on),
# ensures the expected postgres password, waits for readiness, then returns.
# Safe to run repeatedly.
set -euo pipefail

echo ">> Starting PostgreSQL..."
# pg_ctlcluster returns non-zero if already running; that is fine.
sudo pg_ctlcluster 16 main start 2>/dev/null || true
for i in $(seq 1 30); do
  if sudo -u postgres pg_isready -q 2>/dev/null; then break; fi
  sleep 1
done
# Ensure the password the services expect (postgres/root).
sudo -u postgres psql -c "ALTER USER postgres WITH PASSWORD 'root';" >/dev/null 2>&1 || true

echo ">> Starting Redis..."
redis-cli ping >/dev/null 2>&1 || sudo redis-server /etc/redis/redis.conf --daemonize yes
for i in $(seq 1 30); do
  if redis-cli ping >/dev/null 2>&1; then break; fi
  sleep 1
done

echo ">> Starting RabbitMQ..."
sudo rabbitmqctl status >/dev/null 2>&1 || sudo rabbitmq-server -detached
for i in $(seq 1 60); do
  if sudo rabbitmqctl status >/dev/null 2>&1; then break; fi
  sleep 2
done

echo ">> Infrastructure status:"
sudo -u postgres pg_isready 2>&1 || true
echo -n "redis: "; redis-cli ping 2>&1 || true
sudo rabbitmqctl status >/dev/null 2>&1 && echo "rabbitmq: running" || echo "rabbitmq: NOT running"

echo ">> Start complete."
