FROM rabbitmq:3-management-alpine

# Copy custom config
COPY rabbitmq.conf /etc/rabbitmq/rabbitmq.conf

# Enable management plugin
RUN rabbitmq-plugins enable --offline rabbitmq_management

# Create a custom user and set permissions
RUN rabbitmqctl add_user admin admin123 || true && \
    rabbitmqctl set_user_tags admin administrator || true && \
    rabbitmqctl set_permissions -p / admin ".*" ".*" ".*" || true && \
    rabbitmqctl delete_user guest || true

# Expose ports
EXPOSE 5672 15672

CMD ["rabbitmq-server"]