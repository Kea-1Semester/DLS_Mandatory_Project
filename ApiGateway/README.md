# API Gateway
This API Gateway serves as a single entry point to route requests to the appropriate microservices, such as `UserService` and `AuthService`.


## Prerequisites

- Ensure Docker is installed and running.
- Make sure the microservices (`UserService` and `AuthService`) are up and running.
- The API Gateway is configured to use Ocelot for routing.

1. **Using Docker Compose**:
   - Run the following command to start the API Gateway along with other services:

```bash
docker-compose up --build
```


## Available Routes

The API Gateway routes requests to the appropriate downstream services based on the configuration in [ocelot.json](./ocelot.json).

### User Service Routes 

- **Create User GUID**:
  - **Endpoint**: `GET /user/CreateGuid`
  - **Downstream Service**: [UserService](./ocelot.json)
  - **Example**:

```bash
    curl -X GET http://localhost:8087/user/CreateGuid
```

- **Create User**:
  - **Endpoint**: `POST /user/{guid}`
  - **Downstream Service**: [UserService](./ocelot.json)
  - **Example**:

```bash
    curl -X POST https://localhost:7289/User?guid=asdsad-asd231-asdad-21321sad
```

### Auth Service Routes

- **Login**:
    - **Endpoint**: `POST /Auth/Login`
    - **Downstream Service**: [AuthService](./ocelot.json)
    - **Example**:

```bash
    curl -X POST http://localhost:8087/Auth/Login -H \ "Content-Type: application/json" -d \
    '{
        "email":"example@outlook.com",
        "password":"pass"
    }'
```

## Swagger Documentation

Swagger documentation is provided to enhance the developer experience. You can access it after running the services using Docker Compose.


### Steps to Run

- Use the following command to start the API Gateway and other services:

```bash
docker-compose up -d --build
```

- **Auth Service Swagger**: [authserviceSwagger](http://localhost:8087/authserviceSwagger)

- **User Service Swagger**: [userserviceSwagger](http://localhost:8084/index.html)