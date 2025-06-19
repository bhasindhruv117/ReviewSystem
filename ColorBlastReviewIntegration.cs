using UnityEngine;
using UnityReviewSystem;

namespace ColorBlast
{
    /// <summary>
    /// Integration class for Color Blast game to use the Unity Review System
    /// This follows the same pattern as your analytics system
    /// </summary>
    public class ColorBlastReviewIntegration : MonoBehaviour
    {
        [Header("Color Blast Review Configuration")]
        [SerializeField] private ReviewConfig _reviewConfig;
        
        [Header("Game-Specific Triggers")]
        [SerializeField] private bool _requestOnLevelComplete = true;
        [SerializeField] private bool _requestOnHighScore = true;
        [SerializeField] private bool _requestOnComboAchieved = true;
        [SerializeField] private int _highScoreThreshold = 1000;
        [SerializeField] private int _comboThreshold = 5;

        private static ColorBlastReviewIntegration _instance;
        public static ColorBlastReviewIntegration Instance => _instance;

        #region Unity Lifecycle
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeReviewSystem();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            // Start tracking this session
            ReviewTracker.StartSession();
            
            // Subscribe to game events if needed
            SubscribeToGameEvents();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
                ReviewTracker.EndSession();
            }
            else
            {
                ReviewTracker.StartSession();
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                ReviewTracker.EndSession();
            }
            else
            {
                ReviewTracker.StartSession();
            }
        }

        private void OnDestroy()
        {
            ReviewTracker.EndSession();
            UnsubscribeFromGameEvents();
        }
        #endregion

        #region Initialization
        private void InitializeReviewSystem()
        {
            ReviewManager.Instance.Initialize(_reviewConfig);
            Debug.Log("[ColorBlastReviewIntegration] Review system initialized for Color Blast");
        }
        #endregion

        #region Game-Specific Review Triggers
        /// <summary>
        /// Call this when player completes a level
        /// </summary>
        /// <param name="levelNumber">The level that was completed</param>
        /// <param name="score">Score achieved in the level</param>
        public void OnLevelCompleted(int levelNumber, int score)
        {
            Debug.Log($"[ColorBlastReviewIntegration] Level {levelNumber} completed with score {score}");
            
            if (_requestOnLevelComplete)
            {
                TryRequestReview($"level {levelNumber} completion");
            }
        }

        /// <summary>
        /// Call this when player achieves a new high score
        /// </summary>
        /// <param name="newScore">The new high score</param>
        /// <param name="previousScore">The previous high score</param>
        public void OnHighScoreAchieved(int newScore, int previousScore)
        {
            Debug.Log($"[ColorBlastReviewIntegration] New high score: {newScore} (previous: {previousScore})");
            
            if (_requestOnHighScore && newScore >= _highScoreThreshold)
            {
                TryRequestReview($"high score achievement ({newScore})");
            }
        }

        /// <summary>
        /// Call this when player achieves a combo
        /// </summary>
        /// <param name="comboCount">Number of consecutive moves in combo</param>
        public void OnComboAchieved(int comboCount)
        {
            Debug.Log($"[ColorBlastReviewIntegration] Combo achieved: {comboCount}");
            
            if (_requestOnComboAchieved && comboCount >= _comboThreshold)
            {
                TryRequestReview($"combo achievement ({comboCount}x)");
            }
        }

        /// <summary>
        /// Call this when player clears a difficult level or achieves a milestone
        /// </summary>
        /// <param name="milestone">Description of the milestone</param>
        public void OnMilestoneAchieved(string milestone)
        {
            Debug.Log($"[ColorBlastReviewIntegration] Milestone achieved: {milestone}");
            TryRequestReview($"milestone: {milestone}");
        }

        /// <summary>
        /// Call this when player has a particularly good game session
        /// </summary>
        public void OnExceptionalGameplay()
        {
            Debug.Log("[ColorBlastReviewIntegration] Exceptional gameplay detected");
            TryRequestReview("exceptional gameplay");
        }
        #endregion

        #region Review Logic
        private void TryRequestReview(string trigger)
        {
            Debug.Log($"[ColorBlastReviewIntegration] Checking review conditions after {trigger}");
            
            if (ReviewManager.Instance.ShouldShowReview())
            {
                Debug.Log($"[ColorBlastReviewIntegration] Requesting review after {trigger}");
                ReviewManager.Instance.RequestReview();
                
                // Track in analytics if available
                TrackReviewRequest(trigger);
            }
            else
            {
                Debug.Log($"[ColorBlastReviewIntegration] Review conditions not met after {trigger}");
                Debug.Log($"[ColorBlastReviewIntegration] Current status: {ReviewTracker.GetTrackingDataSummary()}");
            }
        }

        private void TrackReviewRequest(string trigger)
        {
            // Example: If you have analytics system, track the review request
            // ColorBlastAnalyticsManager.Instance?.TrackEvent("review_requested", new { trigger = trigger });
            Debug.Log($"[ColorBlastReviewIntegration] Review requested via {trigger}");
        }
        #endregion

        #region Event Handling
        private void SubscribeToGameEvents()
        {
            ReviewManager.OnReviewFlowStarted += OnReviewFlowStarted;
            ReviewManager.OnReviewFlowCompleted += OnReviewFlowCompleted;
            ReviewManager.OnReviewFlowFailed += OnReviewFlowFailed;
        }

        private void UnsubscribeFromGameEvents()
        {
            ReviewManager.OnReviewFlowStarted -= OnReviewFlowStarted;
            ReviewManager.OnReviewFlowCompleted -= OnReviewFlowCompleted;
            ReviewManager.OnReviewFlowFailed -= OnReviewFlowFailed;
        }

        private void OnReviewFlowStarted()
        {
            Debug.Log("[ColorBlastReviewIntegration] Review flow started");
            
            // Pause the game during review
            Time.timeScale = 0f;
            
            // Track in analytics
            TrackReviewEvent("review_flow_started");
        }

        private void OnReviewFlowCompleted(ReviewResult result)
        {
            Debug.Log($"[ColorBlastReviewIntegration] Review flow completed: {result}");
            
            // Resume game
            Time.timeScale = 1f;
            
            // Track in analytics
            TrackReviewEvent("review_flow_completed", result.ToString());
            
            // Show thank you message for completed reviews
            if (result == ReviewResult.Completed)
            {
                ShowThankYouMessage();
            }
        }

        private void OnReviewFlowFailed(Google.Play.Review.ReviewErrorCode errorCode)
        {
            Debug.LogWarning($"[ColorBlastReviewIntegration] Review flow failed: {errorCode}");
            
            // Resume game
            Time.timeScale = 1f;
            
            // Track in analytics
            TrackReviewEvent("review_flow_failed", errorCode.ToString());
        }

        private void TrackReviewEvent(string eventName, string parameter = null)
        {
            // Example: Track in your analytics system
            // if (parameter != null)
            // {
            //     ColorBlastAnalyticsManager.Instance?.TrackEvent(eventName, new { result = parameter });
            // }
            // else
            // {
            //     ColorBlastAnalyticsManager.Instance?.TrackEvent(eventName);
            // }
            
            Debug.Log($"[ColorBlastReviewIntegration] Analytics event: {eventName}" + 
                     (parameter != null ? $" with parameter: {parameter}" : ""));
        }

        private void ShowThankYouMessage()
        {
            // Example: Show thank you message in your UI system
            // UIManager.Instance?.ShowPopup("Thank You!", "Thanks for rating Color Blast!");
            
            Debug.Log("[ColorBlastReviewIntegration] Showing thank you message to player");
        }
        #endregion

        #region Public Utilities
        /// <summary>
        /// Force request review (for testing)
        /// </summary>
        [ContextMenu("Force Request Review")]
        public void ForceRequestReview()
        {
            ReviewManager.Instance.RequestReview(forceShow: true);
        }

        /// <summary>
        /// Get current review system status
        /// </summary>
        /// <returns>Status string</returns>
        public string GetReviewStatus()
        {
            return $"Initialized: {ReviewManager.Instance.IsInitialized}, " +
                   $"Should Show: {ReviewManager.Instance.ShouldShowReview()}, " +
                   $"Active: {ReviewManager.Instance.IsReviewFlowActive}, " +
                   $"Data: {ReviewTracker.GetTrackingDataSummary()}";
        }

        /// <summary>
        /// Reset review data for testing
        /// </summary>
        [ContextMenu("Reset Review Data")]
        public void ResetReviewData()
        {
            ReviewTracker.ResetAllData();
            Debug.Log("[ColorBlastReviewIntegration] Review data reset");
        }
        #endregion
    }
}
