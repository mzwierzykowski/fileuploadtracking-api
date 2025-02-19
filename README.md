# 📂 FileUploadTracking - Microservices Project

🚀 A .NET 8 Microservices-based MVP of file upload tracking system with RabbitMQ messaging

## 📖 Project Overview

This project is a file upload tracking system where:

- Upload Process Service allows users to upload files and triggers a completion event.
- RabbitMQ is used as the event broker.
- Notification Service listens to RabbitMQ and notifies user about completed process.

## 💡 How It Works

- A user creates file upload process via the Upload Process Service.
- User simulates file upload for specified process identifier (trackingId).
- Once 3 files are uploaded, the service publishes an event to RabbitMQ.
- Notification Service listens for the event and triggers a callback URL to notify external systems.

## 🛠️ Technologies Used

- .NET 8 (Minimal APIs, Worker Service)
- RabbitMQ (for event-driven communication)
- Docker & Docker Compose (for containerization)
- FluentValidation (for request validation)
- HttpClient (for callbacks in Notification Service)
- Swagger (for API testing)

## 📜 Architecture

![image](https://github.com/user-attachments/assets/95d27704-feac-47c2-b104-c5ea72f613f1)

This project follows a microservices architecture with the following components:
- Upload Service: 	Manages file uploads and publishes a message when all required files are uploaded.
- RabbitMQ:	Message broker used to facilitate communication between services.
- Notification Service:	Listens for completed upload events and triggers a call to callback URL.

## 🚀 Getting Started

### 1️⃣ Prerequisites

Before running the project, ensure you have the following installed:

- Docker
- Docker Compose
- .NET 8 SDK

### 2️⃣ Running the Project

Run the application using Docker Compose:

    docker-compose up --build

This will:

- Start RabbitMQ on port 5672 (management UI: http://localhost:15672)
- Start Upload Process Service on http://localhost:5000
- Start Notification Service on http://localhost:5001

After containers are up and running to interact with the system visit: http://localhost:5000/swagger/index.html

### 3️⃣ Stopping the Project

To stop the services:

    docker-compose down

## 🔧 Configuration

RabbitMQ Settings

The application uses RabbitMQ for event-driven communication. Configuration is set in appsettings.json or environment variables.

Example appsettings.json:
    
    {
      "RabbitMQ": {
        "HostName": "rabbitmq",
        "Port": "5672",
        "UserName": "guest",
        "Password": "guest"
      }
    }

Environment Variables

If needed, update docker-compose.yml to change environment variables.

## 📡 API Endpoints

Once running, you can access Swagger UI to test the API:

    Upload Process Service → http://localhost:5000/swagger

### 📌 Upload Process Service Endpoints
| Method | Endpoint                                      | Description                          |
|--------|-----------------------------------------------|--------------------------------------|
| POST   | /process                                      | Create a new file upload process    |
| POST   | /process/{trackingId}/users/{userId}/files    | Upload a file for a given process   |
| GET    | /process/{trackingId}                         | Get process details                 |



