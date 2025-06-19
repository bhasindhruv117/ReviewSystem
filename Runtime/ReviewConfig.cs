using UnityEngine;

namespace UnityReviewSystem
{
    /// <summary>
    /// Configuration settings for the review system
    /// </summary>
    [CreateAssetMenu(fileName = "ReviewConfig", menuName = "Review System/Review Config")]
    public class ReviewConfig : ScriptableObject
    {
        [Header("Review Trigger Conditions")]
        [SerializeField, Min(1)] 
        private int _minimumSessions = 5;
        
        [SerializeField, Min(0)] 
        private float _minimumPlayTimeSeconds = 300f; // 5 minutes
        
        [Header("Review Flow Settings")]
        [SerializeField, Min(0)] 
        private int _cooldownDays = 30;
        
        [SerializeField] 
        private bool _allowMultipleReviews = false;
        
        [Header("Debug Settings")]
        [SerializeField] 
        private bool _enableDebugLogs = true;
        
        [SerializeField] 
        private bool _testMode = false;

        #region Public Properties
        /// <summary>
        /// Minimum number of game sessions before showing review
        /// </summary>
        public int MinimumSessions => _minimumSessions;

        /// <summary>
        /// Minimum total play time in seconds before showing review
        /// </summary>
        public float MinimumPlayTimeSeconds => _minimumPlayTimeSeconds;

        /// <summary>
        /// Number of days to wait between review requests
        /// </summary>
        public int CooldownDays => _cooldownDays;

        /// <summary>
        /// Whether to allow multiple review requests for the same user
        /// </summary>
        public bool AllowMultipleReviews => _allowMultipleReviews;

        /// <summary>
        /// Whether to enable debug logging
        /// </summary>
        public bool EnableDebugLogs => _enableDebugLogs;

        /// <summary>
        /// Whether to run in test mode (ignores all conditions)
        /// </summary>
        public bool TestMode => _testMode;
        #endregion

        #region Public Methods
        /// <summary>
        /// Create a default configuration
        /// </summary>
        /// <returns>Default review configuration</returns>
        public static ReviewConfig CreateDefault()
        {
            var config = CreateInstance<ReviewConfig>();
            config._minimumSessions = 5;
            config._minimumPlayTimeSeconds = 300f;
            config._cooldownDays = 30;
            config._allowMultipleReviews = false;
            config._enableDebugLogs = true;
            config._testMode = false;
            return config;
        }

        /// <summary>
        /// Validate the configuration settings
        /// </summary>
        /// <returns>True if configuration is valid</returns>
        public bool IsValid()
        {
            if (_minimumSessions < 1)
            {
                Debug.LogError("[ReviewConfig] Minimum sessions must be at least 1");
                return false;
            }

            if (_minimumPlayTimeSeconds < 0)
            {
                Debug.LogError("[ReviewConfig] Minimum play time cannot be negative");
                return false;
            }

            if (_cooldownDays < 0)
            {
                Debug.LogError("[ReviewConfig] Cooldown days cannot be negative");
                return false;
            }

            return true;
        }
        #endregion

        #region Unity Lifecycle
        private void OnValidate()
        {
            _minimumSessions = Mathf.Max(1, _minimumSessions);
            _minimumPlayTimeSeconds = Mathf.Max(0, _minimumPlayTimeSeconds);
            _cooldownDays = Mathf.Max(0, _cooldownDays);
        }
        #endregion
    }
}
