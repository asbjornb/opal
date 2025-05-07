# Opal: Operational Actions Library

<!-- Shields to be added once packages and CI are live -->
<!-- 
[![NuGet](https://img.shields.io/nuget/v/Opal.Client.svg)](https://www.nuget.org/packages/Opal.Client/)
[![Build Status](https://img.shields.io/github/actions/workflow/status/opal-project/opal/CI.yml)](https://github.com/opal-project/opal/actions)
-->

[![License](https://img.shields.io/github/license/opal-project/opal.svg)](LICENSE)

Opal is a lightweight framework for exposing internal operational methods from .NET services through reflection and centralized registration. It enables teams to perform ad hoc debugging, reruns, maintenance tasks, or manual overrides without modifying public APIs or writing temporary scripts.

## 🔍 Why Use Opal?

Every service ecosystem eventually needs operational tools for maintenance, debugging, and support. Opal offers a standardized approach to expose these capabilities without:

- Writing one-off scripts that quickly become outdated
- Adding temporary API endpoints that remain in production
- Deploying code changes for every operational need
- Direct database access that bypasses business logic

## 🧱 Architecture

Opal consists of two main components:

1. **Opal.Client** – NuGet package for your backend services to expose operational methods
2. **Opal.Server** – A centralized gRPC server with optional UI that discovers and executes methods

![Opal Architecture](https://via.placeholder.com/800x400?text=Opal+Architecture+Diagram)

### How It Works

1. Developers mark internal methods with `[Callable]` attributes
2. Services register these methods with Opal.Server at startup
3. Operations/Support teams can discover and invoke methods through Opal.Server
4. Opal.Server forwards calls to the appropriate service
5. All calls are executed within the proper application context

## 🔧 Getting Started

### 1. Install Package

```bash
dotnet add package Opal.Client
```

### 2. Mark Operational Methods

```csharp
using Opal.Client.Attributes;

public class UserOperations
{
    private readonly IUserRepository _userRepository;
    private readonly ICache _cache;
    
    public UserOperations(IUserRepository userRepository, ICache cache)
    {
        _userRepository = userRepository;
        _cache = cache;
    }
    
    [Callable("Triggers a manual user reimport")]
    public async Task<string> ReimportUser(int userId)
    {
        // Method implementation that leverages existing business logic
        var result = await _userRepository.Reimport(userId);
        return $"User {userId} reimported successfully: {result}";
    }
    
    [Callable("Clears the user cache", ReadOnly = false)]
    public void ClearUserCache()
    {
        _cache.Clear();
        return "User cache cleared";
    }
    
    [Callable("Get user information", ReadOnly = true)]
    public async Task<UserDetails> GetUserDetails(int userId)
    {
        return await _userRepository.GetUserDetailsAsync(userId);
    }
}
```

### 3. Register with Opal in Startup

```csharp
// In Program.cs or Startup.cs
using Opal.Client;

// Add Opal services
services.AddOpal("user-service", options =>
{
    options.RegistryUrl = "https://opal.mycompany.local";
    options.ServiceName = "User Service";
    options.ServiceGroup = "Account Management";
    options.AutoDiscoverCallables = true;  // Auto-scan assemblies for [Callable] methods
});
```

### 4. Run Opal.Server

```bash
# Using Docker
docker run -p 5001:5001 ghcr.io/opal-project/opal-server:latest

# Or with Docker Compose
docker-compose up opal-server
```

## 🛠️ Features

- **Method Discovery** - Automatically identify and register operational methods
- **Parameter Validation** - Basic validation for method parameters
- **Access Controls** - Simple role-based permissions for method execution
- **Audit Logging** - Track who executed what methods and when
- **Discoverability** - Easy way to see available methods and their documentation

## 💡 Use Cases

- **Reprocessing** - Manually reprocess a failed item
- **Data Repair** - Fix inconsistent data without direct DB access
- **Diagnostics** - Run health checks and diagnostics on demand
- **Cache Management** - Clear or inspect caches
- **Configuration** - View or update runtime configurations
- **Metrics** - Gather real-time internal metrics
- **Testing** - Trigger specific behaviors for testing
- **Support Tools** - Enable support teams to resolve issues

## 👩‍💻 Examples

### Simple Parameter Types

```csharp
[Callable("Send welcome email")]
public async Task SendWelcomeEmail(string email)
{
    await _emailService.SendWelcomeEmail(email);
    return $"Welcome email sent to {email}";
}
```

### Complex Parameter Types

```csharp
[Callable("Create test user")]
public async Task<int> CreateTestUser(UserCreationRequest request)
{
    var userId = await _userService.CreateUser(request);
    return userId;
}

public class UserCreationRequest
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string[] Roles { get; set; }
}
```

## 🔐 Security

Opal is designed with security in mind:

- **Authentication** - Integrates with your existing auth systems
- **Authorization** - Role-based access to methods
- **ReadOnly Flag** - Mark methods that don't modify state
- **Service Isolation** - Services only expose explicitly marked methods
- **Audit Trail** - Log all method executions with parameters
- **Secure Defaults** - Methods are not exposed by default

## 🛣️ Roadmap

- **Web UI** - Simple interface for discovering and executing methods
- **Method Categories** - Group related operational methods
- **Parameter Suggestions** - Basic parameter type hints
- **Result Formatting** - Simple formatting of common return types
- **Bulk Operations** - Execute methods across multiple services
- **Multi-tenancy** - Support for multi-tenant environments

## 📦 Project Components

- `Opal.Client` – Package containing attributes, method reflection and registration
- `Opal.Server` – gRPC server for receiving registrations and triggering method calls
- `Opal.Proto` – Shared gRPC definitions
- `Opal.UI` – Optional web UI for Opal.Server
- `samples/SampleService` – Example implementation in a demo project

## 🤝 Contributing

Contributions are welcome! Please see our [Contributing Guide](CONTRIBUTING.md) for details.

1. Fork the repository
2. Create a feature branch
3. Add your changes
4. Run tests
5. Submit a pull request

## 🐳 Deployment

### Docker

```bash
# Opal Server
docker run -p 5001:5001 -v ./config:/app/config ghcr.io/opal-project/opal-server:latest

# With UI
docker run -p 5001:5001 -p 8080:80 -v ./config:/app/config ghcr.io/opal-project/opal-server-ui:latest
```

### Kubernetes

```bash
kubectl apply -f https://raw.githubusercontent.com/opal-project/opal/main/deploy/kubernetes/opal.yaml
```

## 📄 License

Opal is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 📚 Resources

- [Documentation](https://opal-project.github.io/docs)
- [API Reference](https://opal-project.github.io/api)
- [Samples](https://github.com/opal-project/opal/tree/main/samples)

---

Built with ❤️ for .NET developers who need operational tooling they can trust.