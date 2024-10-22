# AI.DaDataProxy

## Overview
AI.DaDataProxy is a microservice that serves as a proxy for the DaData API. It provides request caching, rate limiting, and special handling for certain types of requests, such as searching for legal entities by INN (Taxpayer Identification Number).

## Table of Contents
- [AI.DaDataProxy](#aidadataproxy)
  - [Overview](#overview)
  - [Table of Contents](#table-of-contents)
  - [Features](#features)
  - [Requirements](#requirements)
  - [Getting Started](#getting-started)
    - [Installation](#installation)
    - [Configuration](#configuration)
  - [Usage](#usage)
    - [Local Development](#local-development)
    - [Docker Compose](#docker-compose)
  - [API Documentation](#api-documentation)
  - [Testing](#testing)
  - [Deployment](#deployment)
  - [Monitoring and Logging](#monitoring-and-logging)
  - [Contributing](#contributing)
  - [License](#license)

## Features
- Proxying requests to the DaData API
- Caching requests and responses using Redis
- Rate limiting for DaData API requests
- Special handling of requests for legal entities by INN
- Configurable cache lifetime for different types of requests

## Requirements
- .NET 8 SDK
- Docker and Docker Compose
- Redis

## Getting Started

### Installation
1. Clone the repository:
   ```
   git clone [your-repository-url]
   ```
2. Navigate to the project directory:
   ```
   cd AI.DaDataProxy
   ```
3. Restore dependencies:
   ```
   dotnet restore
   ```

### Configuration
1. Update the connection strings in `appsettings.json` and `appsettings.Development.json`.
2. Configure DaData and caching parameters in `appsettings.json`.
3. For local development, use user secrets to store sensitive data.

## Usage

### Local Development
Run the application:
```
dotnet run --project Src/AI.DaDataProxy.Host
```

### Docker Compose
Run the application using Docker Compose:
```
docker-compose up
```

## API Documentation
The API documentation is available via Swagger UI when running the application in development mode. Access it at:

```
http://localhost:5000/swagger
```

[Include any additional API documentation or link to external API docs]

## Testing
To run the tests:

```
dotnet test
```

[Include information about the types of tests, test coverage, etc.]

## Deployment
[Provide instructions or links to deployment guides for various environments (staging, production, etc.)]

## Monitoring and Logging
This microservice uses Serilog for logging. Logs are written to both console and file.

[Include information about any monitoring tools, log aggregation services, etc.]

## Contributing
[Include guidelines for contributing to the project, code of conduct, etc.]

## License
[Specify the license under which your microservice is released]
