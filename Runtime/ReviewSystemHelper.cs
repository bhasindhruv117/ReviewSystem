using UnityEngine;

namespace UnityReviewSystem
{
    /// <summary>
    /// Helper component that automatically handles session tracking and review requests
    /// Add this to a GameObject in your scene for automatic functionality
    /// </summary>
    public class ReviewSystemHelper : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private ReviewConfig _reviewConfig;
        
        [Header("Auto-Request Settings")]
        [SerializeField] private bool _autoRequestOnStart = false;
        [SerializeField] private float _autoRequestDelay = 2f;
        
        [Header("Session Tracking")]
        [SerializeField] private bool _trackSessionsAutomatically = true;

        #region Unity Lifecycle
        private void Start()
        {
            InitializeReviewSystem();
            
            if (_trackSessionsAutomatically)
            {
                ReviewTracker.StartSession();
            }
            
            if (_autoRequestOnStart)
            {
                Invoke(nameof(RequestReviewIfConditionsMet), _autoRequestDelay);
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (_trackSessionsAutomatically)
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
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (_trackSessionsAutomatically)
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
        }

        private void OnDestroy()
        {
            if (_trackSessionsAutomatically)
            {
                ReviewTracker.EndSession();
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Initialize the review system with the configured settings
        /// </summary>
        public void InitializeReviewSystem()
        {
            ReviewManager.Instance.Initialize(_reviewConfig);
        }

        /// <summary>
        /// Request review if conditions are met
        /// </summary>
        public void RequestReviewIfConditionsMet()
        {
            if (ReviewManager.Instance.ShouldShowReview())
            {
                ReviewManager.Instance.RequestReview();
            }
        }

        /// <summary>
        /// Force request review (for testing)
        /// </summary>
        public void ForceRequestReview()
        {
            ReviewManager.Instance.RequestReview(forceShow: true);
        }

        /// <summary>
        /// Get current tracking data summary
        /// </summary>
        /// <returns>Summary string</returns>
        public string GetTrackingDataSummary()
        {
            return ReviewTracker.GetTrackingDataSummary();
        }

        /// <summary>
        /// Reset all review data (for testing)
        /// </summary>
        [ContextMenu("Reset Review Data")]
        public void ResetReviewData()
        {
            ReviewTracker.ResetAllData();
            Debug.Log("[ReviewSystemHelper] Review data reset");
        }
        #endregion

        #region Event Handlers
        private void OnEnable()
        {
            ReviewManager.OnReviewFlowStarted += OnReviewFlowStarted;
            ReviewManager.OnReviewFlowCompleted += OnReviewFlowCompleted;
            ReviewManager.OnReviewFlowFailed += OnReviewFlowFailed;
        }

        private void OnDisable()
        {
            ReviewManager.OnReviewFlowStarted -= OnReviewFlowStarted;
            ReviewManager.OnReviewFlowCompleted -= OnReviewFlowCompleted;
            ReviewManager.OnReviewFlowFailed -= OnReviewFlowFailed;
        }

        private void OnReviewFlowStarted()
        {
            Debug.Log("[ReviewSystemHelper] Review flow started");
        }

        private void OnReviewFlowCompleted(ReviewResult result)
        {
            Debug.Log($"[ReviewSystemHelper] Review flow completed with result: {result}");
        }

        private void OnReviewFlowFailed(Google.Play.Review.ReviewErrorCode errorCode)
        {
            Debug.LogWarning($"[ReviewSystemHelper] Review flow failed with error: {errorCode}");
        }
        #endregion
    }
}
