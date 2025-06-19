using UnityEngine;
using UnityReviewSystem;

namespace UnityReviewSystem.Samples
{
    /// <summary>
    /// Example script showing how to integrate the review system into your game
    /// </summary>
    public class ExampleUsage : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private ReviewConfig _reviewConfig;
        
        [Header("Example Triggers")]
        [SerializeField] private bool _requestOnLevelComplete = true;
        [SerializeField] private bool _requestOnHighScore = true;
        [SerializeField] private bool _requestOnPositiveEvent = true;

        private void Start()
        {
            // Initialize the review system
            InitializeReviewSystem();
            
            // Subscribe to events
            SubscribeToEvents();
        }

        private void OnDestroy()
        {
            // Unsubscribe from events
            UnsubscribeFromEvents();
        }

        #region Initialization
        private void InitializeReviewSystem()
        {
            // Initialize with config
            ReviewManager.Instance.Initialize(_reviewConfig);
            
            // Start tracking this session
            ReviewTracker.StartSession();
            
            Debug.Log("[ExampleUsage] Review system initialized");
        }
        #endregion

        #region Example Trigger Points
        /// <summary>
        /// Call this when player completes a level
        /// </summary>
        public void OnLevelCompleted()
        {
            Debug.Log("[ExampleUsage] Level completed");
            
            if (_requestOnLevelComplete)
            {
                TryRequestReview("level completion");
            }
        }

        /// <summary>
        /// Call this when player achieves a new high score
        /// </summary>
        public void OnHighScoreAchieved()
        {
            Debug.Log("[ExampleUsage] High score achieved");
            
            if (_requestOnHighScore)
            {
                TryRequestReview("high score achievement");
            }
        }

        /// <summary>
        /// Call this after any positive game event
        /// </summary>
        public void OnPositiveGameEvent()
        {
            Debug.Log("[ExampleUsage] Positive game event");
            
            if (_requestOnPositiveEvent)
            {
                TryRequestReview("positive game event");
            }
        }

        /// <summary>
        /// Force review request (for testing)
        /// </summary>
        [ContextMenu("Force Request Review")]
        public void ForceRequestReview()
        {
            Debug.Log("[ExampleUsage] Forcing review request");
            ReviewManager.Instance.RequestReview(forceShow: true);
        }
        #endregion

        #region Review Logic
        private void TryRequestReview(string trigger)
        {
            Debug.Log($"[ExampleUsage] Checking review conditions after {trigger}");
            
            if (ReviewManager.Instance.ShouldShowReview())
            {
                Debug.Log($"[ExampleUsage] Requesting review after {trigger}");
                ReviewManager.Instance.RequestReview();
            }
            else
            {
                Debug.Log($"[ExampleUsage] Review conditions not met after {trigger}");
                Debug.Log($"[ExampleUsage] Current status: {ReviewTracker.GetTrackingDataSummary()}");
            }
        }
        #endregion

        #region Event Handling
        private void SubscribeToEvents()
        {
            ReviewManager.OnReviewFlowStarted += OnReviewFlowStarted;
            ReviewManager.OnReviewFlowCompleted += OnReviewFlowCompleted;
            ReviewManager.OnReviewFlowFailed += OnReviewFlowFailed;
        }

        private void UnsubscribeFromEvents()
        {
            ReviewManager.OnReviewFlowStarted -= OnReviewFlowStarted;
            ReviewManager.OnReviewFlowCompleted -= OnReviewFlowCompleted;
            ReviewManager.OnReviewFlowFailed -= OnReviewFlowFailed;
        }

        private void OnReviewFlowStarted()
        {
            Debug.Log("[ExampleUsage] Review flow started - pausing game or showing loading");
            
            // Example: Pause the game during review
            Time.timeScale = 0f;
        }

        private void OnReviewFlowCompleted(ReviewResult result)
        {
            Debug.Log($"[ExampleUsage] Review flow completed with result: {result}");
            
            // Resume game
            Time.timeScale = 1f;
            
            // Example: Track in analytics
            // AnalyticsManager.TrackEvent("review_completed", new { result = result.ToString() });
            
            // Example: Show thank you message
            if (result == ReviewResult.Completed)
            {
                ShowThankYouMessage();
            }
        }

        private void OnReviewFlowFailed(Google.Play.Review.ReviewErrorCode errorCode)
        {
            Debug.LogWarning($"[ExampleUsage] Review flow failed with error: {errorCode}");
            
            // Resume game
            Time.timeScale = 1f;
            
            // Example: Track in analytics
            // AnalyticsManager.TrackEvent("review_failed", new { error = errorCode.ToString() });
        }

        private void ShowThankYouMessage()
        {
            Debug.Log("[ExampleUsage] Showing thank you message to player");
            
            // Example: Show UI popup
            // UIManager.ShowPopup("Thank you for rating our game!");
        }
        #endregion

        #region Debug/Testing
        [ContextMenu("Show Current Status")]
        public void ShowCurrentStatus()
        {
            Debug.Log($"[ExampleUsage] Review Status:");
            Debug.Log($"  Initialized: {ReviewManager.Instance.IsInitialized}");
            Debug.Log($"  Should Show Review: {ReviewManager.Instance.ShouldShowReview()}");
            Debug.Log($"  Tracking Data: {ReviewTracker.GetTrackingDataSummary()}");
        }

        [ContextMenu("Reset Review Data")]
        public void ResetReviewData()
        {
            ReviewTracker.ResetAllData();
            Debug.Log("[ExampleUsage] Review data reset");
        }

        [ContextMenu("Simulate Multiple Sessions")]
        public void SimulateMultipleSessions()
        {
            for (int i = 0; i < 10; i++)
            {
                ReviewTracker.StartSession();
                ReviewTracker.EndSession();
            }
            Debug.Log("[ExampleUsage] Simulated 10 sessions");
        }
        #endregion
    }
}
