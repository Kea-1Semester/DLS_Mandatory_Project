#!/bin/bash

#################################
# Kubernetes Deployment Script  #
# Script to deploy all Kubernetes configurations and start port forwarding for necessary services.
#################################

# Run:
# bash kubernetes-deployment.sh

# To delete all deployments and port forwarding processes, run:
# bash kubernetes-deployment.sh --delete

###############################

# Function to wait for a pod to be ready
wait_for_pod() {
  local label=$1
  echo "Waiting for pod with label $label to be ready..."
  kubectl wait --for=condition=ready pod -l $label --timeout=600s
}

# Function to kill port forwarding processes
kill_port_forwarding() {
  if [ -f port_forward_pids.txt ]; then
    while read -r pid; do
      echo "Killing port forwarding process with PID $pid..."
      kill $pid
    done < port_forward_pids.txt
    rm port_forward_pids.txt
  fi
}

# Check for the --delete argument
if [ "$1" == "--delete" ]; then
  echo "Killing any existing port forwarding processes..."
  kill_port_forwarding
  echo "Deleting all existing deployments..."
  kubectl delete -R -f k8s/
  echo "All deployments deleted."
  exit 0
fi


# Apply all Kubernetes configurations from the directory recursively
echo "Applying Kubernetes configurations..."
kubectl apply -R -f k8s/

# Wait for the RabbitMQ service pod to be ready
wait_for_pod "app=rabbitmq"

# Start port forwarding for RabbitMQ management UI
# kubectl port-forward svc/rabbitmq 15672:15672 > /dev/null 2>&1 &
# RABBITMQ_PORT_FORWARD_PID=$!
# echo $RABBITMQ_PORT_FORWARD_PID >> port_forward_pids.txt
# echo "? Port forwarding for RabbitMQ management UI set up on port 15672."

kubectl port-forward svc/rabbitmq 15672:15672 > /dev/null 2>&1 &
RABBITMQ_PORT_FORWARD_PID=$!
if ! ps -p $RABBITMQ_PORT_FORWARD_PID > /dev/null; then
  echo "Failed to start port forwarding for RabbitMQ."
  exit 1
fi
echo $RABBITMQ_PORT_FORWARD_PID >> port_forward_pids.txt
echo "? Port forwarding for RabbitMQ management UI set up on port 15672."


# # Wait for the AuthService pod to be ready
# wait_for_pod "app=authservice"

# # Start port forwarding for AuthService
# kubectl port-forward svc/authservice 8084:8084 > /dev/null 2>&1 &
# AUTHSERVICE_PORT_FORWARD_PID=$!
# echo $AUTHSERVICE_PORT_FORWARD_PID >> port_forward_pids.txt
# echo "? Port forwarding for AuthService set up on port 8084."

# # Wait for the UserService pod to be ready
# wait_for_pod "app=userservice"

# # Start port forwarding for UserService
# kubectl port-forward svc/userservice 8086:8086 > /dev/null 2>&1 &
# USERSERVICE_PORT_FORWARD_PID=$!
# echo $USERSERVICE_PORT_FORWARD_PID >> port_forward_pids.txt
# echo "? Port forwarding for UserService set up on port 8086."

# Wait for the UI service pod to be ready
wait_for_pod "app=dlsmandatoryproject"
# Start port forwarding for UI service
kubectl port-forward svc/dlsmandatoryproject 8081:8081 > /dev/null 2>&1 &
UI_PORT_FORWARD_PID=$!
echo $UI_PORT_FORWARD_PID >> port_forward_pids.txt
echo "? Port forwarding for UI service set up on port 8080." 

# Open Chrome with different UI services
./scripts/open-chrome.sh

# Detach the script to keep port forwarding running
disown
