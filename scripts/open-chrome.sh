# Open default browser with different UI services
echo "Opening default browser with different UI services..."
start http://localhost:8084 & # AuthService NodePort
start http://localhost:8086 & # UserService NodePort
start http://localhost:15672 & # RabbitMQ Management UI