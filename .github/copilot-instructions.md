# .NET Todo Application - Development Guidelines

## Purpose of the Application
The .NET Todo application is a command-line utility inspired by the Todo.txt shell script. It provides a robust and extensible way to manage tasks using a plain text file. The application aims to remain faithful to the original Todo.txt functionality while leveraging modern .NET features.

## Coding Style
- **Language**: C#
- **Framework**: .NET Core
- **Conventions**: The project adheres to standard C# coding conventions, including PascalCase for public members and camelCase for private members.
- **Dependency Injection**: The application uses Microsoft.Extensions.DependencyInjection for managing dependencies.
- **Async/Await**: Asynchronous programming is used extensively for I/O-bound operations.
- **Command Pattern**: Commands and queries are implemented using the MediatR library.

## Architecture
The application follows a modular clean architecture with the following layers:

1. **Domain Layer** (`todo.domain`):
   - Contains core entities like `TaskItem` and interfaces such as `ITaskFile` and `ITaskConfiguration`.
   - Implements business logic and rules.

2. **Application Layer** (`todo.application`):
   - Implements commands and queries using the MediatR library.
   - Provides handlers for operations like adding, deleting, and listing tasks.

3. **Infrastructure Layer** (`todo.infrastructure`):
   - Handles persistence and configuration management.
   - Implements interfaces defined in the domain layer.

4. **Presentation Layer** (`todo`):
   - Contains the `TodoApplication` class, which serves as the entry point.
   - Implements CLI commands using `System.CommandLine`.

## NuGet Packages Used
- **MediatR**: Implements the mediator pattern for handling commands and queries.
- **System.CommandLine**: Provides a framework for building command-line applications.
- **ColoredConsole**: Adds colorized output to the console.
- **NUnit**: Used for unit testing.
- **FluentAssertions**: Provides a fluent syntax for writing assertions in tests.

## Testing Style
- **Framework**: NUnit
- **Mocking**: Custom mocks (e.g., `TaskFileMock`) are used to simulate dependencies.
- **Structure**: Tests are organized by layer and feature (e.g., `Commands`, `Queries`).
- **Assertions**: FluentAssertions is used for readable and expressive assertions.
- **Setup**: Common setup logic is placed in `[SetUp]` methods.

## CI/CD Workflow
The project includes a GitHub Actions workflow (`.github/workflows/dotnet-core.yml`) that:
- Restores dependencies.
- Builds the project in Release mode.
- Runs unit tests.
- Packages the application as a NuGet package.
- Publishes the package to GitHub Packages and NuGet.org.
