# Unity Review System

A generic wrapper for Google Play Review API that can be used across any Unity games.

## Features

- 🎮 **Game-agnostic**: Works with any Unity game
- 📊 **Smart Triggering**: Configurable conditions for when to show reviews
- 🔄 **Session Tracking**: Automatic session and playtime tracking
- ⚙️ **Configurable**: ScriptableObject-based configuration
- 🧪 **Testing Tools**: Built-in testing and debugging tools
- 📱 **Platform Support**: Android (Google Play) with extensible architecture

## Installation

### Option 1: Unity Package Manager (Recommended)
1. Open Unity Package Manager
2. Click "+" and select "Add package from git URL"
3. Enter: `https://github.com:bhasindhruv117/ReviewSystem.git`

### Option 2: Manual Installation
1. Download this repository
2. Copy the `ReviewSystem` folder to your `Assets/Scripts/` directory

## Quick Start

### 1. Basic Setup
```csharp
// Initialize the review system
ReviewManager.Instance.Initialize();

// Request review when conditions are met
if (ReviewManager.Instance.ShouldShowReview())
{
    ReviewManager.Instance.RequestReview();
}
```

### 2. Automatic Setup (Recommended)
1. Add `ReviewSystemHelper` component to a GameObject in your scene
2. Create a `ReviewConfig` asset (Right-click → Create → Review System → Review Config)
3. Assign the config to the helper component
4. Enable "Auto Request On Start" if desired

### 3. Configuration
Create a `ReviewConfig` asset to customize:
- **Minimum Sessions**: Number of sessions before showing review
- **Minimum Play Time**: Required playtime in seconds
- **Cooldown Days**: Days between review requests
- **Allow Multiple Reviews**: Whether to show review multiple times

## Advanced Usage

### Manual Session Tracking
```csharp
// Start a session
ReviewTracker.StartSession();

// End a session
ReviewTracker.EndSession();

// Get tracking data
int sessions = ReviewTracker.GetSessionCount();
float playTime = ReviewTracker.GetTotalPlayTime();
```

### Event Handling
```csharp
// Subscribe to events
ReviewManager.OnReviewFlowStarted += OnReviewStarted;
ReviewManager.OnReviewFlowCompleted += OnReviewCompleted;
ReviewManager.OnReviewFlowFailed += OnReviewFailed;

private void OnReviewStarted()
{
    Debug.Log("Review flow started");
}

private void OnReviewCompleted(ReviewResult result)
{
    Debug.Log($"Review completed: {result}");
}

private void OnReviewFailed(ReviewErrorCode error)
{
    Debug.LogError($"Review failed: {error}");
}
```

### Custom Conditions
```csharp
// Check if conditions are met
bool shouldShow = ReviewManager.Instance.ShouldShowReview();

// Force show review (for testing)
ReviewManager.Instance.RequestReview(forceShow: true);
```

## Editor Tools

Access the Review System Manager via `Tools → Review System → Review System Manager` to:
- Configure review settings
- View current tracking data
- Test review functionality
- Reset tracking data
- Monitor review status in real-time

## API Reference

### ReviewManager
- `Initialize(ReviewConfig config)`: Initialize with configuration
- `RequestReview(bool forceShow)`: Request review from user
- `ShouldShowReview()`: Check if conditions are met
- `IsInitialized`: Whether manager is initialized
- `IsReviewFlowActive`: Whether review flow is currently active

### ReviewTracker
- `StartSession()`: Start tracking session
- `EndSession()`: End current session
- `GetSessionCount()`: Get total sessions
- `GetTotalPlayTime()`: Get total playtime
- `HasUserReviewed()`: Check if user reviewed
- `ResetAllData()`: Reset all tracking data

### ReviewConfig
- `MinimumSessions`: Required sessions before review
- `MinimumPlayTimeSeconds`: Required playtime before review
- `CooldownDays`: Days between review requests
- `AllowMultipleReviews`: Allow multiple review requests
- `TestMode`: Bypass all conditions for testing

## Testing

### In Editor
1. Open `Tools → Review System → Review System Manager`
2. Enter Play Mode
3. Use "Force Request Review" to test
4. Monitor tracking data in real-time

### Build Testing
1. Set `TestMode = true` in ReviewConfig
2. Build and test on device
3. Use `ForceRequestReview()` for immediate testing

## Best Practices

1. **Timing**: Show review after positive game experiences
2. **Frequency**: Don't spam users with review requests
3. **Conditions**: Set reasonable minimum sessions/playtime
4. **Testing**: Always test on actual devices
5. **Analytics**: Track review request success/failure rates

## Platform Support

- ✅ **Android (Google Play)**: Full support via Google Play Review API
- 🔄 **iOS (App Store)**: Planned for future release
- 🔄 **Other Platforms**: Extensible architecture for additional platforms

## Troubleshooting

### Common Issues
1. **Review not showing**: Check if conditions are met and device supports in-app reviews
2. **Tracking not working**: Ensure ReviewSystemHelper is in scene and session tracking is enabled
3. **Build errors**: Verify Google Play Review package is installed

### Debug Mode
Enable debug logs in ReviewConfig to see detailed information about:
- Session tracking
- Condition checking
- Review flow execution
- Error details

## License

MIT License - see LICENSE.md for details

## Contributing

Contributions welcome! Please read CONTRIBUTING.md for guidelines.

## Support

- 🐛 **Issues**: [GitHub Issues](https://github.com:bhasindhruv117/ReviewSystem/issues)
- 📖 **Documentation**: [Wiki](https://github.com:bhasindhruv117/ReviewSystem/wiki)
- 💬 **Discussions**: [GitHub Discussions](https://github.com:bhasindhruv117/ReviewSystem/discussions)
