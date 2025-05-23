# DLS_Mandatory_Project

## Microservice Architecture Diagram
![alt text](/imgs/MicroService%20DLS-V01.6.png "Microservice Architecture Diagram")

## How to Run the Application

To run the application, use the following command:

```sh
docker-compose up -d --build
```

To clear images and volumes, use the following command:

```sh
docker-compose down -v
```

## How to Test the Frontend

To test the frontend, navigate to the `Websecurity` folder and open the HTML file that demonstrates the frontend in your browser.  
The HTML file demonstrates:

- [Registering the user](./webSecurity/index.html)
  - Vulnerable
  - Non-vulnerable
- Getting the user
  - [Vulnerable](./webSecurity/showUser_vulnerable.html)
  - [Non-vulnerable](./webSecurity/ShowUser_preventXSS.html)

All files are static.

## How to Test the Backend
The rest of the system has been tested with Postman, including login and testing authorization and authentication.

To navigate to the Postman collection file, follow this path in your project:

[Postman_json_file/Security in web development.postman_collection.json](./Postman_json_file/Security%20in%20web%20development.postman_collection.json)
