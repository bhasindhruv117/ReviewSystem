using System;

namespace UnityReviewSystem
{
    /// <summary>
    /// Event constants for the review system
    /// </summary>
    public static class ReviewEvents
    {
        // Review flow events
        public const string REVIEW_FLOW_STARTED = "review_flow_started";
        public const string REVIEW_FLOW_COMPLETED = "review_flow_completed";
        public const string REVIEW_FLOW_FAILED = "review_flow_failed";
        public const string REVIEW_FLOW_CANCELLED = "review_flow_cancelled";
        
        // Review trigger events
        public const string REVIEW_CONDITIONS_MET = "review_conditions_met";
        public const string REVIEW_CONDITIONS_NOT_MET = "review_conditions_not_met";
        public const string REVIEW_REQUEST_TRIGGERED = "review_request_triggered";
        
        // Session tracking events
        public const string SESSION_STARTED = "session_started";
        public const string SESSION_ENDED = "session_ended";
        public const string MILESTONE_REACHED = "milestone_reached";
    }

    /// <summary>
    /// Result types for review operations
    /// </summary>
    public enum ReviewResult
    {
        Unknown,
        Completed,
        Cancelled,
        Failed,
        NotInitialized,
        ConditionsNotMet,
        AlreadyActive
    }

    /// <summary>
    /// Review milestone types
    /// </summary>
    public enum ReviewMilestone
    {
        FirstSession,
        FifthSession,
        TenthSession,
        MinimumPlayTimeReached,
        CooldownExpired
    }
}
