# Changelog

All notable changes to the Unity Review System will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2024-06-19

### Added
- Initial release of Unity Review System
- Generic wrapper for Google Play Review API
- ReviewManager singleton for centralized review management
- ReviewConfig ScriptableObject for configuration
- ReviewTracker static utility for session and playtime tracking
- ReviewSystemHelper component for automatic setup
- Event system for review flow monitoring
- Editor tools and manager window
- Comprehensive documentation and examples
- Assembly definitions for proper dependency management
- Smart triggering based on configurable conditions:
  - Minimum session count
  - Minimum playtime
  - Cooldown period
  - Multiple review prevention
- Session tracking with automatic start/end detection
- PlayerPrefs-based data persistence
- Test mode for development and debugging
- Unity Package Manager support

### Features
- 🎮 Game-agnostic design
- 📊 Smart triggering conditions
- 🔄 Automatic session tracking
- ⚙️ ScriptableObject configuration
- 🧪 Built-in testing tools
- 📱 Android (Google Play) support
- 🎯 Event-driven architecture
- 🛠️ Comprehensive editor tools

### Technical Details
- Minimum Unity version: 2020.3
- Dependencies: Google Play Review package
- Platforms: Android (Google Play)
- Architecture: Singleton pattern with static utilities
- Data persistence: PlayerPrefs
- Assembly definitions: Runtime and Editor separated

### Documentation
- README.md with comprehensive setup guide
- API reference documentation
- Best practices and troubleshooting
- Example usage patterns
- Editor tool documentation
