using System;
using System.Collections;
using UnityEngine;
using Google.Play.Review;

namespace UnityReviewSystem
{
    /// <summary>
    /// Generic singleton manager for handling Google Play Review API
    /// Can be used across any Unity games
    /// </summary>
    public class ReviewManager : MonoBehaviour
    {
        #region Singleton
        private static ReviewManager _instance;
        public static ReviewManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("ReviewManager");
                    _instance = go.AddComponent<ReviewManager>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }
        #endregion

        #region Events
        public static event System.Action<ReviewResult> OnReviewFlowCompleted;
        public static event System.Action<ReviewErrorCode> OnReviewFlowFailed;
        public static event System.Action OnReviewFlowStarted;
        #endregion

        #region Private Fields
        private Google.Play.Review.ReviewManager _playReviewManager;
        private PlayReviewInfo _playReviewInfo;
        private bool _isReviewFlowActive = false;
        private ReviewConfig _config;
        #endregion

        #region Public Properties
        public bool IsReviewFlowActive => _isReviewFlowActive;
        public bool IsInitialized { get; private set; } = false;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeReviewManager();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Initialize the review manager with configuration
        /// </summary>
        /// <param name="config">Review configuration settings</param>
        public void Initialize(ReviewConfig config = null)
        {
            _config = config ?? ReviewConfig.CreateDefault();
            InitializeReviewManager();
        }

        /// <summary>
        /// Request review flow from user
        /// </summary>
        /// <param name="forceShow">Force show review dialog (for testing)</param>
        public void RequestReview(bool forceShow = false)
        {
            if (_isReviewFlowActive)
            {
                Debug.LogWarning("[ReviewManager] Review flow is already active");
                return;
            }

            if (!IsInitialized)
            {
                Debug.LogError("[ReviewManager] Review manager is not initialized");
                OnReviewFlowFailed?.Invoke(ReviewErrorCode.ErrorRequestingFlow);
                return;
            }

            if (!ShouldShowReview() && !forceShow)
            {
                Debug.Log("[ReviewManager] Review conditions not met");
                return;
            }

            StartCoroutine(RequestReviewCoroutine());
        }

        /// <summary>
        /// Check if review should be shown based on configuration
        /// </summary>
        /// <returns>True if review should be shown</returns>
        public bool ShouldShowReview()
        {
            if (_config == null) return false;

            // Check minimum sessions
            if (ReviewTracker.GetSessionCount() < _config.MinimumSessions)
            {
                Debug.Log($"[ReviewManager] Session count ({ReviewTracker.GetSessionCount()}) below minimum ({_config.MinimumSessions})");
                return false;
            }

            // Check minimum playtime
            if (ReviewTracker.GetTotalPlayTime() < _config.MinimumPlayTimeSeconds)
            {
                Debug.Log($"[ReviewManager] Play time ({ReviewTracker.GetTotalPlayTime()}s) below minimum ({_config.MinimumPlayTimeSeconds}s)");
                return false;
            }

            // Check cooldown period
            if (ReviewTracker.IsInCooldownPeriod(_config.CooldownDays))
            {
                Debug.Log($"[ReviewManager] Still in cooldown period ({_config.CooldownDays} days)");
                return false;
            }

            // Check if already reviewed
            if (ReviewTracker.HasUserReviewed() && !_config.AllowMultipleReviews)
            {
                Debug.Log("[ReviewManager] User has already reviewed");
                return false;
            }

            return true;
        }
        #endregion

        #region Private Methods
        private void InitializeReviewManager()
        {
            try
            {
                _playReviewManager = new Google.Play.Review.ReviewManager();
                IsInitialized = true;
                Debug.Log("[ReviewManager] Review manager initialized successfully");
            }
            catch (Exception e)
            {
                Debug.LogError($"[ReviewManager] Failed to initialize: {e.Message}");
                IsInitialized = false;
            }
        }

        private IEnumerator RequestReviewCoroutine()
        {
            _isReviewFlowActive = true;
            OnReviewFlowStarted?.Invoke();

            Debug.Log("[ReviewManager] Starting review flow...");

            // Request review info
            var requestFlowOperation = _playReviewManager.RequestReviewFlow();
            yield return requestFlowOperation;

            if (requestFlowOperation.Error != ReviewErrorCode.NoError)
            {
                Debug.LogError($"[ReviewManager] Failed to request review flow: {requestFlowOperation.Error}");
                _isReviewFlowActive = false;
                OnReviewFlowFailed?.Invoke(requestFlowOperation.Error);
                yield break;
            }

            _playReviewInfo = requestFlowOperation.GetResult();

            // Launch review flow
            var launchFlowOperation = _playReviewManager.LaunchReviewFlow(_playReviewInfo);
            yield return launchFlowOperation;

            _playReviewInfo = null;
            _isReviewFlowActive = false;

            if (launchFlowOperation.Error != ReviewErrorCode.NoError)
            {
                Debug.LogError($"[ReviewManager] Failed to launch review flow: {launchFlowOperation.Error}");
                OnReviewFlowFailed?.Invoke(launchFlowOperation.Error);
            }
            else
            {
                Debug.Log("[ReviewManager] Review flow completed successfully");
                ReviewTracker.MarkReviewShown();
                OnReviewFlowCompleted?.Invoke(ReviewResult.Completed);
            }
        }
        #endregion
    }
}
