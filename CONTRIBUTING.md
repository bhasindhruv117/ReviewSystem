# Contributing to Unity Review System

Thank you for your interest in contributing to the Unity Review System! We welcome contributions from the community.

## How to Contribute

### Reporting Issues
- Use the GitHub issue tracker to report bugs
- Include Unity version, platform, and detailed reproduction steps
- Provide error logs and stack traces when applicable

### Suggesting Features
- Open an issue with the "enhancement" label
- Describe the feature and its use case
- Explain how it fits with the existing architecture

### Submitting Changes
1. Fork the repository
2. Create a feature branch: `git checkout -b feature/your-feature-name`
3. Make your changes following the coding standards
4. Test your changes thoroughly
5. Commit with clear, descriptive messages
6. Push to your fork and submit a pull request

## Coding Standards

### C# Style Guidelines
- Follow Microsoft C# coding conventions
- Use meaningful variable and method names
- Add XML documentation comments for public APIs
- Keep methods focused and concise
- Use regions to organize code sections

### Code Organization
- Put generic functionality in the `Runtime` folder
- Put editor tools in the `Editor` folder  
- Use appropriate namespaces (`UnityReviewSystem` for core, `UnityReviewSystem.Editor` for editor)
- Follow the established pattern of Manager → Config → Tracker → Helper

### Testing
- Test on actual Android devices when possible
- Verify functionality works in Unity Editor
- Test with different configuration settings
- Ensure no regression in existing functionality

## Architecture Guidelines

### Modularity
- Keep the system game-agnostic
- Use dependency injection where appropriate
- Minimize coupling between components
- Follow the single responsibility principle

### Extensibility
- Design for easy extension to other platforms
- Use interfaces where future implementations are expected
- Document extension points for developers

### Performance
- Minimize memory allocations
- Use coroutines for async operations
- Cache frequently accessed data
- Avoid Update() methods where possible

## Pull Request Process

1. Ensure your code follows the coding standards
2. Update documentation for any new features
3. Add or update tests as needed
4. Ensure all existing tests still pass
5. Update the CHANGELOG.md with your changes
6. Request review from maintainers

## Development Setup

1. Clone the repository
2. Open in Unity 2020.3 or later
3. Install Google Play Review package dependency
4. Build and test on Android device

## Questions?

Feel free to open an issue for questions about contributing or reach out to the maintainers directly.

Thank you for contributing! 🎮
