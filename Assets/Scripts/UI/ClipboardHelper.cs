using System;
using UnityEngine;

// This code was taken from:
// https://discussions.unity.com/t/how-can-i-add-copy-paste-clipboard-support-to-my-game/44249/8

/// <summary>
/// Thin wrapper around Unity's public system clipboard API.
/// Use ClipBoard to get/set the OS clipboard; logs a warning if unsupported.
/// </summary>
public static class ClipboardHelper
{
    /// <summary>
    /// Get or set the OS clipboard contents (plain text).
    /// </summary>
    public static string ClipBoard
    {
        get
        {
            try
            {
                return GUIUtility.systemCopyBuffer;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Clipboard get failed: {e.Message}");
                return string.Empty;
            }
        }
        set
        {
            try
            {
                GUIUtility.systemCopyBuffer = value;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Clipboard set failed: {e.Message}");
            }
        }
    }
}
