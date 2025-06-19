using System;
using UnityEngine;

namespace UnityReviewSystem
{
    /// <summary>
    /// Static utility class for tracking review-related data
    /// </summary>
    public static class ReviewTracker
    {
        #region Constants
        private const string SESSION_COUNT_KEY = "ReviewSystem_SessionCount";
        private const string TOTAL_PLAYTIME_KEY = "ReviewSystem_TotalPlayTime";
        private const string LAST_REVIEW_DATE_KEY = "ReviewSystem_LastReviewDate";
        private const string USER_REVIEWED_KEY = "ReviewSystem_UserReviewed";
        private const string SESSION_START_TIME_KEY = "ReviewSystem_SessionStartTime";
        #endregion

        #region Session Tracking
        /// <summary>
        /// Start tracking a new session
        /// </summary>
        public static void StartSession()
        {
            // Increment session count
            int currentSessions = GetSessionCount();
            PlayerPrefs.SetInt(SESSION_COUNT_KEY, currentSessions + 1);
            
            // Record session start time
            PlayerPrefs.SetString(SESSION_START_TIME_KEY, DateTime.Now.ToBinary().ToString());
            
            PlayerPrefs.Save();
            
            Debug.Log($"[ReviewTracker] Session started. Total sessions: {currentSessions + 1}");
        }

        /// <summary>
        /// End the current session and update play time
        /// </summary>
        public static void EndSession()
        {
            if (PlayerPrefs.HasKey(SESSION_START_TIME_KEY))
            {
                string startTimeString = PlayerPrefs.GetString(SESSION_START_TIME_KEY);
                if (long.TryParse(startTimeString, out long startTimeBinary))
                {
                    DateTime startTime = DateTime.FromBinary(startTimeBinary);
                    float sessionDuration = (float)(DateTime.Now - startTime).TotalSeconds;
                    
                    // Add to total play time
                    float totalPlayTime = GetTotalPlayTime();
                    PlayerPrefs.SetFloat(TOTAL_PLAYTIME_KEY, totalPlayTime + sessionDuration);
                    
                    PlayerPrefs.DeleteKey(SESSION_START_TIME_KEY);
                    PlayerPrefs.Save();
                    
                    Debug.Log($"[ReviewTracker] Session ended. Duration: {sessionDuration:F1}s, Total: {totalPlayTime + sessionDuration:F1}s");
                }
            }
        }

        /// <summary>
        /// Get the total number of sessions
        /// </summary>
        /// <returns>Total session count</returns>
        public static int GetSessionCount()
        {
            return PlayerPrefs.GetInt(SESSION_COUNT_KEY, 0);
        }

        /// <summary>
        /// Get the total play time in seconds
        /// </summary>
        /// <returns>Total play time in seconds</returns>
        public static float GetTotalPlayTime()
        {
            return PlayerPrefs.GetFloat(TOTAL_PLAYTIME_KEY, 0f);
        }
        #endregion

        #region Review Status Tracking
        /// <summary>
        /// Mark that a review was shown to the user
        /// </summary>
        public static void MarkReviewShown()
        {
            PlayerPrefs.SetString(LAST_REVIEW_DATE_KEY, DateTime.Now.ToBinary().ToString());
            PlayerPrefs.SetInt(USER_REVIEWED_KEY, 1);
            PlayerPrefs.Save();
            
            Debug.Log("[ReviewTracker] Review marked as shown");
        }

        /// <summary>
        /// Check if the user has already reviewed
        /// </summary>
        /// <returns>True if user has reviewed</returns>
        public static bool HasUserReviewed()
        {
            return PlayerPrefs.GetInt(USER_REVIEWED_KEY, 0) == 1;
        }

        /// <summary>
        /// Check if we're still in the cooldown period
        /// </summary>
        /// <param name="cooldownDays">Number of cooldown days</param>
        /// <returns>True if still in cooldown period</returns>
        public static bool IsInCooldownPeriod(int cooldownDays)
        {
            if (!PlayerPrefs.HasKey(LAST_REVIEW_DATE_KEY))
                return false;

            string lastReviewDateString = PlayerPrefs.GetString(LAST_REVIEW_DATE_KEY);
            if (long.TryParse(lastReviewDateString, out long lastReviewDateBinary))
            {
                DateTime lastReviewDate = DateTime.FromBinary(lastReviewDateBinary);
                DateTime cooldownEndDate = lastReviewDate.AddDays(cooldownDays);
                return DateTime.Now < cooldownEndDate;
            }

            return false;
        }

        /// <summary>
        /// Get the last review date
        /// </summary>
        /// <returns>Last review date or null if never reviewed</returns>
        public static DateTime? GetLastReviewDate()
        {
            if (!PlayerPrefs.HasKey(LAST_REVIEW_DATE_KEY))
                return null;

            string lastReviewDateString = PlayerPrefs.GetString(LAST_REVIEW_DATE_KEY);
            if (long.TryParse(lastReviewDateString, out long lastReviewDateBinary))
            {
                return DateTime.FromBinary(lastReviewDateBinary);
            }

            return null;
        }
        #endregion

        #region Utility Methods
        /// <summary>
        /// Reset all review tracking data (useful for testing)
        /// </summary>
        public static void ResetAllData()
        {
            PlayerPrefs.DeleteKey(SESSION_COUNT_KEY);
            PlayerPrefs.DeleteKey(TOTAL_PLAYTIME_KEY);
            PlayerPrefs.DeleteKey(LAST_REVIEW_DATE_KEY);
            PlayerPrefs.DeleteKey(USER_REVIEWED_KEY);
            PlayerPrefs.DeleteKey(SESSION_START_TIME_KEY);
            PlayerPrefs.Save();
            
            Debug.Log("[ReviewTracker] All review data reset");
        }

        /// <summary>
        /// Get a summary of current tracking data
        /// </summary>
        /// <returns>Formatted string with tracking data</returns>
        public static string GetTrackingDataSummary()
        {
            return $"Sessions: {GetSessionCount()}, " +
                   $"Play Time: {GetTotalPlayTime():F1}s, " +
                   $"Has Reviewed: {HasUserReviewed()}, " +
                   $"Last Review: {GetLastReviewDate()?.ToString("yyyy-MM-dd") ?? "Never"}";
        }
        #endregion
    }
}
