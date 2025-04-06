# auth-rbac-api_c#

Authentication &amp; RBAC

# auth-rbac-api_c#

## Overview

This project is an authentication and role-based access control (RBAC) API built with C# and ASP.NET Core. It provides secure authentication and authorization features, allowing users to register, log in, and access resources based on their roles and permissions.

## Features

- User registration with password hashing.
- JWT-based authentication.
- Role-based access control (RBAC).
- S3 bucket for Image uploads
- API endpoints for managing users, roles, resources, and permissions.
- Secure and scalable architecture with .NET 8.0.

## Technologies Used

- C# (.NET 8.0)
- ASP.NET Core
- Entity Framework Core (EF Core)
- PostgreSQL
- JWT Authentication
- Docker, Docker-compose (for containerization)

## Setup

### Prerequisites

- .NET 8.0 SDK
- PostgreSQL (for database)
- Docker (optional, if you want to run the app in a container)

### Running the Application Locally

1. Clone this repository:
   ```bash
   git clone https://github.com/kennykay20/auth-rbac-api.git
   ```
2. Navigate to the directory ``` cd auth-rbac-api_csharp

3. Set up the PostgreSQL database:

- Make sure you have a running PostgreSQL instance.
- Create a database for the app or modify the appsettings.json to use an existing database.
- Run the migrations to set up the schema.

4. run dotnet build to build and dotnet run to start the app, or run dotnet run to do both actions

### Running with Docker

## To run the application using Docker, follow these steps:

1. Build the Docker image:

- docker compose build

2. Run the application:

- docker compose up

- The app will be available at http://localhost:5284

## API Endpoints

- Authentication Endpoints
- POST /api/v1/authentication/register - Register a new user.
- body { "FirstName", "LastName", "Email", "Password"}
- POST /api/v1/authentication/login - Log in with credentials and receive a JWT token.
- body {"Email", "Password"}

# Example request for register

- POST /api/v1/authentication/register
  Content-Type: application/json

{
"FirstName": "test1",
"LastName": "test2",
"Email": "test@gmail.com",
"Password": "Securepassword123"
}

# Example response

{
"Success": true,
"Status": 201,
"Message": "New User added successfully, activate your account",
"Data": "user-data without password in return"
}

- The message refers you to update and validate your account with an OTP sent to your email within 10mins, without that you cannot login as an active user.

# Example request for login

- POST /api/v1/authentication/login
  Content-Type: application/json

{
"Email": "test@gmail.com",
"Password": "Securepassword123"
}

# Example response

{
"Success": true,
"Status": 200,
"Message": "Login successfully",
"AccessToken": "your-jwt-access-token-here",
"RefreshToken": "your-jwt-refresh-token-here",
}
