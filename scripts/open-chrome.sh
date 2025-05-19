# Open default browser with different UI services
echo "Opening default browser with different UI services..."
# start http://localhost:30084 & # AuthService NodePort
# start http://localhost:30082 & # UserService NodePort
# start http://localhost:30086 & # ApiGateway NodePort
start http://localhost:30008 & # RabbitMQ Management UI
start http://localhost:30001 & # Blazor UI NodePort