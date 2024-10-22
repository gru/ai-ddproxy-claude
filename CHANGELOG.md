# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.1.0] - 2024-10-22

### Added
- Initial release of AI.DaDataProxy microservice
- Proxy functionality for DaData API requests
- Redis-based caching system for API responses
- Rate limiting for DaData API requests
- Special handling for legal entity requests by INN
- Configurable cache durations for different request types
- Serilog integration for logging to console and file
- Swagger UI for API documentation
- Docker and Docker Compose support for easy deployment
- Feature management using Microsoft.FeatureManagement
- Custom error handling and problem details
- Unit tests for caching functionality

### Changed
- N/A (Initial release)

### Deprecated
- N/A (Initial release)

### Removed
- N/A (Initial release)

### Fixed
- N/A (Initial release)

### Security
- Implemented user secrets for storing sensitive configuration data
- Added HTTPS redirection for secure communication
