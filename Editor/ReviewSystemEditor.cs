using UnityEngine;
using UnityEditor;
using System;

namespace UnityReviewSystem.Editor
{
    /// <summary>
    /// Custom editor window for managing the review system
    /// </summary>
    public class ReviewSystemEditor : EditorWindow
    {
        private ReviewConfig _config;
        private Vector2 _scrollPosition;

        [MenuItem("Tools/Review System/Review System Manager")]
        public static void ShowWindow()
        {
            GetWindow<ReviewSystemEditor>("Review System Manager");
        }

        private void OnGUI()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            EditorGUILayout.LabelField("Review System Manager", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            DrawConfigurationSection();
            EditorGUILayout.Space();
            
            DrawTrackingDataSection();
            EditorGUILayout.Space();
            
            DrawTestingSection();
            EditorGUILayout.Space();
            
            DrawUtilitiesSection();

            EditorGUILayout.EndScrollView();
        }

        private void DrawConfigurationSection()
        {
            EditorGUILayout.LabelField("Configuration", EditorStyles.boldLabel);
            
            ReviewConfig newConfig = (ReviewConfig)EditorGUILayout.ObjectField(
                "Review Config", _config, typeof(ReviewConfig), false);
            
            if (newConfig != _config)
            {
                _config = newConfig;
            }

            if (_config == null)
            {
                EditorGUILayout.HelpBox("No Review Config assigned. Create one or assign an existing config.", MessageType.Warning);
                
                if (GUILayout.Button("Create New Review Config"))
                {
                    CreateReviewConfig();
                }
            }
            else
            {
                EditorGUILayout.LabelField($"Minimum Sessions: {_config.MinimumSessions}");
                EditorGUILayout.LabelField($"Minimum Play Time: {_config.MinimumPlayTimeSeconds}s");
                EditorGUILayout.LabelField($"Cooldown Period: {_config.CooldownDays} days");
                EditorGUILayout.LabelField($"Allow Multiple Reviews: {_config.AllowMultipleReviews}");
                EditorGUILayout.LabelField($"Test Mode: {_config.TestMode}");
            }
        }

        private void DrawTrackingDataSection()
        {
            EditorGUILayout.LabelField("Current Tracking Data", EditorStyles.boldLabel);
            
            if (Application.isPlaying)
            {
                EditorGUILayout.LabelField(ReviewTracker.GetTrackingDataSummary());
                
                EditorGUILayout.LabelField($"Review Manager Initialized: {ReviewManager.Instance.IsInitialized}");
                EditorGUILayout.LabelField($"Review Flow Active: {ReviewManager.Instance.IsReviewFlowActive}");
                EditorGUILayout.LabelField($"Should Show Review: {ReviewManager.Instance.ShouldShowReview()}");
            }
            else
            {
                EditorGUILayout.HelpBox("Enter Play Mode to see tracking data", MessageType.Info);
            }
        }

        private void DrawTestingSection()
        {
            EditorGUILayout.LabelField("Testing", EditorStyles.boldLabel);
            
            if (Application.isPlaying)
            {
                if (GUILayout.Button("Force Request Review"))
                {
                    ReviewManager.Instance.RequestReview(forceShow: true);
                }
                
                if (GUILayout.Button("Check Should Show Review"))
                {
                    bool shouldShow = ReviewManager.Instance.ShouldShowReview();
                    EditorUtility.DisplayDialog("Review Check", 
                        $"Should show review: {shouldShow}", "OK");
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Enter Play Mode to test review functionality", MessageType.Info);
            }
        }

        private void DrawUtilitiesSection()
        {
            EditorGUILayout.LabelField("Utilities", EditorStyles.boldLabel);
            
            if (GUILayout.Button("Reset All Review Data"))
            {
                if (EditorUtility.DisplayDialog("Reset Review Data", 
                    "Are you sure you want to reset all review tracking data? This cannot be undone.", 
                    "Yes", "Cancel"))
                {
                    if (Application.isPlaying)
                    {
                        ReviewTracker.ResetAllData();
                    }
                    else
                    {
                        // Reset PlayerPrefs directly when not in play mode
                        PlayerPrefs.DeleteKey("ReviewSystem_SessionCount");
                        PlayerPrefs.DeleteKey("ReviewSystem_TotalPlayTime");
                        PlayerPrefs.DeleteKey("ReviewSystem_LastReviewDate");
                        PlayerPrefs.DeleteKey("ReviewSystem_UserReviewed");
                        PlayerPrefs.DeleteKey("ReviewSystem_SessionStartTime");
                        PlayerPrefs.Save();
                    }
                    
                    Debug.Log("Review data reset");
                }
            }
            
            if (GUILayout.Button("Open Review System Documentation"))
            {
                Application.OpenURL("https://github.com/your-username/unity-review-system");
            }
        }

        private void CreateReviewConfig()
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "Create Review Config", 
                "ReviewConfig", 
                "asset", 
                "Create a new Review Config asset");
            
            if (!string.IsNullOrEmpty(path))
            {
                ReviewConfig config = ReviewConfig.CreateDefault();
                AssetDatabase.CreateAsset(config, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                
                _config = config;
                Selection.activeObject = config;
            }
        }
    }
}
