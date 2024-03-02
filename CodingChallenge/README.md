# Coding Challenge Solution

This project provides an efficient solution to interact with the Hacker News API without overloading it, ensuring a smooth and responsive experience for users. By caching API responses, we minimize the frequency of requests sent to Hacker News, balancing the load and improving performance.

## Key Features

- **Caching Mechanism**: Responses from the Hacker News API are cached for 60 seconds. This reduces the number of requests sent to the API, preventing overload and improving response time for subsequent requests.
- **Swagger UI Integration**: Easily test the API endpoints through a user-friendly interface provided by Swagger UI.
- **Support for Multiple Request Methods**: The API can be tested using Swagger UI, Postman, or a simple curl command.

## Getting Started

### Prerequisites

- .NET SDK installed on your machine.

### Running the Project

1. Clone this repository to your local machine.
2. Navigate to the `CodingChallenge` folder in your terminal.
3. Run the following command to start the project:
```
dotnet run
```

### Testing the API

You can test the API using one of the following methods:

- **Swagger UI**: Navigate to `http://localhost:5090/swagger` in your browser to access the Swagger UI interface.
- **Postman**: Send a GET request to `http://localhost:5090/api/stories/3`.
- **Curl Command**: Use the following curl command:
```bash
curl -X 'GET' \
 'http://localhost:5090/api/stories/3' \
 -H 'accept: */*'
```
## Future Enhancements

- **Dockerize the Application**: Containerize the application to simplify deployment and ensure consistency across different environments.

- **Add Code Comments**: Improve code readability and maintenance by adding comments throughout the codebase.

- **Expand Testing**: Create a new project within the solution that contains unit tests and integration tests to ensure code quality and reliability.
